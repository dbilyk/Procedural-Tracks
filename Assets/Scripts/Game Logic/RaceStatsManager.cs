using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPolePositionData
{
    public GameObject CarObject;
    public int IndexInDataArray;
    public int Curr_CheckpointIndex;
    public int PrevCheckpointIndex;
    public int LastValidCheckpointIndex;
    public int Curr_PolePosition;
    public int Curr_LapNumber;
    public float Curr_LapStartTime;
    public float Curr_LapTime;
    public Time FastestLapTime;
    public int TotalCheckpointsPassedThisLap;


}

public class RaceStatsManager : MonoBehaviour {
    public MapCreator mapCreator;
    public LapTrigger lapTrigger;
    public GameObject AIContainer;
    public GameObject Player;
    public float PctOfCheckpointsThatConstitutesALap = 90f;
    public int CheckpointFreq =1;
    public CarPolePositionData PlayerPoleData = new CarPolePositionData();

    private bool UpdatePoleData = false;
    private List<Vector2> Checkpoints = new List<Vector2>();
    private List<CarPolePositionData> CarsOnTrack = new List<CarPolePositionData>();
    private int TotalCheckpointsOnMap;

    //call at the start of a race ot populate currentPoleData list
    void CurrentPoleDataInit() {
        Checkpoints = mapCreator.CreateTrackPoints(Data.Curr_ControlPoints, CheckpointFreq);
        Data.Curr_PoleCheckpoints = Checkpoints;
        TotalCheckpointsOnMap = Checkpoints.Count;

        //populate player struct and push to list
        PlayerPoleData.CarObject = Player.gameObject;
        int playerNearestWPindex = ExtensionMethods.GetNearestInList(Player.transform.position, Checkpoints);
        PlayerPoleData.Curr_CheckpointIndex = playerNearestWPindex;
        //make sure we get both the current closest and WP before this one so that we can use that vector to make sure the player is driving the right way.
        if (playerNearestWPindex ==0)
        {
            PlayerPoleData.PrevCheckpointIndex = Checkpoints.Count - 1;
        }
        else
        {
            PlayerPoleData.PrevCheckpointIndex = playerNearestWPindex - 1; 
        }
        PlayerPoleData.LastValidCheckpointIndex = PlayerPoleData.Curr_CheckpointIndex;
        PlayerPoleData.Curr_LapNumber = 0;
        PlayerPoleData.TotalCheckpointsPassedThisLap = 0;
        CarsOnTrack.Add(PlayerPoleData);
        PlayerPoleData.IndexInDataArray = 0;

        //AI struct creation
		for(int i = 0; i < AIContainer.transform.childCount; i++)
        {
            CarPolePositionData AIData = new CarPolePositionData();
            AIData.CarObject =AIContainer.transform.GetChild(i).gameObject;
            
            AIData.Curr_CheckpointIndex = ExtensionMethods.GetNearestInList(AIData.CarObject.transform.position,Checkpoints);
            if (AIData.Curr_CheckpointIndex == 0)
            {
               AIData.PrevCheckpointIndex = Checkpoints.Count - 1;
            }
            else
            {
                AIData.PrevCheckpointIndex = AIData.Curr_CheckpointIndex - 1;
            }
            AIData.LastValidCheckpointIndex = AIData.Curr_CheckpointIndex;
            AIData.Curr_LapNumber = 0;
            AIData.TotalCheckpointsPassedThisLap = 1;
            AIData.IndexInDataArray = i+1;
            CarsOnTrack.Add(AIData);
        }
        Data.CarPoleData = CarsOnTrack;

	}

    public void Start()
    {
        LapTrigger.OnLapComplete += LapComplete;
    }

    // Update is called once per frame
    void Update () {
        if (Data.Curr_RaceBegun && !UpdatePoleData)
        {
            CurrentPoleDataInit();
            StartCoroutine("RecalculatePoleData");
            UpdatePoleData = true;
        }

	}

    IEnumerator RecalculatePoleData()
    {
        Debug.Log("heythere");

        yield return new WaitForSeconds(0.2f);
        UpdatePoleData = false;
    }

    

    //this delegate subscriber is called each time a car passes the lap line.
    void LapComplete(int PoleDataIndex)
    {
        CarPolePositionData thisCar = Data.CarPoleData[PoleDataIndex];

        if (thisCar.Curr_LapNumber == 0)
        {
            thisCar.Curr_LapNumber += 1;
            thisCar.Curr_LapStartTime = Time.time;
            Debug.Log(Time.time);
        }

        if (thisCar.TotalCheckpointsPassedThisLap >= Data.Curr_PoleCheckpoints.Count * (PctOfCheckpointsThatConstitutesALap / 100))
        {
            thisCar.Curr_LapNumber += 1;
            thisCar.Curr_LapStartTime = Time.time;
            Debug.Log("time:" + Time.time + "  Lap#:" + thisCar.Curr_LapNumber);

        }

    }
}
