using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInputController : MonoBehaviour {
    public CarMovement CarMovement;
    public int steeringWaypointLookahead = 3;
    public int throttleWaypointLookahead = 8;
    public float minAngleToEngageBraking = 10;
    public float minVelocityMagToStopBraking = 4;
    [Tooltip("controls how hard cars will stomp on the brake.  set to zero for most professional braking performance")]
    public float minBrakingApplication;
    public float minAccelApplication;

    private List<Vector2> RacingLine = Data.Curr_RacingLinePoints;
    private Rigidbody2D rigidbody;
	// Use this for initialization
	void Start () {
        rigidbody = gameObject.transform.GetComponent<Rigidbody2D>();

        //Data.Curr_RacingLinePoints.DebugPlot(Data.green);

    }

    int GetNearestWaypoint()
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
            else
            {
                if (((Vector2)transform.position - RacingLine[i]).sqrMagnitude < ((Vector2)transform.position - RacingLine[closestWaypointIndex]).sqrMagnitude)
                {
                    closestWaypointIndex = i;
                }
            }
        }
        return closestWaypointIndex;
    }

    int GetSteeringWaypoint(int nearestWP, int steeringWPLookahead)
    {
        int newPosSteer = nearestWP + steeringWaypointLookahead;
        //move this waypoint logic somewhere more... logical....
        if (nearestWP + steeringWPLookahead >= RacingLine.Count)
        {
            newPosSteer = (newPosSteer + steeringWaypointLookahead) - RacingLine.Count;
        }
        return newPosSteer;
    }

    float GetSteeringInput(Vector2 targetWaypoint)
    {
        //cross product is 0 when the two vectors its perpendicular to are the same.
        Vector3 targetDelta = targetWaypoint - (Vector2)transform.position;
        float angleDifference = Vector2.Angle(transform.right, targetDelta);
        Vector3 cross = Vector3.Cross(transform.right, targetDelta);
        return Mathf.Clamp(angleDifference * cross.z,-1,1);

        //return Mathf.Clamp(angleDifference/180, 0, 1f) * Mathf.Clamp(ExtensionMethods.AngularDirection(gameObject.transform.right, targetWaypoint),-1,1);


    }

    
    float GetAccelerationInput(Vector2 steeringTarget)
    {
        Vector3 targetDelta = steeringTarget - (Vector2)transform.position;
        float angleDifference = Mathf.Abs(Vector2.Angle(transform.right, targetDelta));
        if (angleDifference > minAngleToEngageBraking && rigidbody.velocity.sqrMagnitude > minVelocityMagToStopBraking)
            angleDifference = -angleDifference;
        if (rigidbody.velocity.sqrMagnitude <= minVelocityMagToStopBraking)
        {
            angleDifference = 1;
        }
        
        return Mathf.Clamp(minAngleToEngageBraking/angleDifference,-1,1);
    }

    // Update is called once per frame
    void Update() {
        int CurrentNearest = GetNearestWaypoint();
        int CurrentSteering = GetSteeringWaypoint(CurrentNearest,steeringWaypointLookahead);
        Vector2 steeringTarget = RacingLine[CurrentSteering];
        float steeringInput = GetSteeringInput(steeringTarget);
        CarMovement.SteerTarget(steeringInput,CarMovement.SteeringResponsiveness,this.GetComponent<Rigidbody2D>());
        //Get Accel input is responsible for both accel and braking
        if (GetAccelerationInput(steeringTarget) >0)
        {
            CarMovement.Accelerate(Vector2.right * Mathf.Clamp(GetAccelerationInput(steeringTarget), minAccelApplication, 1), CarMovement.MaxSpeed, CarMovement.AccelerationRate, rigidbody);
        }
        else
        {
            CarMovement.Deccelerate(-rigidbody.velocity.normalized * Mathf.Clamp(GetAccelerationInput(steeringTarget), minBrakingApplication, 1), CarMovement.MaxBrake, CarMovement.BrakeRate, rigidbody);
            //ExtensionMethods.DebugPlot(rigidbody.transform.position, Data.red);
        }
    }
}
