using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CarPolePositionData
{
    public GameObject CarObject;
    public Vector2 NearestCheckpoint;
    public int CurrentPolePosition;
    public int CurrentLap;
    public int TotalCheckpointsPassedThisLap;
}

public class RaceStatsManager : MonoBehaviour {
    public MapCreator mapCreator;
    public AIInputController aIInputController;
    public GameObject AIContainer;
    public GameObject Player;
    public int CheckpointFreq =3;
    public bool RaceStarted;
    //opponets and lap map 1 to 1 to each other
    private List<Vector2> Checkpoints = new List<Vector2>();
    private List<AIPolePositionData> Opponents = new List<AIPolePositionData>();
    private int TotalCheckpointsOnMap;




	// Use this for initialization
	void Start () {
        Checkpoints = mapCreator.CreateTrackPoints(Data.Curr_ControlPoints, CheckpointFreq);
        TotalCheckpointsOnMap = Checkpoints.Count;
		for(int i = 0; i < AIContainer.transform.childCount; i++)
        {
            CarPolePositionData AIData = new CarPolePositionData();
            AIData.CarObject =AIContainer.transform.GetChild(i).gameObject;
            AIData.CurrentLap = 1;
            AIData.NearestCheckpoint = aIInputController.GetNearestWaypoint(AIData.CarObject.transform,Checkpoints);
            Opponents.Add(AIData);
        }


	}

    IEnumerator CheckPlayerPosition()
    {
        Vector2 nearestPlayerCheckpoint = aIInputController.GetNearestWaypoint(Player.transform,Checkpoints);
        for (int i = 0; i < Opponents.Count; i ++)
        {
            


            Vector2 nearestAICheckpoint = aIInputController.GetNearestWaypoint(Opponents[i].transform,Checkpoints);
            if (nearestAICheckpoint == nearestPlayerCheckpoint)
            {

            }

        }

        yield return new WaitForSeconds(0.2f);   
    }

    IEnumerator CheckAI
	
	// Update is called once per frame
	void Update () {
        if (!RaceStarted)
        {
            StartCoroutine("CheckPlayerPosition");
        }

	}
}
