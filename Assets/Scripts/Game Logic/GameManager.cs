using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public Canvas Canvas;
    public MapCreator MapCreator;
    public GameObject ActiveGameTrack;
    public BarrierCreator InnerBarrier;
    public GameObject AIContainer;
    public GameObject newAI;
    public GameObject MiniMapGroup;
    
    public void StartNewGameButton()
    {
        Data.Curr_RawPoints = MapCreator.CreateRawUnsortedPoints();
        Data.Curr_RawPoints = MapCreator.SortPoints(Data.Curr_RawPoints);
        //have to run point thinning and angle adjustment several times because they recursively affect each other.
        Data.Curr_RawPoints = MapCreator.RemovePointsTooClose(Data.Curr_RawPoints, Data.PointSpacing);
        Data.Curr_RawPoints = MapCreator.CheckControlPointAngles(Data.Curr_RawPoints, Data.CornerBroadeningLerpStep);
        Data.Curr_RawPoints = MapCreator.RemovePointsTooClose(Data.Curr_RawPoints, Data.PointSpacing);
        Data.Curr_RawPoints = MapCreator.CheckControlPointAngles(Data.Curr_RawPoints, Data.CornerBroadeningLerpStep);
        Data.Curr_RawPoints = MapCreator.RemovePointsTooClose(Data.Curr_RawPoints, Data.PointSpacing);
        Data.Curr_RawPoints = MapCreator.CheckControlPointAngles(Data.Curr_RawPoints, Data.CornerBroadeningLerpStep);

        //Data.Curr_RawPoints = MapCreator.ApplyRandomRotation(Data.Curr_RawPoints);
        Data.Curr_ControlPoints = MapCreator.CreateControlPoints(Data.Curr_RawPoints);
        Data.Curr_TrackPoints = MapCreator.CreateTrackPoints(Data.Curr_ControlPoints, Data.MeshTrackPointFreq);
        //mesh creation
        MapCreator.CreateOrSetMeshHelperObjects(Data.Curr_TrackPoints);
        MapCreator.RotateTrackObjectsAlongCurves(Data.CurrentMeshHelperObjects);
        MapCreator.CreateTrackMesh(Data.CurrentMeshHelperObjects, Data.TrackMeshThickness, ActiveGameTrack.gameObject.GetComponent<MeshFilter>());
        MapCreator.CreateColliderForTrack(Data.Curr_OuterTrackPoints, Data.Curr_InnerTrackPoints, Data.TrackColliderResolution, ActiveGameTrack.GetComponent<PolygonCollider2D>());

        //populates current racing line with correct data
        Data.Curr_RacingLinePoints = MapCreator.CreateRacingLinePoints(Data.Curr_RawPoints, Data.RacingLineWaypointFreq, Data.RacingLineTightness);
        
        //creates a new AI opponent
        Instantiate(newAI,AIContainer.transform);
        MiniMapGroup.SetActive(true);
       InnerBarrier.CreateBarriers(Data.Curr_RawPoints,Data.InnerExpansionMultiplier,"inner");
        
    }	
}
