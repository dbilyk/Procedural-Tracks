using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public Canvas Canvas;
    public MapCreator MapCreator;
    public BarrierCreator InnerBarrier;
    public GameObject AIContainer;
    public GameObject newAI;
    

    private Color32 red = new Color32(255,0,0,255);
    private Color32 green = new Color32(0, 255, 0,255);
    private Color32 blue = new Color32(0,0, 255, 255);

    public void StartNewGameButton()
    {
        Data.Curr_RawPoints = MapCreator.CreateRawUnsortedPoints();
        

        Data.Curr_RawPoints = MapCreator.SortPoints(Data.Curr_RawPoints);
        Data.Curr_RawPoints.DebugPlot(blue);

        Data.Curr_RawPoints = MapCreator.CheckControlPointAngles(Data.Curr_RawPoints, Data.CornerBroadeningLerpStep);
        Data.Curr_RawPoints.DebugPlot(green);
        //Data.Curr_RawPoints = MapCreator.ApplyRandomRotation(Data.Curr_RawPoints);
        Data.Curr_ControlPoints = MapCreator.CreateControlPoints(Data.Curr_RawPoints);
        Data.Curr_TrackPoints = MapCreator.CreateTrackPoints(Data.Curr_ControlPoints, Data.MeshTrackPointFreq);
        //mesh creation
        MapCreator.CreateOrSetMeshHelperObjects(Data.Curr_TrackPoints);
        MapCreator.RotateTrackObjectsAlongCurves(Data.CurrentMeshHelperObjects);
        MapCreator.CreateTrackMesh(Data.CurrentMeshHelperObjects);
        MapCreator.CreateColliderForTrack(Data.Curr_OuterTrackPoints, Data.Curr_InnerTrackPoints);

        //populates current racing line with correct data
        Data.Curr_RacingLinePoints = MapCreator.CreateRacingLinePoints(Data.Curr_RawPoints, Data.RacingLineWaypointFreq, Data.RacingLineTightness);
        
        //creates a new AI opponent
        Instantiate(newAI,AIContainer.transform);
       //InnerBarrier.CreateBarriers(Data.Curr_RawPoints, Data.BarrierShrinkFactor, Data.TireRadius, "Inner");
    }	
}
