using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarMovement : MonoBehaviour{
    public InputManager input;
    public float MaxSpeed;
    public float MaxBrake;
    public float AccelerationRate;
    public float BrakeRate;
    public float MaxTractionForce;
    public float SteeringResponsiveness;
    public  Vector2 Velocity;
    private bool isPlayer;

    private Rigidbody2D rigidbody;
    public float _currentAcceleration = 0f;
    private float _currentBraking = 0f;
    private float Brake = 0f;

    

    // Use this for initialization
    void Start () {
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        if (gameObject.tag == "Player") isPlayer = true;
        else isPlayer = false;
    }
	
    //get traction force
    public Vector2 GetTractionVector(float maxTraction)
    {
        float inverseMultiplier = 300;
        float minimumMultiplier = 5;
        Vector2.Dot(rigidbody.velocity, transform.right);
        float turningTowards = ExtensionMethods.AngularDirection(gameObject.transform.right, Velocity);
        Vector2 velocityNormal = new Vector2(-rigidbody.velocity.y, rigidbody.velocity.x);
        return Vector2.ClampMagnitude(transform.up * velocityNormal.sqrMagnitude * (GetSteeringAngle())* Mathf.Clamp(inverseMultiplier - (inverseMultiplier*Velocity.sqrMagnitude* Time.fixedDeltaTime),minimumMultiplier,inverseMultiplier)* turningTowards, maxTraction);
    }

    //gets the current steering angle (positive only)
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

    //steers given normalized input and 
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
    
	void FixedUpdate () {
            Velocity = rigidbody.velocity;
            //traction force
            if (ExtensionMethods.AngularDirection(gameObject.transform.right, Velocity) != 0)
            {
                rigidbody.AddForce(GetTractionVector(MaxTractionForce));
            }
        if (isPlayer)
        {
            //gas force
            if (input.GetAccel())
            {
                Accelerate(Vector2.right, _currentAcceleration, MaxSpeed, AccelerationRate, rigidbody);
            }


            //brake force
            if (Input.GetKey(KeyCode.B) && Vector2.Dot(Velocity, gameObject.transform.right) > 0.01f)
            {
                Accelerate(-Vector2.right, _currentBraking, MaxBrake, BrakeRate, rigidbody);
            }
            //steering force
            if (input.GetSteering() > 0.2f || input.GetSteering() < -0.2f)
            {
                SteerTarget(input.GetSteering(), SteeringResponsiveness, rigidbody);
            }
        }
    }

    //___________________________________________________________________________________________________________________

    //TrackTriggerBehavior

    private float StartingAccel;
    private float StartingBrakes;
    private float StartingTractionForce;
    private float StartingSteeringResponse;
    private bool TrackTriggerInit = false;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!TrackTriggerInit && col.gameObject.name == "ActiveGameTrack")
        {
            StartingAccel = this.AccelerationRate;
            StartingBrakes = this.MaxBrake;
            StartingTractionForce = this.MaxTractionForce;
            StartingSteeringResponse = this.SteeringResponsiveness;
            TrackTriggerInit = true;
        }
        else
        {
            this.AccelerationRate = StartingAccel;
            this.MaxBrake = StartingBrakes;
            this.MaxTractionForce = StartingTractionForce;
            this.SteeringResponsiveness = StartingSteeringResponse;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.name == "ActiveGameTrack")
        {
            this.AccelerationRate /= Data.PlayerAccelerationDivisor;
            this.MaxBrake /= Data.PlayerMaxBrakeDivisor;
            this.MaxTractionForce /= Data.PlayerMaxTractionDivisor;
            this.SteeringResponsiveness /= Data.PlayerSteeringResponsivenessDivisor;
        }
    }
}
