using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public MapCreator MapCreator;
    public BarrierCreator InnerBarrier;
    public GameObject AIContainer;
    public GameObject newAI;


    //public BarrierCreator barrierCreator;

    public void StartNewGameButton()
    {
        

        Data.Curr_RawPoints = MapCreator.CreateRawUnsortedPoints();
        Data.Curr_RawPoints = MapCreator.SortPoints(Data.Curr_RawPoints);
        Data.Curr_RawPoints = MapCreator.CheckControlPointAngles(Data.Curr_RawPoints, Data.CornerBroadeningLerpStep);
        Data.Curr_ControlPoints = MapCreator.CreateControlPoints(Data.Curr_RawPoints);
        Data.Curr_TrackPoints = MapCreator.CreateTrackPoints(Data.Curr_ControlPoints, Data.MeshTrackPointFreq);
        Data.Curr_TrackPoints.DebugPlot(new Color32(0, 0, 0, 255));

        //mesh creation
        MapCreator.CreateOrSetMeshHelperObjects(Data.Curr_TrackPoints);
        MapCreator.RotateTrackObjectsAlongCurves(Data.CurrentMeshHelperObjects);
        MapCreator.CreateTrackMesh(Data.CurrentMeshHelperObjects);

        //populates current racing line with correct data
        Data.Curr_RacingLinePoints = MapCreator.CreateRacingLinePoints(Data.Curr_RawPoints, Data.RacingLineWaypointFreq, Data.RacingLineTightness);
        

        //creates a new AI opponent
        Instantiate(newAI,AIContainer.transform);
        
       //InnerBarrier.CreateBarriers(Data.Curr_RawPoints, Data.BarrierShrinkFactor, Data.TireRadius, "Inner");
    }	
}
