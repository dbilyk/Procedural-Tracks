using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceStatsUIUpdate : MonoBehaviour {
    public Text CurrentLapTimeBig;
    public Text CurrentLapTimeSmall;

    public Text LastLapTimeBig;
    public Text LastLapTimeSmall;

    public Text BestLapTimeBig;
    public Text BestLapTimeSmall;

    public Text CurrentPlace;
    public Text TotalPlaces;

    public Text CurrentLap;
    public Text TotalLaps;
    
    public float IndentCurrentTime;
    public float IndentBestAndLastTime;

    public Text LapCompleteAlert;
    public RaceStatsManager raceStatsManager;
    private CarPolePositionData PlayerRaceStats;

    bool UpdateUI = false;
    void OnEnable()
    {
        raceStatsManager.OnLapAlert += TriggerLapAlert;
        UpdateUI = false;
        PlayerRaceStats = Data.CarPoleData[0];

    } 

    void TriggerLapAlert(int newLapNumber)
    {
        Debug.Log("Trigger Alert");
        LapCompleteAlert.gameObject.SetActive(true);
        if (newLapNumber == Data.Curr_NumberOfLapsInRace)
        {
            LapCompleteAlert.text = "Final Lap";

        }
        else
        {
            LapCompleteAlert.text = "Lap " + newLapNumber.ToString();
        }
    }

    void Update () {
        if (!UpdateUI)
        {
            StartCoroutine("UpdateUIStats");
            UpdateUI = true;
        }

    
    }
    IEnumerator UpdateUIStats()
    {
        //populate current time
        string[] CurrLapTime = PlayerRaceStats.Curr_LapTime.ToString("F2").Split('.');
        CurrentLapTimeBig.text = CurrLapTime[0];
        CurrentLapTimeSmall.text = "." +CurrLapTime[1];

        //populate best and last times
        if (PlayerRaceStats.Curr_LapNumber <2)
        {
            LastLapTimeBig.text = "";
            LastLapTimeSmall.text = "";
            BestLapTimeBig.text = "";
            BestLapTimeSmall.text = "";
        }
        else
        {
            string[] LastLapTime = PlayerRaceStats.LastLapTime.ToString("F2").Split('.');
            LastLapTimeBig.text = LastLapTime[0];
            LastLapTimeSmall.text = "." + LastLapTime[1];

            string[] BestLapTime = PlayerRaceStats.FastestLapTime.ToString("F2").Split('.');
            BestLapTimeBig.text = BestLapTime[0];
            BestLapTimeSmall.text = "." + BestLapTime[1];
        }
        //populate Pole position
        CurrentPlace.text = PlayerRaceStats.Curr_PolePosition.ToString();
        TotalPlaces.text = Data.CarPoleData.Count.ToString();

        //populate current lap
        if (PlayerRaceStats.Curr_LapNumber <1)
        {
            CurrentLap.text = "1";
        }
        else
        {
            CurrentLap.text = PlayerRaceStats.Curr_LapNumber.ToString();
        }
        TotalLaps.text = Data.Curr_NumberOfLapsInRace.ToString();

        yield return new WaitForSeconds(0.05f);
        UpdateUI = false;
    }
}
