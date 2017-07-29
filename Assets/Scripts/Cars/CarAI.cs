using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAI : MonoBehaviour {

    public MapCreator MapGen;
    public List<Vector2> RacingLine;
    public float MaxSpeed;
    public float MaxBrake;
    public float AccelRate;
    public float BrakeRate;

    public float MaxTractionForce;
    public float steeringResponse;

    public bool TrackTriggerInit;
    public float SteeringAngle;
    public Vector2 CurrentTraction;

    public string FacingRelativeToVelocity;

    //how many waypoints ahead of the closest racing line waypoint does the car aim?
    public int RacingLineWaypointsLookahead = 5;

    //how many waypoints ahead of the closest racing line waypoint does the car aim when calculating throttle?
    public int RacingLineWaypointsThrottleLookahead = 8;

    private float Accel = 0f;
    private float Brake = 0f;

    private Rigidbody2D RB;
    private Vector2 Velocity;

    int SteerTowardsWaypoint()
    {
        int ArraySearchStartLocation;
        int ArraySearchEndLocation;
        int WaypointCount = RacingLine.Count;
        int QuarterOfArray;
        int closestWaypointIndex = -1;
        //check if array count is divisible evenly into quadrants for looping.
        if (WaypointCount % 4 != 0)
        {
            QuarterOfArray = Mathf.RoundToInt(WaypointCount / 4);
        }
        else
        {
            QuarterOfArray = WaypointCount / 4;
        }

        if (transform.position.x > 0 && transform.position.y > 0)
        {
            ArraySearchStartLocation = 0;
            ArraySearchEndLocation = QuarterOfArray;
        }
        else if (transform.position.x > 0 && transform.position.y < 0)
        {
            ArraySearchStartLocation = QuarterOfArray;
            ArraySearchEndLocation = QuarterOfArray * 2;
        }
        else if (transform.position.x < 0 && transform.position.y < 0)
        {
            ArraySearchStartLocation = QuarterOfArray * 2;
            ArraySearchEndLocation = QuarterOfArray * 3;
        }
        else
        {
            ArraySearchStartLocation = QuarterOfArray * 3;
            ArraySearchEndLocation = WaypointCount;
        }

        //find closest point to my transform
        //for (int i = ArraySearchStartLocation + 1; i < ArraySearchEndLocation; i++)
        for (int i = 1; i < WaypointCount; i++)

            {

                if (closestWaypointIndex == -1)
            {
                if (((Vector2)transform.position - RacingLine[i]).sqrMagnitude < ((Vector2)transform.position - RacingLine[i - 1]).sqrMagnitude)
                    closestWaypointIndex = i;
                else closestWaypointIndex = i - 1;
            }
            else {
                if (((Vector2)transform.position - RacingLine[i]).sqrMagnitude < ((Vector2)transform.position - RacingLine[closestWaypointIndex]).sqrMagnitude)
                {
                    closestWaypointIndex = i;
                }
            }
        }
        return closestWaypointIndex;
    }

    void Start()
    {
        RB = gameObject.GetComponent<Rigidbody2D>();
        MapGen = MapCreator.FindObjectOfType<MapCreator>();
        RacingLine = Data.Curr_RacingLinePoints;
        
    }
    int newPosSteer;
    int newPosThrottle;
    int oldPos;
    public float force = 0.00000001f;

    void Update()
    {
        newPosSteer = SteerTowardsWaypoint();
        newPosThrottle = SteerTowardsWaypoint();

        if (newPosSteer + RacingLineWaypointsLookahead >= RacingLine.Count)
        {
            newPosSteer = (newPosSteer+RacingLineWaypointsLookahead)-RacingLine.Count;
        }


        if (newPosThrottle + RacingLineWaypointsThrottleLookahead >= RacingLine.Count)
        {
            newPosThrottle = (newPosThrottle + RacingLineWaypointsThrottleLookahead) - RacingLine.Count;
        }

        Vector2 target = RacingLine[newPosSteer + RacingLineWaypointsLookahead];

        Vector3 targetDelta = target - (Vector2)transform.position;

        //get the angle between transform.forward and target delta
        float angleDiff = Vector3.Angle(transform.right, targetDelta);


        // get its cross product, which is the axis of rotation to
        // get from one vector to the other
        Vector3 cross = Vector3.Cross(transform.right, targetDelta);

        // apply torque along that axis according to the magnitude of the angle.
        RB.AddTorque(cross.z * angleDiff / 10 * force);

        


        Velocity = RB.velocity;
        CurrentTraction = Vector2.ClampMagnitude(gameObject.transform.up * Velocity.magnitude * (SteeringAngle * 5), MaxTractionForce);
        SteeringAngle = Mathf.Acos((Vector2.Dot(Velocity, gameObject.transform.right)) / (Velocity.magnitude * gameObject.transform.right.magnitude));
        if (float.IsNaN(SteeringAngle))
        {
            SteeringAngle = 0;
        }

        if (ExtensionMethods.AngularDirection(Velocity, gameObject.transform.right) < 0)
        {
            FacingRelativeToVelocity = "PushRight";

        }
        else
        {
            FacingRelativeToVelocity = "PushLeft";

        }

        if (!float.IsNaN(SteeringAngle) && SteeringAngle > 0.01f && FacingRelativeToVelocity == "PushLeft")
        {
            RB.AddForce(CurrentTraction * -1);

        }
        if (!float.IsNaN(SteeringAngle) && SteeringAngle > 0.01f && FacingRelativeToVelocity == "PushRight")
        {
            RB.AddForce(CurrentTraction);
        }

        if (newPosSteer != oldPos)
        {
            List<Vector2> x = new List<Vector2>();
            x.Add(RacingLine[newPosSteer]);
            //plot closest point
            //MapGen.DebugPlot(x, new Color32(0, 0, 255, 255));
        }
        oldPos = SteerTowardsWaypoint();
        


        //THROTTLE FORMULA-----------------------------------------------
        float angleDiff2 = Vector3.Angle(transform.right, RacingLine[newPosThrottle + RacingLineWaypointsThrottleLookahead] - (Vector2)transform.position);
        if (Accel + AccelRate < MaxSpeed)
        {
            Accel += AccelRate;
            RB.AddRelativeForce(Vector2.right * Mathf.Clamp((Accel - angleDiff2*angleDiff2*angleDiff2), Accel/2, Accel) * Time.deltaTime);
        }
        else
        {
            RB.AddRelativeForce(Vector2.right * Mathf.Clamp((MaxSpeed - angleDiff2*angleDiff2* angleDiff2),MaxSpeed/2,MaxSpeed) * Time.deltaTime);
        }



    }
    private float StartingTractionForce;
    private float StartingSpeed;
    private float StartingBrakes;
    private float StartingSteeringResponse;
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
            this.MaxTractionForce /= 2;
            this.MaxSpeed /= 2;
            this.MaxBrake /= 2;
            this.steeringResponse /= 2f;
        }
    }

}


