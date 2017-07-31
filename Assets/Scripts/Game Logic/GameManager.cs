﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public Canvas Canvas;
    public GameObject Player;
    public MapCreator MapCreator;
    public GameObject ActiveGameTrack;
    public BarrierCreator InnerBarrier;
    public BarrierCreator OuterBarrier;
    public GameObject AIContainer;
    public GameObject newAI;
    public GameObject MiniMapGroup;
    public MiniMap MiniMapScript;

    private void StartNewGame()
    {
        Data.Curr_RawPoints = new List<Vector2>();

        Data.Curr_RawPoints = MapCreator.CreateRawUnsortedPoints();
        Data.Curr_RawPoints = MapCreator.SortPoints(Data.Curr_RawPoints);
        //have to run point thinning and angle adjustment several times because they recursively affect each other.
        for(int i = 0; i < 20; i++)
        {
            Data.Curr_RawPoints = MapCreator.RemovePointsTooClose(Data.Curr_RawPoints, Data.PointSpacing);
            Data.Curr_RawPoints = MapCreator.CheckControlPointAngles(Data.Curr_RawPoints, Data.CornerBroadeningLerpStep);
        }

        Data.Curr_RawPoints = MapCreator.ApplyRandomRotation(Data.Curr_RawPoints);
        Data.Curr_ControlPoints = MapCreator.CreateControlPoints(Data.Curr_RawPoints);
        Data.Curr_TrackPoints = MapCreator.CreateTrackPoints(Data.Curr_ControlPoints, Data.MeshTrackPointFreq);
        //mesh creation
        MapCreator.CreateOrSetMeshHelperObjects(Data.Curr_TrackPoints);
        MapCreator.RotateTrackObjectsAlongCurves(Data.CurrentMeshHelperObjects);
        MapCreator.CreateStartingGrid(Data.CurrentMeshHelperObjects,Data.StartingGridLength, Data.StartingGridWidth, Data.NumberOfGridPositions);
       
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

        //positions player/AIs
        Player.transform.position = Data.CarStartingPositions[0].transform.position;
        Player.transform.rotation = Data.CarStartingPositions[0].transform.rotation;

        //creates a new AI opponent
        for(int i = 1; i < Data.CarStartingPositions.Count; i++)
        {
            GameObject Ai = Instantiate(newAI, AIContainer.transform);

            Ai.transform.position = Data.CarStartingPositions[i].transform.position;
            Ai.transform.rotation = Data.CarStartingPositions[i].transform.rotation;

        }

    }


    public void StartNewGameButton()
    {
        StartNewGame();
    }	
}
