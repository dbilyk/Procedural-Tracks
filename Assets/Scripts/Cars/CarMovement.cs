using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public interface CarDynamics
{
    /// <summary>
    /// Steering Input should be a value between -1 and 1.
    /// </summary>
    /// <param name="SteeringInput"></param>
    void ApplySteeringInput(float SteeringInput);

    void ApplyBrakingInput(bool brakingInput);

    void ApplyThrottle(bool Throttle);
}

public class CarMovement : MonoBehaviour{

    public float MaxSpeed;
    public float MaxBrake;
    public float AccelerationRate;
    public float BrakeRate;

    public float MaxTractionForce;
    public float steeringResponse;
    
    public string FacingRelativeToVelocity;


    private float _currentAcceleration = 0f;
    private float _currentBraking = 0f;
    private float Brake = 0f;
    private Rigidbody2D rigidbody;
    private Vector2 Velocity;
    public InputManager input;
    private float touchLoc;



    
   

    // Use this for initialization
    void Start () {
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        
    }
	
    //get traction force
    public Vector2 GetTractionVector(float maxTraction)
    {
        float inverseMultiplier = 400;
        float minimumMultiplier = 5;
        Vector2.Dot(rigidbody.velocity, transform.right);
        float turningTowards = ExtensionMethods.AngularDirection(gameObject.transform.right, Velocity);
        Vector2 velocityNormal = new Vector2(-rigidbody.velocity.y, rigidbody.velocity.x);
        Debug.Log(inverseMultiplier * Velocity.SqrMagnitude());
        return Vector2.ClampMagnitude(transform.up * velocityNormal.sqrMagnitude * (GetSteeringAngle())* Mathf.Clamp(inverseMultiplier - (inverseMultiplier*Velocity.sqrMagnitude),minimumMultiplier,inverseMultiplier)* turningTowards, maxTraction);
    }

    //gets the current steering angle...
    public float GetSteeringAngle()
    {
        float steeringAngle  = Mathf.Acos((Vector2.Dot(Velocity, gameObject.transform.right)) / (Velocity.magnitude/ gameObject.transform.right.magnitude));
        if (float.IsNaN(steeringAngle))
            steeringAngle = 0;
        return steeringAngle;
    }

    //accelerates the target
    public void Accelerate(Vector2 direction, float currentAcceleration, float maxSpeed, float accelerationRate, Rigidbody2D target)
    {
        target.AddRelativeForce(direction * Mathf.Lerp(currentAcceleration, maxSpeed, accelerationRate * Time.fixedDeltaTime));
    }

    public void SteerTarget(float input, float responsiveness, Rigidbody2D target)
    {
        target.AddTorque(input * responsiveness * Mathf.Clamp(Velocity.magnitude / 2f, 0, 1f) * Time.deltaTime);
    }
    //debug
    void OnGUI()
    {
        GUIContent content = new GUIContent();
        content.text = GetSteeringAngle().ToString();
        GUI.Label(new Rect(50,50,50,50),  content);
    }

	void Update () {
        
        Velocity = rigidbody.velocity;
        
       //traction force
        //if (ExtensionMethods.AngularDirection(gameObject.transform.right, Velocity) != 0)
        //{
            rigidbody.AddForce(GetTractionVector(MaxTractionForce));
        //}

        //gas force
        if (input.GetAccel(DeviceType.Desktop))
        {
            Accelerate(Vector2.right,_currentAcceleration, MaxSpeed, AccelerationRate, rigidbody);
        }
        
        //brake force
        if (Input.GetKey(KeyCode.B) && Vector2.Dot(Velocity, gameObject.transform.right) > 0.01f)
        {
            Accelerate(-Vector2.right,_currentBraking,MaxBrake,BrakeRate,rigidbody);
        }
        //steering force
        if (input.GetSteering() >0.2f || input.GetSteering() < -0.2f)
        {
            SteerTarget(input.GetSteering(), steeringResponse, rigidbody);
        }
        
    }





    //___________________________________________________________________________________________________________________

    //TrackTriggerBehavior

    private float StartingTractionForce;
    private float StartingSpeed;
    private float StartingBrakes;
    private float StartingSteeringResponse;
    private bool TrackTriggerInit = false;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!TrackTriggerInit && col.gameObject.name == "ActiveGameTrack")
        {
            StartingTractionForce = this.MaxTractionForce;
            StartingSpeed = this.MaxSpeed;
            StartingBrakes = this.MaxBrake;
            StartingSteeringResponse = this.steeringResponse;
            TrackTriggerInit = true;
        }
        else
        {
            this.MaxTractionForce = StartingTractionForce;
            this.MaxSpeed = StartingSpeed;
            this.MaxBrake = StartingBrakes;
            this.steeringResponse = StartingSteeringResponse;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.name == "ActiveGameTrack")
        {
            this.MaxTractionForce /= 1.5f;
            this.MaxSpeed /= 1.5f;
            this.MaxBrake /= 1.5f;
            this.steeringResponse /= 1.2f;
        }
    }
}
