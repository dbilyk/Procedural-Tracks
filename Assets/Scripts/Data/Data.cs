﻿using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour {
    public static Color32 red= new Color32(255,0,0,255);
    public static Color32 green= new Color32(0, 255,0, 255);
    public static Color32 blue= new Color32(0,0, 255, 255);
    public static Color32 yellow= new Color32(255, 255,0, 255);
    public static Color32 orange= new Color32(255, 100,0, 255);


    //gerenal map settings
    public static float MapWidth = 70;//done
    public static float MapHeight = 70;//done
    
    //track creation settings
    public static float MinCornerWidth = 90;
    //lerp step that is applied while trying to reach the minimum corner angle
    public static float CornerBroadeningLerpStep = 0.1f;
    public static int PtCtPerQuad =500;
    public static float PointSpacing =4.7f;

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
    public static List<Vector2> InnerBarrierPoints;
    public static List<Vector2> OuterBarrierPoints;

    public static float InnerBarrierOffset = 2f;
    public static float OuterBarrierOffset = 2f;
    public static float BarrierThickness = 0.05f;
    public static int BarrierMeshPointFrequency = 10;
    public static int BarrierColliderResolution = 2;
    

    //CURRENT game state MAP data
    public static int Curr_TrackRotation;
    public static List<Vector2> Curr_RawPoints;
    public static List<Vector2> Curr_ControlPoints;
    public static List<Vector2> Curr_TrackPoints;

    //racing line settings + data
    public static float RacingLineTightness = 0.1f;
    public static float RacingLineWaypointFreq = 10;
    public static List<Vector2> Curr_RacingLinePoints;

    //saved game data
    public static List<SavedTrack> Svd_Tracks;

    

}


//data structure for saving a track
public struct SavedTrack 
{
    public List<Vector2> RawPoints;
    public string name;
    public int id;
    
}
