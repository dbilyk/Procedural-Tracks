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
  //populated on GenerateLevel
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

  public List<Vector2> InnerFoliageLocs = new List<Vector2> ();
  public List<Vector2> OuterFoliageLocs = new List<Vector2> ();

  //cloning constructor, yes this is BAD because now i have to update it when new fields added...
  public Track (Track t) {
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
    this.InnerFoliageLocs = t.InnerFoliageLocs;
    this.OuterFoliageLocs = t.OuterFoliageLocs;

  }
  public Track () { }
  //populates this track object with equivalent sTrack data
  public void DeserializeTrack (sTrack serializedVersion) {
    this.OuterTrackPoints = serializedVersion.OuterTrackPoints.DeserializeListV2 ();
    this.InnerTrackPoints = serializedVersion.InnerTrackPoints.DeserializeListV2 ();
    this.OuterBarrierRawPoints = serializedVersion.OuterBarrierRawPoints.DeserializeListV2 ();
    this.InnerBarrierRawPoints = serializedVersion.InnerBarrierRawPoints.DeserializeListV2 ();
    this.RawPoints = serializedVersion.RawPoints.DeserializeListV2 ();
    this.ControlPoints = serializedVersion.ControlPoints.DeserializeListV2 ();
    this.TrackPoints = serializedVersion.TrackPoints.DeserializeListV2 ();
    this.RacingLinePoints = serializedVersion.RacingLinePoints.DeserializeListV2 ();
    this.InnerFoliageLocs = serializedVersion.InnerFoliageLocs.DeserializeListV2 ();
    this.OuterFoliageLocs = serializedVersion.OuterFoliageLocs.DeserializeListV2 ();
    for (int i = 0; i < serializedVersion.CarStartingPositions.Count; i++) {
      Tform serialCopy = new Tform ();
      serialCopy.position = serializedVersion.CarStartingPositions[i].position.DeserializeV3 ();
      serialCopy.rotation = serializedVersion.CarStartingPositions[i].rotation.DeserializeQuat ();
      this.CarStartingPositions.Add (serialCopy);
    }
    this.StartingLineTform.position = serializedVersion.StartingLineTform.position.DeserializeV3 ();
    this.StartingLineTform.rotation = serializedVersion.StartingLineTform.rotation.DeserializeQuat ();
  }

  public sTrack toSerializedTrack () {
    sTrack result = new sTrack ();
    result.OuterTrackPoints = this.OuterTrackPoints.SerializeListV2 ();
    result.InnerTrackPoints = this.InnerTrackPoints.SerializeListV2 ();
    result.OuterBarrierRawPoints = this.OuterBarrierRawPoints.SerializeListV2 ();
    result.InnerBarrierRawPoints = this.InnerBarrierRawPoints.SerializeListV2 ();
    result.RawPoints = this.RawPoints.SerializeListV2 ();
    result.ControlPoints = this.ControlPoints.SerializeListV2 ();
    result.TrackPoints = this.TrackPoints.SerializeListV2 ();
    result.RacingLinePoints = this.RacingLinePoints.SerializeListV2 ();
    result.InnerFoliageLocs = this.InnerFoliageLocs.SerializeListV2 ();
    result.OuterFoliageLocs = this.OuterFoliageLocs.SerializeListV2 ();
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

  public List<V2> InnerFoliageLocs = new List<V2> ();
  public List<V2> OuterFoliageLocs = new List<V2> ();

}