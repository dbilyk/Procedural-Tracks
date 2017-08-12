using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPolePositionData
{
    private int _prevChkPtInd;
    public GameObject CarObject;//constant
    public int IndexInDataArray;//constant
    public bool FacingForward;//check this on a slower schedule
    public bool AllowCheckpointUpdates;
    public int Curr_CheckpointIndex;//needs to be updated on schedule
    public int PrevCheckpointIndex {
        get
        {
            if (this.Curr_CheckpointIndex == 0)
            {
                _prevChkPtInd = Data.Curr_PoleCheckpoints.Count - 1;
                
            }
            else
            {
                _prevChkPtInd = this.Curr_CheckpointIndex - 1;
            }
            return _prevChkPtInd;
        }
         }
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
    private bool initComplete = false;
    void Update () {
        if (!initComplete && Data.Curr_RaceBegun)
        {
            CurrentPoleDataInit();
            initComplete = true;
        }

        //perioodically checking if player is facing forward
        if (!CheckFacingForward && Data.Curr_RaceBegun)
        {
            StartCoroutine("GetFacingForward");
            CheckFacingForward = true;
        }
        //periodically recalculating player checkpoints
        if (!UpdatePoleData && Data.Curr_RaceBegun)
        {
            StartCoroutine("RecalculatePoleData");
            UpdatePoleData = true;
            Debug.Log("Current Chkpt: " + Data.CarPoleData[0].Curr_CheckpointIndex + "chkpts passed: " + Data.CarPoleData[0].LastValidCheckpointIndex);

        }

    }

    IEnumerator GetFacingForward()
    {
        CarPolePositionData player = Data.CarPoleData[0];
        List<Vector2> ChkPts = Data.Curr_PoleCheckpoints;

        if (Vector2.Dot(player.CarObject.GetComponent<Rigidbody2D>().velocity, ChkPts[player.Curr_CheckpointIndex] - ChkPts[player.PrevCheckpointIndex]) > 0f)
        {
            player.FacingForward = true;
        }
        else
        {
            player.FacingForward = false;
            player.AllowCheckpointUpdates = false;

        }
        yield return new WaitForSeconds(0.1f);
        CheckFacingForward = false;
    }

    IEnumerator RecalculatePoleData()
    {
        //player is index 0
        List<CarPolePositionData> carData = Data.CarPoleData;
        CarPolePositionData player = carData[0];
        List<Vector2> ChkPts = Data.Curr_PoleCheckpoints;
        //IF facing forward 
        
        if (!player.AllowCheckpointUpdates)
        {
            //check that the most recent checkpoint is the same as the last valid checkpoint
            if (player.Curr_CheckpointIndex == player.LastValidCheckpointIndex && player.FacingForward)
            {
                player.AllowCheckpointUpdates = true;
            }
            
            else
            {
                //update current nearest checkpoint
                player.Curr_CheckpointIndex = ExtensionMethods.GetNearestInList(player.CarObject.transform.position,Data.Curr_PoleCheckpoints);
            }
        }

        if (player.AllowCheckpointUpdates)
        {
            player.Curr_CheckpointIndex = ExtensionMethods.GetNearestInList(player.CarObject.transform.position, Data.Curr_PoleCheckpoints);
            if (player.Curr_CheckpointIndex != player.LastValidCheckpointIndex)
            {
                player.TotalCheckpointsPassedThisLap += 1;
            }

            //player.PrevCheckpointIndex = player.Curr_CheckpointIndex -1;
            player.LastValidCheckpointIndex = player.Curr_CheckpointIndex;
            

        }

        yield return new WaitForSeconds(0.1f);
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
            thisCar.TotalCheckpointsPassedThisLap = 0;
            thisCar.Curr_LapNumber += 1;
            thisCar.Curr_LapStartTime = Time.time;
            Debug.Log("time:" + Time.time + "  Lap#:" + thisCar.Curr_LapNumber);

        }

    }
}
