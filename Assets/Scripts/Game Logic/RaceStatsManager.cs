using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPolePositionData
{
    public GameObject CarObject;
    public int Curr_CheckpointIndex { get; set;}
    public int PrevCheckpointIndex { get; set; }
    public int LastValidCheckpointIndex { get; set; }
    public int Curr_PolePosition { get; set; }
    public int Curr_Lap { get; set; }
    public Time Curr_LapTime { get; set; }
    public Time FastestLapTime { get; set; }
    public int TotalCheckpointsPassedThisLap { get; set; }


}

public class RaceStatsManager : MonoBehaviour {
    public MapCreator mapCreator;
    public AIInputController aIInputController;
    public GameObject AIContainer;
    public GameObject Player;
    public int CheckpointFreq =1;
    public bool RaceStarted;
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
        PlayerPoleData.Curr_Lap = 1;
        PlayerPoleData.TotalCheckpointsPassedThisLap = 1;
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
            AIData.Curr_Lap = 1;
            AIData.TotalCheckpointsPassedThisLap = 1;

            CarsOnTrack.Add(AIData);
        }
        Data.CarPoleData = CarsOnTrack;

	}

    //IEnumerator CheckPosition()
    //{

    //    Vector2 nearestPlayerCheckpoint = aIInputController.GetNearestWaypoint(Player.transform,Checkpoints);
    //    for (int i = 0; i < Opponents.Count; i ++)
    //    {
            


    //        Vector2 nearestAICheckpoint = aIInputController.GetNearestWaypoint(Opponents[i].transform,Checkpoints);
    //        if (nearestAICheckpoint == nearestPlayerCheckpoint)
    //        {

    //        }

    //    }

    //    yield return new WaitForSeconds(0.2f);   
    //}

	
	// Update is called once per frame
	void Update () {
        if (Data.Curr_RaceBegun && !RaceStarted)
        {
            CurrentPoleDataInit();
            //StartCoroutine("CheckPlayerPosition");
            RaceStarted = true;
        }

	}
}
