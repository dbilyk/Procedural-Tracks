using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Powertrain{
	FWD, RWD, AWD
}




public enum Direction {
Left,Right,Center
}


public class CarPhysics : MonoBehaviour {

	[SerializeField]
	[Tooltip("Make sure each value is high enough that the current speed can reach the next segment of the powerband")]
	float[] Powerband = new float[]{1.3f,0.8f,1.2f,1.1f,1f}, powerSpacing = new float[]{0.25f,0.5f,0.75f,0.88f,1f};

	[SerializeField]
	int targetPowerCurve = 0;

	[SerializeField]
	GameObject CarMesh;

	[SerializeField]
	GameObject[] WheelMeshes = new GameObject[2];

	[SerializeField]
	Rigidbody2D Body, FrontWheel, RearWheel;

	Rigidbody2D[] Wheels;

	[SerializeField]
	HingeJoint2D steeringHinge;


	float SteeringAngle,CurrentTractionReduction=0,CurrentSpeed=0;

	[SerializeField]
  float TopSpeed, 
	//must be negative
	DragCoeff=-0.2f,
	MaxTractionFront=5,
	MaxTractionBack=5, 
	SteeringSensitivity=0.1f, 
	SteeringLimit=45,
	//this gets gradually added when the car is below a certain velocity 
	SlowTraction=2,
	MaxTractionReductionUnderPower=2,
	MaxTractionReductionAtSpeed=2,
	MaxBodyRoll=4,
	BodyRollSpeed=0.08f;

	[SerializeField]
	Powertrain powertrain = Powertrain.RWD;


	bool gasOn = false;
	
	void Start(){
		Wheels = new Rigidbody2D[]{FrontWheel,RearWheel};
	}

	
	// Update is called once per frame
	void FixedUpdate () {
		gasOn = false;
		if(Input.GetKey(KeyCode.Space)){
			ApplyGas();
			gasOn = true;
		}
		if(Input.GetKey(KeyCode.DownArrow)){
			Reverse();
		}
		if(Input.GetKey(KeyCode.LeftArrow)){
			ApplySteering(Direction.Left);
			ApplyBodyRoll(Direction.Left);
		}
		else if(Input.GetKey(KeyCode.RightArrow)){
			ApplySteering(Direction.Right);
			ApplyBodyRoll(Direction.Right);
		}
		else{
			ApplySteering(Direction.Center);
			ApplyBodyRoll(Direction.Center);
		}
		ApplyTraction();
		ApplyDrag();
		
		CurrentSpeed = Body.velocity.sqrMagnitude/100;
		
	}

	void ApplyGas(){
		Rigidbody2D[] target = getPowerWheel();
		bool AWD = (target.Length == 2);
		int divisor = 1;
		if(AWD){
			divisor = 2;
		}


		int c = targetPowerCurve;
		float percentOfTopSpeed = CurrentSpeed/TopSpeed;
		float totalForce;

		int sign = 1,spacingI=0;

		// where are we in the powerband?
		if(percentOfTopSpeed<powerSpacing[0]){
			spacingI = 0;
		}
		else if(percentOfTopSpeed>=powerSpacing[0] && percentOfTopSpeed < powerSpacing[1]){
			spacingI = 1;
		}
		else if(percentOfTopSpeed>=powerSpacing[1] && percentOfTopSpeed < powerSpacing[2]){
			spacingI = 2;
		}
		else{
			spacingI = 3;
		}

//are we adding force or subtracting force along this part of the powerband?
		if(Powerband[spacingI] - Powerband[spacingI+1]< 0){
				sign = 1;
		}else{
			sign = -1;
		}

		totalForce = TopSpeed * (Powerband[spacingI] + (sign*(Mathf.Abs(Powerband[spacingI]-Powerband[spacingI+1])*(CurrentSpeed/(TopSpeed*powerSpacing[spacingI])))));
		
		Vector2 force = new Vector2(0,totalForce/divisor);

		for(int i = 0; i<target.Length; i++){
			target[i].AddRelativeForce(force);

		}

	}

	Rigidbody2D[] getPowerWheel(){
		if(powertrain == Powertrain.FWD){
			return new Rigidbody2D[]{FrontWheel};
		}
		else if(powertrain == Powertrain.RWD){
			return new Rigidbody2D[]{RearWheel};
		}
		else if(powertrain == Powertrain.AWD){
			return new Rigidbody2D[]{FrontWheel,RearWheel};
		}
		else{
			throw new MissingReferenceException("Powertrain is not set properly, cannot get correct wheel to apply power to...");	
		}
	}

