﻿using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour {
    //gerenal map settings
    public static float MapWidth = 40;//done
    public static float MapHeight = 40;//done
    
    //track creation settings
    public static float MinCornerWidth = 70;
    public static int PtCtPerQuad = 9;
    public static float PointSpacing = 4;
    public static float TrackPointFreq = 40;

    //track mesh and collider settings
    public static float TrackMeshThickness = -0.1f;
    public static int TrackColliderResolution = 24; 


    //CURRENT game state MAP data
    public static List<Vector2> Curr_RawPoints;//done
    public static List<Vector2> Curr_ControlPoints;//done
    public static List<Vector2> Curr_TrackPoints;//done

    
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
