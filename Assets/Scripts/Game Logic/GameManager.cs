using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    //vars
    public float CountdownLength = 2;

    //script objects
    public BarrierCreator InnerBarrier;
    public BarrierCreator OuterBarrier;
    public MapCreator MapCreator;
    public SmoothFollowCam FollowCam;

    //game objects
    public GameObject RaceStatsManager;
    public GameObject Player;
    public GameObject newAI;
    public GameObject ActiveGameTrack;
    public GameObject BermDecals;
    public GameObject chickenTest;

    public GameObject AIContainer;
    public GameObject StartingGridContainer;
    public GameObject FoliageContainer;

    public GameObject GameLoopUI;
    public GameObject MiniMapGroup;
    public GameObject StartingLights;

    public List<AIInputController> AIInputs = new List<AIInputController>();
    

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    //small helper to toggle AI input
    void SetAIInput(bool isActive)
    {
        for (int i = 0; i < AIInputs.Count; i++)
        {
            AIInputs[i].enabled = isActive;
        }
    }


    void GenerateNewTrackData()
    {
        Data.Curr_RawPoints = new List<Vector2>();

        Data.Curr_RawPoints = MapCreator.CreateRawUnsortedPoints();
        Data.Curr_RawPoints = MapCreator.SortPoints(Data.Curr_RawPoints);
        //have to run point thinning and angle adjustment several times because they recursively affect each other.
        for (int i = 0; i < 50; i++)
        {
            Data.Curr_RawPoints = MapCreator.RemovePointsTooClose(Data.Curr_RawPoints, Data.PointSpacing);
            Data.Curr_RawPoints = MapCreator.CheckControlPointAngles(Data.Curr_RawPoints, Data.CornerBroadeningLerpStep);
        }

        //Data.Curr_RawPoints = MapCreator.ApplyRandomRotation(Data.Curr_RawPoints);
        Data.Curr_ControlPoints = MapCreator.CreateControlPoints(Data.Curr_RawPoints);
        Data.Curr_TrackPoints = MapCreator.CreateTrackPoints(Data.Curr_ControlPoints, Data.MeshTrackPointFreq);
    }

    void GenerateLevel()
    {

       
        //mesh creation
        MapCreator.CreateOrSetMeshHelperObjects(Data.Curr_TrackPoints);
        MapCreator.RotateTrackObjectsAlongCurves(Data.CurrentMeshHelperObjects);

        for(int i = 0; i < Data.CurrentMeshHelperObjects.Count; i+=4)
        {
            GameObject chicken = Instantiate(chickenTest);
            chickenTest.transform.position = Data.CurrentMeshHelperObjects[i].transform.position;
            chickenTest.transform.rotation = Data.CurrentMeshHelperObjects[i].transform.rotation;

        }


        MapCreator.CreateStartingGrid(Data.CurrentMeshHelperObjects, Data.StartingGridLength, Data.StartingGridWidth, Data.NumberOfGridPositions);

        MapCreator.CreateTrackBerms(Data.CurrentMeshHelperObjects, Data.BermWidth, Data.BermOffset, Data.BermLength, BermDecals.GetComponent<MeshFilter>());

        MapCreator.CreateTrackMesh(Data.CurrentMeshHelperObjects, Data.TrackMeshThickness, ActiveGameTrack.gameObject.GetComponent<MeshFilter>());

        MapCreator.CreateColliderForTrack(Data.Curr_OuterTrackPoints, Data.Curr_InnerTrackPoints, Data.TrackColliderResolution, ActiveGameTrack.GetComponent<PolygonCollider2D>());

        //populates current racing line with correct data
        Data.Curr_RacingLinePoints = MapCreator.CreateRacingLinePoints(Data.Curr_RawPoints, Data.RacingLineWaypointFreq, Data.RacingLineTightness);

        //create barriers
        Data.InnerBarrierPoints = InnerBarrier.CreateOutline(Data.Curr_RawPoints, Data.InnerBarrierOffset, "inner");
        Data.OuterBarrierPoints = OuterBarrier.CreateOutline(Data.Curr_RawPoints, Data.OuterBarrierOffset, "outer");
        InnerBarrier.CreateBarrier(Data.InnerBarrierPoints);
        OuterBarrier.CreateBarrier(Data.OuterBarrierPoints);
        
        FoliageContainer.SetActive(true);

        StaticBatchingUtility.Combine(FoliageContainer);
    }

    private void GenerateAI()
    {
        //creates a new AI opponent
        for (int i = 0; i < Data.CarStartingPositions.Count - 1; i++)
        {
            GameObject Ai = Instantiate(newAI, AIContainer.transform);

            Ai.transform.position = Data.CarStartingPositions[i].transform.position;
            Ai.transform.rotation = Data.CarStartingPositions[i].transform.rotation;
            AIInputController aiInput = Ai.GetComponent<AIInputController>();
            aiInput.enabled = false;
            AIInputs.Add(aiInput);
        }
    }

    private void StartingCountdown()
    {
        //positions player/AIs
        Player.transform.position = Data.CarStartingPositions[Data.CarStartingPositions.Count - 1].transform.position;
        Player.transform.rotation = Data.CarStartingPositions[Data.CarStartingPositions.Count - 1].transform.rotation;
        Player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Player.GetComponent<CarMovement>().enabled = false;
        //disable UI before start of race
        GameLoopUI.SetActive(false);

        StartCoroutine("StartRace");
        Vector3 CamStartPosition = new Vector3(Player.transform.position.x -5, Player.transform.position.y,-3);
        Quaternion CamStartRotation = Quaternion.Euler(0,100,0);
        FollowCam.gameObject.transform.position = CamStartPosition;
        FollowCam.gameObject.transform.rotation = CamStartRotation;
        InvokeRepeating("StartingCam",0,0.02f);
    }

    void StartingCam()
    {
        GameObject cam = FollowCam.gameObject;
            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(Player.transform.position.x, Player.transform.position.y, -18), 0.05f);
            cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, Quaternion.Euler(Player.transform.rotation.eulerAngles.x, Player.transform.rotation.eulerAngles.y, Player.transform.rotation.eulerAngles.z - 90), 0.05f);
    }

    IEnumerator StartRace()
    {
        StartingLights.SetActive(true);
        yield return new WaitForSeconds(CountdownLength);
        CancelInvoke("StartingCam");
        FollowCam.enabled = true;

        //must activate before GameloopUI
        RaceStatsManager.SetActive(true);
        
        //Enable gameloop UI
        GameLoopUI.SetActive(true);
        MiniMapGroup.SetActive(true);

        //enables AI input
        SetAIInput(true);
        //enables player movement
        Player.GetComponent<CarMovement>().enabled = true;

    }

    //destroys stuff that gets recreated in StartNewGame
    void ResetGame()
    {
        StopCoroutine("StartRace");
        StartingLights.SetActive(false);
        SetAIInput(false);
        //setting these false will re-trigger Initialization in their respective OnEnable functions
        GameLoopUI.SetActive(false);
        RaceStatsManager.SetActive(false);

        //places AI back on starting grid
        for (int i =0; i < AIContainer.transform.childCount; i ++)
        {
            GameObject AI = AIContainer.transform.GetChild(i).gameObject;

            AI.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            AI.transform.position = StartingGridContainer.transform.GetChild(i).transform.position;
            AI.transform.rotation = StartingGridContainer.transform.GetChild(i).transform.rotation;
        }

    }


    //
    public void StartRaceButton()
    {
        ResetGame();
        GenerateNewTrackData();
        GenerateLevel();
        GenerateAI();
        StartingCountdown();
        StartRace();
    }

    public void RestartLevelButton()
    {
        ResetGame();
        StartingCountdown();
    }
}
