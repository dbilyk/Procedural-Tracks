using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPolePositionData
{
    public GameObject CarObject;//constant
    public int IndexInDataArray;//constant
    public bool FacingForward;//check this on a slower schedule
    public bool AllowCheckpointUpdates;
    public int Curr_CheckpointIndex;//needs to be updated on schedule
    public int PrevCheckpointIndex {
        get {
            return PrevCheckpointIndex;
        }
        set {
            if(value == -1)
            {
                value = Data.Curr_PoleCheckpoints.Count - 1;
            }
        } }
    public int LastValidCheckpointIndex;//needs to be updated on schedule
    public int Curr_PolePosition;
    public int Curr_LapNumber;
    public float Curr_LapStartTime;
    public float Curr_LapTime;
    public Time FastestLapTime;
    public int TotalCheckpointsPassedThisLap;


}
/*
 if NOT  facingforward, dont enter into collect checkpoint logic
     */


public class RaceStatsManager : MonoBehaviour {
    public MapCreator mapCreator;
    public LapTrigger lapTrigger;
    public GameObject AIContainer;
    public GameObject Player;
    public float PctOfCheckpointsThatConstitutesALap = 90f;
    public int CheckpointFreq =1;
    public CarPolePositionData PlayerPoleData = new CarPolePositionData();

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
        PlayerPoleData.FacingForward = true;
        PlayerPoleData.IndexInDataArray = 0;
        PlayerPoleData.AllowCheckpointUpdates = true;
        CarsOnTrack.Add(PlayerPoleData);

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
            AIData.FacingForward = true;
            AIData.AllowCheckpointUpdates = true;

            CarsOnTrack.Add(AIData);

        }
        Data.CarPoleData = CarsOnTrack;

	}

    public void Start()
    {
        LapTrigger.OnLapComplete += LapComplete;
    }

    private bool UpdatePoleData = false;
    private bool CheckFacingForward = false;
    // Update is called once per frame
    void Update () {
        if (!UpdatePoleData && Data.Curr_RaceBegun)
        {
            CurrentPoleDataInit();
            StartCoroutine("RecalculatePoleData");
            UpdatePoleData = true;
        }
        if{

        }

	}

    IEnumerator CheckFacingForward()
    {
        List<CarPolePositionData> carData = Data.CarPoleData;
        List<Vector2> ChkPts = Data.Curr_PoleCheckpoints;

        if (Vector2.Dot(carData[0].CarObject.GetComponent<Rigidbody2D>().velocity, ChkPts[carData[0].Curr_CheckpointIndex] - ChkPts[carData[0].PrevCheckpointIndex]) > 0f)
        {
            carData[0].FacingForward = true;
        }
        else
        {
            carData[0].FacingForward = false;

        }
        yield return new WaitForSeconds(0.4f);
    }



    IEnumerator RecalculatePoleData()
    {
        //player is index 0
        List<CarPolePositionData> carData = Data.CarPoleData;
        List<Vector2> ChkPts = Data.Curr_PoleCheckpoints;
        //IF facing forward 
        
        if (carData[0].FacingForward)
        {
            //check that the most recent checkpoint is the same as the last valid checkpoint
            if (carData[0].Curr_CheckpointIndex == carData[0].LastValidCheckpointIndex)
            {
                carData[0].AllowCheckpointUpdates = true;
            }
            
            else
            {
                //update current nearest checkpoint
                carData[0].Curr_CheckpointIndex = ExtensionMethods.GetNearestInList(carData[0].CarObject,Data.Curr_PoleCheckpoints);
            }
        }

        else
        {
            return;
        }

        if (carData[0].AllowCheckpointUpdates)
        {
            carData[0].Curr_CheckpointIndex = ExtensionMethods.GetNearestInList(carData[0].CarObject, Data.Curr_PoleCheckpoints);
            carData[0].PrevCheckpointIndex = 

        }

        for (int i = 1; i < carData.Count; i++)
        {
            //insert logic for updating all checkpts, and pole positions
            

        }

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
