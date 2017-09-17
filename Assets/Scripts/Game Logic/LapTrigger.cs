using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapTrigger : MonoBehaviour {
    public delegate void LapCompleteDel(int CarIndex);
    public static event LapCompleteDel OnLapComplete;

    

    void OnTriggerEnter2D(Collider2D col)
    {
        for (int i = 0; i < Data.CarPoleData.Count; i++)
        {
            if (col.gameObject == Data.CarPoleData[i].CarObject) {
                OnLapComplete(i);
                break;
            }
           
        }
        
        
    }
	
}
