using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour {
    //gerenal map settings
    public static float MapWidth = 60;//done
    public static float MapHeight = 60;//done
    
    //track creation settings
    public static float MinCornerWidth = 50;
    public static int PtCtPerQuad =19;
    public static float PointSpacing = 6;
    public static float TrackPointFreq = 40;

    //track mesh and collider settings
    public static float TrackMeshThickness = -0.1f;
    public static int TrackColliderResolution = 24;


    //barrier data
    public static float TireRadius = 0.1f;
    public static float BarrierShrinkFactor = 2;
    public static float BarrierCornerKinkFactor = 2;


    //CURRENT game state MAP data
    public static List<Vector2> Curr_RawPoints;
    public static List<Vector2> Curr_ControlPoints;
    public static List<Vector2> Curr_TrackPoints;
    public static List<GameObject> Curr_TrackObjects;

    //racing line settings + data
    public static float RacingLineTightness = 0.15f;
    public static float RacingLineWaypointFreq = 8;
    public static List<Vector2> Curr_RacingLinePoints;

    //saved game data
    public static List<SavedTrack> Svd_Tracks;

    

}


//data structure for saving a track
public struct SavedTrack 
{
    public List<Vector2> ControlPoints;
    public string name;
    public int id;
    
}
