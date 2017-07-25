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
        Data.Curr_RawPoints = MapCreator.CheckControlPointAngles(Data.Curr_RawPoints);
        Data.Curr_ControlPoints = MapCreator.CreateControlPoints(Data.Curr_RawPoints);
        Data.Curr_TrackPoints = MapCreator.CreateTrackPoints(Data.Curr_ControlPoints, Data.TrackPointFreq);

        //creates the objects from which to creat mesh
        MapCreator.MeshHelperObjects = new List<GameObject>();
        foreach (Vector2 pt in Data.Curr_TrackPoints)
        {
            GameObject point = Instantiate(MapCreator.PointObject);
            point.transform.position = pt;
            point.transform.parent = MapCreator.MeshHelperContainer.transform;
            MapCreator.MeshHelperObjects.Add(point);
        }
       
        MapCreator.RotateTrackObjectsAlongCurves(MapCreator.MeshHelperObjects);
        MapCreator.CreateTrackMesh(MapCreator.MeshHelperObjects);

        //populates current racing line with correct data
        Data.Curr_RacingLinePoints = MapCreator.CreateRacingLinePoints(Data.Curr_RawPoints, Data.RacingLineWaypointFreq, Data.RacingLineTightness);
        

        //creates a new AI opponent
        Instantiate(newAI,AIContainer.transform);
        
       InnerBarrier.CreateBarriers(Data.Curr_RawPoints, Data.BarrierShrinkFactor, Data.TireRadius, "Inner");
    }	
}
