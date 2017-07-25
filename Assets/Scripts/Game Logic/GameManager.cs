using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public GameObject AIContainer;
    public GameObject newAI;

    public MapCreator MapCreator;


    //public BarrierCreator barrierCreator;

    public void StartNewGameButton()
    {
        MapCreator.MeshHelperEmpties = MapCreator.CreateTrackData();
        MapCreator.RotateTrackObjectsAlongCurves(MapCreator.MeshHelperEmpties);
        MapCreator.CreateTrackMesh(MapCreator.MeshHelperEmpties);


        Data.Curr_RacingLinePoints = MapCreator.CreateRacingLinePoints(Data.Curr_RawPoints, Data.RacingLineWaypointFreq, Data.RacingLineTightness);
        Instantiate(newAI,AIContainer.transform);
        
       // MapCreator.CreateBarriers(Data.Curr_RawPoints, Data.BarrierShrinkFactor, Data.TireRadius, Data.TireBarrier);
    }	
}
