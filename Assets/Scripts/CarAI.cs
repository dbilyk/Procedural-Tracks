﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAI : MonoBehaviour {

    public MapGen MapGen;
    public List<Vector2> RacingLine;
    public float MaxSpeed;
    public float MaxBrake;
    public float AccelRate;
    public float BrakeRate;

    public float MaxTractionForce;
    public float steeringResponse;

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

    //returns positive number on the left side of a vector, negative on the right, and 0 on the same vector
    public static float AngleDir(Vector2 A, Vector2 B)
    {
        return -A.x * B.y + A.y * B.x;
    }

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



    // Use this for initialization
    void Start()
    {
        RB = gameObject.GetComponent<Rigidbody2D>();
        RacingLine = MapGen.RacingLinePoints;


    }


    int newPosSteer;
    int newPosThrottle;
    int oldPos;

    public float force = 0.0000001f;



    // Update is called once per frame
    //traction force must be proportional to velocity.  is velocity is 
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

        if (AngleDir(Velocity, gameObject.transform.right) < 0)
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
       

        //    //brake force
        //    if (Input.GetKey(KeyCode.B))
        //    {
        //        //Debug.Log(Velocity.SqrMagnitude());

        //        if (Brake + BrakeRate < MaxBrake && Velocity.SqrMagnitude() > 0.001f)
        //        {

        //            Brake += BrakeRate;
        //            RB.AddForce(Velocity.normalized * -Brake * Time.deltaTime);
        //        }
        //        else if (Brake + BrakeRate >= MaxBrake && Velocity.SqrMagnitude() > 0.001f)
        //        {
        //            RB.AddForce(Velocity.normalized * -Brake * Time.deltaTime);
        //        }
        //        else
        //        {
        //            return;
        //        }
        //    }
        //    if (!Input.GetKey(KeyCode.B) && Brake > 1f)
        //    {
        //        Brake -= BrakeRate / 5;
        //    }



    
}

