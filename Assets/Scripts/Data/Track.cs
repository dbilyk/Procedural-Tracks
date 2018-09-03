using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class MeshData {
  public List<Vector3> Verts = new List<Vector3> ();
  public List<Vector2> UVs = new List<Vector2> ();
  public List<Vector3> Normals = new List<Vector3> ();
  public List<int> Indicies = new List<int> ();
}

public class Tform {
  public Vector3 position;
  public Quaternion rotation;

}

[Serializable]
public class sTform {
  public V3 position;
  public Quat rotation;
}

[Serializable]
public class V2 {
  public float x;
  public float y;
}

[Serializable]
public class V3 {
  public float x;
  public float y;
  public float z;
}

//stored as euler
[Serializable]
public class Quat {
  public float x;
  public float y;
  public float z;

}

[Serializable]
public class Track {
  public int CornerCount, TrackPtFrequency;
  public float GoldMultiplier,SilverMultiplier, BronzeMultiplier, Length, FastestLap, TrackWidth;
  public string Name;

  public List<Vector2> OuterTrackPoints = new List<Vector2> ();
  public List<Vector2> InnerTrackPoints = new List<Vector2> ();

  //public MeshData trackMeshData = new MeshData ();
  //public MeshData outerBarrierData = new MeshData ();
  //public MeshData innerBarrierData = new MeshData ();

  //populated on track GenerateNewTrackData()

  public List<Vector2> OuterBarrierRawPoints = new List<Vector2> ();
  public List<Vector2> InnerBarrierRawPoints = new List<Vector2> ();
  public List<Vector2> RawPoints = new List<Vector2> ();
  public List<Vector2> ControlPoints = new List<Vector2> ();
  public List<Vector2> TrackPoints = new List<Vector2> ();

  public List<Vector2> RacingLinePoints = new List<Vector2> ();

  public List<Tform> CarStartingPositions = new List<Tform> ();
  public Tform StartingLineTform = new Tform ();

  public List<Vector2> SmallEnvModelLocsInner = new List<Vector2> ();
  public List<Vector2> LrgEnvModelLocsInner = new List<Vector2>();
   
  public List<Vector2> SmallEnvModelLocsOuter = new List<Vector2> ();
  public List<Vector2> LrgEnvModelLocsOuter = new List<Vector2>();

  public List<Tform> LandmarkLocs = new List<Tform>();
  //cloning constructor, yes this is BAD because now i have to update it when new fields added...
  public Track (Track t) {
    this.TrackPtFrequency = t.TrackPtFrequency;
    this.TrackWidth = t.TrackWidth;

    this.CornerCount = t.CornerCount;
    this.Length = t.Length;
    this.FastestLap = t.FastestLap;
    this.GoldMultiplier = t.GoldMultiplier;
    this.SilverMultiplier = t.SilverMultiplier;
    this.BronzeMultiplier = t.BronzeMultiplier;
    this.Name = t.Name;

    this.OuterTrackPoints = t.OuterTrackPoints;
    this.InnerTrackPoints = t.InnerTrackPoints;
    this.OuterBarrierRawPoints = t.OuterBarrierRawPoints;
    this.InnerBarrierRawPoints = t.InnerBarrierRawPoints;
    this.RawPoints = t.RawPoints;
    this.ControlPoints = t.ControlPoints;
    this.TrackPoints = t.TrackPoints;
    this.RacingLinePoints = t.RacingLinePoints;
    this.CarStartingPositions = t.CarStartingPositions;
    this.StartingLineTform = t.StartingLineTform;
    
    this.SmallEnvModelLocsInner = t.SmallEnvModelLocsInner;
    this.SmallEnvModelLocsOuter = t.SmallEnvModelLocsOuter;
    
    this.LrgEnvModelLocsInner = t.LrgEnvModelLocsInner;
    this.LrgEnvModelLocsOuter = t.LrgEnvModelLocsOuter;
    
    this.LandmarkLocs = t.LandmarkLocs;

    

  }
  public Track () { }
  //populates this track object with equivalent sTrack data
  public void DeserializeTrack (sTrack serialTrack) {
    this.CornerCount = serialTrack.CornerCount;
    this.Length = serialTrack.Length;
    this.FastestLap = serialTrack.FastestLap;
    this.GoldMultiplier = serialTrack.GoldMultiplier;
    this.SilverMultiplier = serialTrack.SilverMultiplier;
    this.BronzeMultiplier = serialTrack.BronzeMultiplier;
    this.Name = serialTrack.Name;


    this.TrackPtFrequency = serialTrack.TrackPtFrequency;
    this.TrackWidth = serialTrack.TrackWidth;
    this.OuterTrackPoints = serialTrack.OuterTrackPoints.DeserializeListV2 ();
    this.InnerTrackPoints = serialTrack.InnerTrackPoints.DeserializeListV2 ();
    this.OuterBarrierRawPoints = serialTrack.OuterBarrierRawPoints.DeserializeListV2 ();
    this.InnerBarrierRawPoints = serialTrack.InnerBarrierRawPoints.DeserializeListV2 ();
    this.RawPoints = serialTrack.RawPoints.DeserializeListV2 ();
    this.ControlPoints = serialTrack.ControlPoints.DeserializeListV2 ();
    this.TrackPoints = serialTrack.TrackPoints.DeserializeListV2 ();
    this.RacingLinePoints = serialTrack.RacingLinePoints.DeserializeListV2 ();

    this.SmallEnvModelLocsInner = serialTrack.SmallEnvModelLocsInner.DeserializeListV2 ();
    this.SmallEnvModelLocsOuter = serialTrack.SmallEnvModelLocsOuter.DeserializeListV2 ();

    this.LrgEnvModelLocsInner = serialTrack.LrgEnvModelLocsInner.DeserializeListV2 ();
    this.LrgEnvModelLocsOuter = serialTrack.LrgEnvModelLocsOuter.DeserializeListV2 ();

    for (int i = 0; i < serialTrack.LandmarkLocs.Count; i++) {
      Tform serialCopy = new Tform ();
      serialCopy.position = serialTrack.LandmarkLocs[i].position.DeserializeV3 ();
      serialCopy.rotation = serialTrack.LandmarkLocs[i].rotation.DeserializeQuat ();
      this.LandmarkLocs.Add (serialCopy);
    }

    for (int i = 0; i < serialTrack.CarStartingPositions.Count; i++) {
      Tform serialCopy = new Tform ();
      serialCopy.position = serialTrack.CarStartingPositions[i].position.DeserializeV3 ();
      serialCopy.rotation = serialTrack.CarStartingPositions[i].rotation.DeserializeQuat ();
      this.CarStartingPositions.Add (serialCopy);
    }
    this.StartingLineTform.position = serialTrack.StartingLineTform.position.DeserializeV3 ();
    this.StartingLineTform.rotation = serialTrack.StartingLineTform.rotation.DeserializeQuat ();
  }

