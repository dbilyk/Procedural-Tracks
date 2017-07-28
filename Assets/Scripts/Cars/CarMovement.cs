using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public interface CarDynamics
{
    //
    void ApplySteeringInput();
    

}

public class CarMovement : MonoBehaviour{

    public MapCreator MapGen;
    public float MaxSpeed;
    public float MaxBrake;
    public float AccelRate;
    public float BrakeRate;

    public float MaxTractionForce;
    public float steeringResponse;

    public float SteeringAngle;
    public Vector2 CurrentTraction;

    public string FacingRelativeToVelocity;
    private float Accel = 0f;
    private float Brake = 0f;
    
    private Rigidbody2D RB;
    private Vector2 Velocity;
    public InputManager input;
    private float touchLoc;
    
    // Use this for initialization
    void Start () {
        RB = gameObject.GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
    //traction force must be proportional to velocity.  is velocity is 
	void Update () {
        
        Velocity = RB.velocity;
        CurrentTraction = Vector2.ClampMagnitude(gameObject.transform.up * Velocity.magnitude * (SteeringAngle * 5),MaxTractionForce);
        SteeringAngle = Mathf.Acos((Vector2.Dot(Velocity,gameObject.transform.right))/(Velocity.magnitude * gameObject.transform.right.magnitude));
        if (float.IsNaN(SteeringAngle))
        {
            SteeringAngle = 0;
        }
        if (ExtensionMethods.AngularDirection(Velocity, gameObject.transform.right) < 0)
        {
            FacingRelativeToVelocity = "PushRight";
        }
        else {
            FacingRelativeToVelocity = "PushLeft";
        }
        if(!float.IsNaN(SteeringAngle) && SteeringAngle > 0.01f && FacingRelativeToVelocity == "PushLeft")
        {
            RB.AddForce(CurrentTraction * -1);
        }
        if (!float.IsNaN(SteeringAngle) && SteeringAngle > 0.01f && FacingRelativeToVelocity == "PushRight")
        {
            RB.AddForce(CurrentTraction);
        }

        //gas force
        if (Input.GetKey(KeyCode.Space))
        {
            if(Accel + AccelRate < MaxSpeed)
            {
                Accel += AccelRate;
                RB.AddRelativeForce(Vector2.right * Accel * Time.deltaTime);
            }
            else
            {
                RB.AddRelativeForce(Vector2.right * MaxSpeed * Time.deltaTime);
            }
        }
        if (!Input.GetKey(KeyCode.Space) && Accel > 1f) {
            //Accel -= AccelRate/10;
        }


        //brake force
        if (Input.GetKey(KeyCode.B))
        {
            //Debug.Log(Velocity.SqrMagnitude());

            if (Brake + BrakeRate < MaxBrake && Velocity.SqrMagnitude() > 0.001f)
            {
                
                Brake += BrakeRate;
                RB.AddForce(Velocity.normalized * -Brake * Time.deltaTime);
            }
            else if(Brake + BrakeRate >= MaxBrake && Velocity.SqrMagnitude() > 0.001f)
            {
                RB.AddForce(Velocity.normalized * -Brake * Time.deltaTime);
            }
            else
            {
                return;
            }
        }
        if (!Input.GetKey(KeyCode.B) && Brake > 1f)
        {
            Brake -= BrakeRate/5;
        }



        //iOS control
        if (Velocity.magnitude > 0.01f && Input.touchCount > 0)
        {
            touchLoc = (Input.touches[0].position.x - (Screen.width / 2)) / (Screen.width / 2);
            
            RB.AddTorque(steeringResponse * -touchLoc * Mathf.Clamp(Velocity.magnitude, 0, 1f) * Time.deltaTime);
            
        }
        //turn Left
        if (Velocity.magnitude > 0.01f && Input.GetKey(KeyCode.LeftArrow))
        {
                
            RB.AddTorque(steeringResponse * Mathf.Clamp(Velocity.magnitude, 0, 1f) * Time.deltaTime);
        }


            //turn Right
        
        if (Velocity.magnitude > 0.01f && Input.GetKey(KeyCode.RightArrow))
        {
            RB.AddTorque(-steeringResponse * Mathf.Clamp(Velocity.magnitude, 0, 1f) * Time.deltaTime);
        }
    }
}
