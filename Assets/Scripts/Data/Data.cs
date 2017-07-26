using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour {
    //gerenal map settings
    public static float MapWidth = 60;//done
    public static float MapHeight = 60;//done
    
    //track creation settings
    public static float MinCornerWidth = 60;
    //lerp step that is applied while trying to reach the minimum corner angle
    public static float CornerBroadeningLerpStep = 0.1f;
    public static int PtCtPerQuad =3;
    public static float PointSpacing = 4.5f;

    //track mesh and collider settings
    public static List<GameObject> CurrentMeshHelperObjects;
    public static List<Vector2> Curr_OuterTrackPoints = new List<Vector2>();
    public static List<Vector2> Curr_InnerTrackPoints = new List<Vector2>();

    //to use for saved games
    public static List<Vector3> Curr_Verts = new List<Vector3>();
    public static List<Vector3> Curr_Normals = new List<Vector3>();
    public static List<Vector2> Curr_UVs = new List<Vector2>();
    public static List<int> Curr_Indicies = new List<int>();


    public static float MeshTrackPointFreq = 30;
    public static float TrackMeshThickness = 1.2f;

    public static int TrackColliderResolution = 13;


    //barrier data
    public static float TireRadius = 0.1f;
    public static float BarrierShrinkFactor = 2;
    public static float BarrierCornerKinkFactor = 2;


    //CURRENT game state MAP data
    public static int Curr_TrackRotation;
    public static List<Vector2> Curr_RawPoints;
    public static List<Vector2> Curr_ControlPoints;
    public static List<Vector2> Curr_TrackPoints;

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