	void Reverse(){
		Rigidbody2D[] target = getPowerWheel();
		float velMag = target[0].velocity.SqrMagnitude();
		Vector2 force = new Vector2(0,-100);
		for(int i = 0; i<target.Length; i++){
				target[i].AddRelativeForce(force);
		}
	}

	void ApplySteering(Direction dir){
		int direction = getSteeringDirection(dir);
		
		float newSteeringAngle = direction * Mathf.Clamp(SteeringLimit - (FrontWheel.velocity.sqrMagnitude/TopSpeed), 2*SteeringLimit/3,SteeringLimit);
		Quaternion targetRotation = Quaternion.Euler(0,0,newSteeringAngle);
		FrontWheel.transform.localRotation = Quaternion.Lerp(FrontWheel.transform.localRotation, targetRotation,SteeringSensitivity);
		// if(Mathf.Abs(FrontWheel.transform.localEulerAngles)<0.2f && dir == Direction.Center){
		// 	FrontWheel.transform.localRotation = Quaternion.Euler(0,0,0);
			
		// }
		SteeringAngle = newSteeringAngle;
		renderSteering();
	}

	void renderSteering(){
		for(int i=0; i<2;i++){
			WheelMeshes[i].transform.localEulerAngles = new Vector3(0,0,Mathf.LerpAngle(WheelMeshes[i].transform.localEulerAngles.z,SteeringAngle-10,0.05f));
		}
	}

	int getSteeringDirection(Direction dir){
		if(dir == Direction.Left){
			return 1;
		}
		else if(dir == Direction.Center){
			return 0;
		}
		else{
			return -1;
		}
	}

	void ApplyTraction(){
		float slowTraction = getSlowTractionModifier();
		float powerWheelTractionReducer = getPowerWheelTractionReducer();
		float speedTractionModifier = MaxTractionReductionAtSpeed * (Mathf.Log(CurrentSpeed+0.0001f)/Mathf.Log(TopSpeed));
		float thisMaxTraction = MaxTractionFront + slowTraction-speedTractionModifier;

		for(int i=0;i<Wheels.Length;i++){
			if(i ==1){
				thisMaxTraction = MaxTractionBack + slowTraction;
			}
			//this gradually reduces traction on the power wheel when gas is on.
			if(powertrain == Powertrain.AWD){
				thisMaxTraction -= CurrentTractionReduction;
			}
			if(powertrain == Powertrain.FWD && i == 0){
				thisMaxTraction -= CurrentTractionReduction;
			}
			if(powertrain == Powertrain.RWD && i ==1){
				thisMaxTraction -= CurrentTractionReduction;
			}
			//Debug.Log(thisMaxTraction);
			Rigidbody2D wheel = Wheels[i];
			
			Vector2 lateral_velocity = wheel.transform.right * Mathf.Clamp(Vector2.Dot(wheel.velocity, wheel.transform.right),-thisMaxTraction,thisMaxTraction);
			Vector2 lateral_friction = -lateral_velocity*thisMaxTraction; 
			Debug.Log(thisMaxTraction);
			wheel.AddForce(lateral_friction,ForceMode2D.Force);
		}
	}

//MAGIC number here 40mph
	float getSlowTractionModifier(){
		return Mathf.Clamp(SlowTraction - (SlowTraction *(Mathf.Pow(CurrentSpeed,2)/(TopSpeed/5))),0,SlowTraction);
	}

	float getPowerWheelTractionReducer(){
		if(gasOn){
			float speedMod = Mathf.Clamp(MaxTractionReductionUnderPower-(MaxTractionReductionUnderPower*(CurrentSpeed/(TopSpeed/4))),0,MaxTractionReductionUnderPower);
			CurrentTractionReduction = Mathf.Lerp(CurrentTractionReduction,speedMod,0.1f);
		}
		else{
			CurrentTractionReduction = Mathf.Lerp(CurrentTractionReduction,0,0.12f);
			
		}
		return 1;
	}

	void ApplyDrag(){
		Vector2 drag = Body.velocity.normalized * DragCoeff * Body.velocity.sqrMagnitude;
		Body.AddForce(drag,ForceMode2D.Force);
	}

	void ApplyBodyRoll(Direction dir){
		int sign = 0;
		int gasSign = 0;
		if(dir == Direction.Left){
			sign =-1;
		}
		else if(dir == Direction.Right){
			sign = 1;
		}
		else{
			sign = 0;
		}

		if(gasOn){
			//gasSign = -1;
		}

		float targetRotation = MaxBodyRoll * (Mathf.Clamp(CurrentSpeed/(TopSpeed/5),0,1));
		CarMesh.transform.localRotation = Quaternion.Lerp(CarMesh.transform.localRotation,Quaternion.Euler(targetRotation*gasSign,targetRotation* sign,0),BodyRollSpeed);
	}

}
