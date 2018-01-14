using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData {
  public List<Vector3> Verts = new List<Vector3> ();
  public List<Vector2> UVs = new List<Vector2> ();
  public List<Vector3> Normals = new List<Vector3> ();
  public List<int> Indicies = new List<int> ();
}

public class Track {
  public MeshData trackMeshData = new MeshData ();
  public MeshData outerBarrierData = new MeshData ();
  public MeshData innerBarrierData = new MeshData ();

  public Mesh TrackMesh;
  public Mesh InnerBarrier;
  public Mesh OuterBarrier;

  public List<Vector2> RawPoints;
  public List<Vector2> ControlPoints;
  public List<Vector2> TrackPoints;
  public List<Vector2> RacingLinePoints;
  public List<GameObject> CarStartingPositions;
  public List<Vector2> FoliageLocations;
}