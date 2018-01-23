using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapTrigger : MonoBehaviour {
    public delegate void LapCompleteDel (int CarIndex);
    public static event LapCompleteDel OnLapComplete;

    [SerializeField]
    RaceStatsManager statsManager;

    [SerializeField]
    StartingLight startingLight;

    bool raceStarted = false;

    void Start () {
        startingLight.OnStartingLightsComplete += setRaceStarted;
    }

    void setRaceStarted () {
        raceStarted = true;
    }

    void OnTriggerEnter2D (Collider2D col) {
        if (raceStarted) {
            for (int i = 0; i < statsManager.CarPoleData.Count; i++) {
                if (col.gameObject == statsManager.CarPoleData[i].CarObject) {
                    OnLapComplete (i);
                    break;
                }
            }
        }
    }
}