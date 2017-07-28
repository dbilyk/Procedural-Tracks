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
    public float AccelRate;
    public float BrakeRate;

    public float MaxTractionForce;
    public float steeringResponse;

    public float SteeringAngle;
    
    public string FacingRelativeToVelocity;

    private bool TrackTriggerInit = false;

    private float Accel = 0f;
    private float Brake = 0f;
    
    private Rigidbody2D rigidbody;
    private Vector2 Velocity;
    public InputManager input;
    private float touchLoc;



    private float StartingTractionForce;
    private float StartingSpeed;
    private float StartingBrakes;
    private float StartingSteeringResponse;
   

    // Use this for initialization
    void Start () {
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
    }
	
    //get traction force
    public Vector2 GetTractionVector(float maxTraction)
    {
        Velocity = rigidbody.velocity;
        float turningTowards = ExtensionMethods.AngularDirection(gameObject.transform.right, Velocity);
        return Vector2.ClampMagnitude(gameObject.transform.up * Velocity.magnitude * turningTowards * (SteeringAngle * 5), maxTraction);
    }



	void Update () {
        
        Velocity = rigidbody.velocity;
        
        SteeringAngle = Mathf.Acos((Vector2.Dot(Velocity,gameObject.transform.right))/(Velocity.magnitude * gameObject.transform.right.magnitude));
        if (float.IsNaN(SteeringAngle))
        {
            SteeringAngle = 0;
        }
        //if (ExtensionMethods.AngularDirection(Velocity, gameObject.transform.right) < 0)
        //{
        //    FacingRelativeToVelocity = "PushRight";
        //}
        //else {
        //    FacingRelativeToVelocity = "PushLeft";
        //}
        //if(SteeringAngle > 0.01f && FacingRelativeToVelocity == "PushLeft")
        //{
            //rigidbody.AddForce(GetTractionVector(MaxTractionForce) * -1);
        //}
        if (SteeringAngle > 0.01f/* && FacingRelativeToVelocity == "PushRight"*/)
        {
            rigidbody.AddForce(GetTractionVector(MaxTractionForce));
        }

        //gas force
        if (Input.GetKey(KeyCode.Space))
        {
            if(Accel + AccelRate < MaxSpeed)
            {
                Accel += AccelRate;
                rigidbody.AddRelativeForce(Vector2.right * Accel * Time.deltaTime);
            }
            else
            {
                rigidbody.AddRelativeForce(Vector2.right * MaxSpeed * Time.deltaTime);
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
                rigidbody.AddForce(Velocity.normalized * -Brake * Time.deltaTime);
            }
            else if(Brake + BrakeRate >= MaxBrake && Velocity.SqrMagnitude() > 0.001f)
            {
                rigidbody.AddForce(Velocity.normalized * -Brake * Time.deltaTime);
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

            rigidbody.AddTorque(steeringResponse * -touchLoc * Mathf.Clamp(Velocity.magnitude, 0, 1f) * Time.deltaTime);
            
        }
        //turn Left
        if (Velocity.magnitude > 0.01f && Input.GetKey(KeyCode.LeftArrow))
        {

            rigidbody.AddTorque(steeringResponse * Mathf.Clamp(Velocity.magnitude, 0, 1f) * Time.deltaTime);
        }


            //turn Right
        
        if (Velocity.magnitude > 0.01f && Input.GetKey(KeyCode.RightArrow))
        {
            rigidbody.AddTorque(-steeringResponse * Mathf.Clamp(Velocity.magnitude, 0, 1f) * Time.deltaTime);
        }
    }


    //TrackTriggerBehavior
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
