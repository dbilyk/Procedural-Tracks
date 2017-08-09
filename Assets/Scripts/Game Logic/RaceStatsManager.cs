using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceStatsManager : MonoBehaviour {
    public MapCreator mapCreator;
    public AIInputController aIInputController;
    public GameObject AIContainer;
    public GameObject Player;
    public int CheckpointFreq =3;
    public bool RaceStarted;
    //opponets and lap map 1 to 1 to each other
    private List<GameObject> Opponents = new List<GameObject>();
    private List<int> CurrentAILap = new List<int>();
    private List<Vector2> Checkpoints = new List<Vector2>();

	// Use this for initialization
	void Start () {
		for(int i = 0; i < AIContainer.transform.childCount; i++)
        {
            Opponents.Add(AIContainer.transform.GetChild(i).gameObject);
        }

        Checkpoints = mapCreator.CreateTrackPoints(Data.Curr_ControlPoints, CheckpointFreq);

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


	
	// Update is called once per frame
	void Update () {
        if (!RaceStarted)
        {
            StartCoroutine("CheckPlayerPosition");
        }

	}
}
