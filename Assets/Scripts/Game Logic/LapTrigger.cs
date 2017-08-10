using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapTrigger : MonoBehaviour {
    public delegate void LapCompleteDel(int CarIndex);
    public static LapCompleteDel OnLapComplete;

    public float PctOfCheckpointsThatConstitutesALap = 90f;
    void OnTriggerEnter2D(Collider2D col)
    {

         for (int i = 0; i < Data.CarPoleData.Count; i++)
        {
            if (col.gameObject == Data.CarPoleData[i].CarObject) {
                OnLapComplete(i);

                if (Data.CarPoleData[i].Curr_LapNumber == 0)
                {
                    Data.CarPoleData[i].Curr_LapNumber += 1;
                    Data.CarPoleData[i].Curr_LapStartTime = Time.time;
                    Debug.Log(Data.CarPoleData[i].Curr_LapNumber);

                }
                if (Data.CarPoleData[i].TotalCheckpointsPassedThisLap >= Data.Curr_PoleCheckpoints.Count * (PctOfCheckpointsThatConstitutesALap / 100))
                {
                    Data.CarPoleData[i].Curr_LapNumber += 1;
                    Data.CarPoleData[i].Curr_LapStartTime = Time.time;

                }
            }

        }
        
        
    }
	
}
