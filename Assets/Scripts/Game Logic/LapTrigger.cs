using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapTrigger : MonoBehaviour {
    public float PctOfCheckpointsThatConstitutesALap = 90f;
    void OnTriggerEnter2D(Collider2D col)
    {
        for(int i = 0; i < Data.CarPoleData.Count; i++){
            if (Data.CarPoleData[i].TotalCheckpointsPassedThisLap >= Data.Curr_PoleCheckpoints.Count* (PctOfCheckpointsThatConstitutesALap/100))
            {
                Data.CarPoleData[i].Curr_Lap += 1;

            }


        }
    }
	
}
