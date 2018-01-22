using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInputController : MonoBehaviour {
    public CarMovement CarMovement;
    public int steeringWaypointLookahead = 3;
    public int throttleWaypointLookahead = 8;
    public float minAngleToEngageBraking = 10;
    public float minVelocityMagToStopBraking = 4;
    [Tooltip ("controls how hard cars will stomp on the brake.  set to zero for most 'professional' braking performance")]
    public float minBrakingApplication;
    public float minAccelApplication;
    [SerializeField]
    User user;

    public Vector2 SteeringTarget;
    private List<Vector2> RacingLine;
    private Rigidbody2D rigidbody;
    // Use this for initialization

    void OnEnable () {
        user = GameObject.FindGameObjectWithTag ("User").GetComponent<User> ();
        RacingLine = user.ActiveTrack.RacingLinePoints;

    }

    void Start () {
        rigidbody = gameObject.transform.GetComponent<Rigidbody2D> ();

        //Data.Curr_RacingLinePoints.DebugPlot(Data.green);

    }

    int GetSteeringWaypoint (int nearestWP, int steeringWPLookahead) {
        int newPosSteer = nearestWP + steeringWaypointLookahead;
        //move this waypoint logic somewhere more... logical....
        if (nearestWP + steeringWPLookahead >= RacingLine.Count) {
            newPosSteer = (newPosSteer + steeringWaypointLookahead) - RacingLine.Count;
        }
        return newPosSteer;
    }

    public float GetSteeringInput (Vector2 targetWaypoint) {
        //cross product is 0 when the two vectors its perpendicular to are the same.
        Vector3 targetDelta = targetWaypoint - (Vector2) transform.position;
        float angleDifference = Vector2.Angle (transform.right, targetDelta);
        Vector3 cross = Vector3.Cross (transform.right, targetDelta);
        return Mathf.Clamp (angleDifference * cross.z, -1, 1);

        //return Mathf.Clamp(angleDifference/180, 0, 1f) * Mathf.Clamp(ExtensionMethods.AngularDirection(gameObject.transform.right, targetWaypoint),-1,1);

    }

    float GetAccelerationInput (Vector2 steeringTarget) {
        Vector3 targetDelta = steeringTarget - (Vector2) transform.position;
        float angleDifference = Mathf.Abs (Vector2.Angle (transform.right, targetDelta));
        if (angleDifference > minAngleToEngageBraking && rigidbody.velocity.sqrMagnitude > minVelocityMagToStopBraking)
            angleDifference = -angleDifference;
        if (rigidbody.velocity.sqrMagnitude <= minVelocityMagToStopBraking) {
            angleDifference = 1;
        }

        return Mathf.Clamp (minAngleToEngageBraking / angleDifference, -1, 1);
    }

    // Update is called once per frame
    void FixedUpdate () {
        int CurrentNearest = ExtensionMethods.GetNearestInList (transform.position, RacingLine);
        int CurrentSteering = GetSteeringWaypoint (CurrentNearest, steeringWaypointLookahead);
        SteeringTarget = RacingLine[CurrentSteering];
        float steeringInput = GetSteeringInput (SteeringTarget);
        CarMovement.SteerTarget (steeringInput, CarMovement.SteeringResponsiveness, this.GetComponent<Rigidbody2D> ());
        //Get Accel input is responsible for both accel and braking
        if (GetAccelerationInput (SteeringTarget) > 0) {
            CarMovement.Accelerate (Vector2.right * Mathf.Clamp (GetAccelerationInput (SteeringTarget), minAccelApplication, 1), CarMovement.MaxSpeed, CarMovement.AccelerationRate, rigidbody);
        } else {
            CarMovement.Deccelerate (-rigidbody.velocity.normalized * Mathf.Clamp (GetAccelerationInput (SteeringTarget), minBrakingApplication, 1), CarMovement.MaxBrake, CarMovement.BrakeRate, rigidbody);
            //ExtensionMethods.DebugPlot(rigidbody.transform.position, Data.red);

        }
    }
}