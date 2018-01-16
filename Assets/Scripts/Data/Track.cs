using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData {
  public List<Vector3> Verts = new List<Vector3> ();
  public List<Vector2> UVs = new List<Vector2> ();
  public List<Vector3> Normals = new List<Vector3> ();
  public List<int> Indicies = new List<int> ();
}

public class Tform {
  public Vector2 position;
  public Quaternion rotation;

}

public class Track {
  //populated on GenerateLevel
  public MeshData trackMeshData = new MeshData ();
  public List<Vector2> OuterTrackPoints;
  public List<Vector2> InnerTrackPoints;

  public MeshData outerBarrierData = new MeshData ();
  public MeshData innerBarrierData = new MeshData ();

  //populated on track GenerateNewTrackData()
  public List<Vector2> OuterBarrierRawPoints;
  public List<Vector2> InnerBarrierRawPoints;
  public List<Vector2> RawPoints;
  public List<Vector2> ControlPoints;
  public List<Vector2> TrackPoints;

  public List<Vector2> RacingLinePoints;

  public List<Tform> CarStartingPositions = new List<Tform> ();
  public Tform StartingLineTform;

  public List<Vector2> FoliageLocations;
}