  public sTrack toSerializedTrack () {
    sTrack result = new sTrack ();

    result.CornerCount = this.CornerCount;
    result.Length = this.Length;
    result.FastestLap = this.FastestLap;
    result.GoldMultiplier = this.GoldMultiplier;
    result.SilverMultiplier = this.SilverMultiplier;
    result.BronzeMultiplier = this.BronzeMultiplier;
    result.Name = this.Name;
    

    result.TrackPtFrequency = this.TrackPtFrequency;
    result.TrackWidth = this.TrackWidth;

    result.OuterTrackPoints = this.OuterTrackPoints.SerializeListV2 ();
    result.InnerTrackPoints = this.InnerTrackPoints.SerializeListV2 ();
    result.OuterBarrierRawPoints = this.OuterBarrierRawPoints.SerializeListV2 ();
    result.InnerBarrierRawPoints = this.InnerBarrierRawPoints.SerializeListV2 ();
    result.RawPoints = this.RawPoints.SerializeListV2 ();
    result.ControlPoints = this.ControlPoints.SerializeListV2 ();
    result.TrackPoints = this.TrackPoints.SerializeListV2 ();

    result.RacingLinePoints = this.RacingLinePoints.SerializeListV2 ();
    result.SmallEnvModelLocsInner = this.SmallEnvModelLocsInner.SerializeListV2 ();
    result.SmallEnvModelLocsOuter = this.SmallEnvModelLocsOuter.SerializeListV2 ();
    

    result.LrgEnvModelLocsInner = this.LrgEnvModelLocsInner.SerializeListV2 ();
    result.LrgEnvModelLocsOuter = this.LrgEnvModelLocsOuter.SerializeListV2 ();
    
   for (int i = 0; i < this.LandmarkLocs.Count; i++) {
      sTform serialCopy = new sTform ();
      serialCopy.position = this.LandmarkLocs[i].position.SerializeV3 ();
      serialCopy.rotation = this.LandmarkLocs[i].rotation.SerializeQuat ();
      result.LandmarkLocs.Add (serialCopy);
    } 
    
    for (int i = 0; i < this.CarStartingPositions.Count; i++) {
      sTform serialCopy = new sTform ();
      serialCopy.position = this.CarStartingPositions[i].position.SerializeV3 ();
      serialCopy.rotation = this.CarStartingPositions[i].rotation.SerializeQuat ();
      result.CarStartingPositions.Add (serialCopy);
    }
    result.StartingLineTform.position = this.StartingLineTform.position.SerializeV3 ();
    result.StartingLineTform.rotation = this.StartingLineTform.rotation.SerializeQuat ();

    return result;

  }

}

//this is the serializable version of my track data
[Serializable]
public class sTrack {
  public int CornerCount, TrackPtFrequency;
  public float GoldMultiplier,SilverMultiplier, BronzeMultiplier, Length, FastestLap, TrackWidth;
  public string Name; 

  //populated on GenerateLevel
  public List<V2> OuterTrackPoints = new List<V2> ();
  public List<V2> InnerTrackPoints = new List<V2> ();

  //populated on track GenerateNewTrackData()
  public List<V2> OuterBarrierRawPoints = new List<V2> ();
  public List<V2> InnerBarrierRawPoints = new List<V2> ();
  public List<V2> RawPoints = new List<V2> ();
  public List<V2> ControlPoints = new List<V2> ();
  public List<V2> TrackPoints = new List<V2> ();

  public List<V2> RacingLinePoints = new List<V2> ();

  public List<sTform> CarStartingPositions = new List<sTform> ();
  public sTform StartingLineTform = new sTform ();

  public List<V2> SmallEnvModelLocsInner = new List<V2> ();
  public List<V2> SmallEnvModelLocsOuter = new List<V2> ();
  public List<V2> LrgEnvModelLocsInner = new List<V2> ();
  public List<V2> LrgEnvModelLocsOuter = new List<V2> ();

  public List<sTform> LandmarkLocs = new List<sTform>();

}