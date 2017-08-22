using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    //script objects
    public BarrierCreator InnerBarrier;
    public BarrierCreator OuterBarrier;
    public MapCreator MapCreator;
    public MiniMap MiniMapScript;
    //game objects
    public Canvas Canvas;
    public GameObject RaceStatsManager;
    public GameObject Player;
    public GameObject ActiveGameTrack;
    public GameObject BermDecals;

    public GameObject AIContainer;
    public GameObject StartingGridContainer;
    public GameObject FoliageContainer;

    public GameObject newAI;
    public GameObject MiniMapGroup;

    private void StartNewGame()
    {
        Data.Curr_RawPoints = new List<Vector2>();

        Data.Curr_RawPoints = MapCreator.CreateRawUnsortedPoints();
        Data.Curr_RawPoints = MapCreator.SortPoints(Data.Curr_RawPoints);
        //have to run point thinning and angle adjustment several times because they recursively affect each other.
        for(int i = 0; i < 50; i++)
        {
            Data.Curr_RawPoints = MapCreator.RemovePointsTooClose(Data.Curr_RawPoints, Data.PointSpacing);
            Data.Curr_RawPoints = MapCreator.CheckControlPointAngles(Data.Curr_RawPoints, Data.CornerBroadeningLerpStep);
        }

        //Data.Curr_RawPoints = MapCreator.ApplyRandomRotation(Data.Curr_RawPoints);
        Data.Curr_ControlPoints = MapCreator.CreateControlPoints(Data.Curr_RawPoints);
        Data.Curr_TrackPoints = MapCreator.CreateTrackPoints(Data.Curr_ControlPoints, Data.MeshTrackPointFreq);
        //mesh creation
        MapCreator.CreateOrSetMeshHelperObjects(Data.Curr_TrackPoints);
        MapCreator.RotateTrackObjectsAlongCurves(Data.CurrentMeshHelperObjects);
        MapCreator.CreateStartingGrid(Data.CurrentMeshHelperObjects,Data.StartingGridLength, Data.StartingGridWidth, Data.NumberOfGridPositions);

        MapCreator.CreateTrackBerms(Data.CurrentMeshHelperObjects,Data.BermWidth,Data.BermOffset,Data.BermLength,BermDecals.GetComponent<MeshFilter>());

        MapCreator.CreateTrackMesh(Data.CurrentMeshHelperObjects, Data.TrackMeshThickness, ActiveGameTrack.gameObject.GetComponent<MeshFilter>());
        MapCreator.CreateColliderForTrack(Data.Curr_OuterTrackPoints, Data.Curr_InnerTrackPoints, Data.TrackColliderResolution, ActiveGameTrack.GetComponent<PolygonCollider2D>());

        //populates current racing line with correct data
        Data.Curr_RacingLinePoints = MapCreator.CreateRacingLinePoints(Data.Curr_RawPoints, Data.RacingLineWaypointFreq, Data.RacingLineTightness);
       
        //enable minimap
        MiniMapGroup.SetActive(true);
        MiniMapScript.CreateMinimap(Data.Curr_ControlPoints);

        //create barriers
        Data.InnerBarrierPoints = InnerBarrier.CreateOutline(Data.Curr_RawPoints, Data.InnerBarrierOffset, "inner");
        Data.OuterBarrierPoints = OuterBarrier.CreateOutline(Data.Curr_RawPoints, Data.OuterBarrierOffset, "outer");
        InnerBarrier.CreateBarrier(Data.InnerBarrierPoints);
        OuterBarrier.CreateBarrier(Data.OuterBarrierPoints);

        FoliageContainer.SetActive(true);

        //positions player/AIs
        Player.transform.position = Data.CarStartingPositions[Data.CarStartingPositions.Count-1].transform.position;
        Player.transform.rotation = Data.CarStartingPositions[Data.CarStartingPositions.Count - 1].transform.rotation;

        //creates a new AI opponent
        for(int i = 0; i < Data.CarStartingPositions.Count-1; i++)
        {
            GameObject Ai = Instantiate(newAI, AIContainer.transform);

            Ai.transform.position = Data.CarStartingPositions[i].transform.position;
            Ai.transform.rotation = Data.CarStartingPositions[i].transform.rotation;

        }
        RaceStatsManager.SetActive(true);
        Data.Curr_RaceBegun = true;
    }

    //destroys stuff that gets recreated in StartNewGame
    void ResetGame()
    {
        RaceStatsManager.SetActive(false);
        for(int i = 0; i < StartingGridContainer.transform.childCount; i++)
        {
            Destroy(StartingGridContainer.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i< AIContainer.transform.childCount; i++)
        {
            Destroy(AIContainer.transform.GetChild(i).gameObject);
        }
        FoliageContainer.SetActive(false);
    }


    public void StartNewGameButton()
    {
        ResetGame();
        StartNewGame();
    }	
}
