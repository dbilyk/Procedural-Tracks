using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInputController : MonoBehaviour {
    public CarMovement CarMovement;
    public int steeringWaypointLookahead = 3;
    public int throttleWaypointLookahead = 8;

    private List<Vector2> RacingLine = Data.Curr_RacingLinePoints;

	// Use this for initialization
	void Start () {
		
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

   

    //not working....
    float GetSteeringInput(Vector2 targetWaypoint)
    {
        float angleDifference = Vector2.Angle(GetComponent<Rigidbody2D>().velocity, targetWaypoint);
        
        return Mathf.Clamp(angleDifference/180, 0, 1f) * Mathf.Clamp(ExtensionMethods.AngularDirection(gameObject.transform.right, targetWaypoint),-1,1);


    }

    // Update is called once per frame
    void Update() {
        int nearest = GetNearestWaypoint();
       
        Vector2 steeringTarget = RacingLine[nearest + steeringWaypointLookahead];
        float steeringInput = GetSteeringInput(steeringTarget);
        ExtensionMethods.DebugPlot(steeringTarget, Data.red);
        CarMovement.SteerTarget(steeringInput,40,this.GetComponent<Rigidbody2D>());
        //CarMovement.Accelerate(Vector2.right,1,1,1,this.GetComponent<Rigidbody2D>());
        //CarMovement.SteerTarget(steeringInput,20, this.GetComponent<Rigidbody2D>());
    }
}
