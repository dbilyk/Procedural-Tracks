using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRenderer : MapCreator {
  MapCreator mapCreator = new MapCreator ();

  public List<GameObject> MeshHelpers { get; set; } = new List<GameObject> ();

  float MapWidth { get; set; } = 80;
  float MapHeight { get; set; } = 80;
  float CornerLerpStep { get; } = 0.1f;
  int PtsPerQuad { get; } = 500;

  float PointSpacing { get; set; } = 3f;
  int CornerWidth { get; set; } = 125;

  List<Vector2> Curr_OuterTrackPoints { get; set; } = new List<Vector2> ();
  List<Vector2> Curr_InnerTrackPoints { get; set; } = new List<Vector2> ();
  float MeshTrackPointFreq { get; set; } = 30;

  int TrackColliderResolution { get; } = 13;

  //berm Decal creation settings
  int BermLength { get; } = 30;
  float BermWidth { get; } = 0.2f;
  float BermOffset { get; } = 1.2f;

  // SET THESE IN THE TRACK OBJECT, and DELETE from here. ---------------------------------------- DELETE ME and refactor into Track obj
  List<Vector3> Curr_Verts { get; set; } = new List<Vector3> ();
  List<Vector3> Curr_Normals { get; set; } = new List<Vector3> ();
  List<Vector2> Curr_UVs { get; set; } = new List<Vector2> ();
  List<int> Curr_Indicies { get; set; } = new List<int> ();

  //starting grid
  [SerializeField]
  GameObject StartingLine;
  List<GameObject> CarStartingPositions;
  float StartingGridLength = 3;
  float StartingGridWidth = 0.5f;
  //also controls how many AI are on the track
  int NumberOfGridPositions = 3;

  //racing line settings + data
  float RacingLineTightness = 0.3f;
  float RacingLineWaypointFreq = 10;

  private Track track = new Track ();

  public void GenerateNewTrackData (Track track) {

    track.RawPoints = new List<Vector2> ();

    track.RawPoints = mapCreator.CreateRawUnsortedPoints (MapWidth, MapHeight, PtsPerQuad);
    track.RawPoints = mapCreator.SortPoints (track.RawPoints);
    //have to run point thinning and angle adjustment several times because they recursively affect each other.
    for (int i = 0; i < 50; i++) {
      track.RawPoints = mapCreator.RemovePointsTooClose (track.RawPoints, PointSpacing);
      track.RawPoints = mapCreator.CheckControlPointAngles (track.RawPoints, CornerLerpStep, CornerWidth);
    }

    //track.RawPoints = MapCreator.ApplyRandomRotation(track.RawPoints);
    track.ControlPoints = mapCreator.CreateControlPoints (track.RawPoints);
    track.TrackPoints = mapCreator.CreateTrackPoints (track.ControlPoints, MeshTrackPointFreq);
  }

  public void GenerateLevel () {
    //mesh creation
    mapCreator.CreateOrSetMeshHelperObjects (track.TrackPoints, MeshHelpers);
    mapCreator.RotateTrackObjectsAlongCurves (MeshHelpers);
    mapCreator.CreateStartingGrid (MeshHelpers, StartingGridLength, StartingGridWidth, NumberOfGridPositions);
    mapCreator.CreateTrackBerms (Data.CurrentMeshHelperObjects, Data.BermWidth, Data.BermOffset, Data.BermLength, BermDecals.GetComponent<MeshFilter> ());
    mapCreator.CreateTrackMesh (Data.CurrentMeshHelperObjects, Data.TrackMeshThickness, ActiveGameTrack.gameObject.GetComponent<MeshFilter> ());
    mapCreator.CreateColliderForTrack (Data.Curr_OuterTrackPoints, Data.Curr_InnerTrackPoints, Data.TrackColliderResolution, ActiveGameTrack.GetComponent<PolygonCollider2D> ());

    //populates current racing line with correct data
    Data.Curr_RacingLinePoints = mapCreator.CreateRacingLinePoints (Data.Curr_RawPoints, Data.RacingLineWaypointFreq, Data.RacingLineTightness);

    //create barriers
    Data.InnerBarrierPoints = InnerBarrier.CreateOutline (Data.Curr_RawPoints, Data.InnerBarrierOffset, "inner");
    Data.OuterBarrierPoints = OuterBarrier.CreateOutline (Data.Curr_RawPoints, Data.OuterBarrierOffset, "outer");
    InnerBarrier.CreateBarrier (Data.InnerBarrierPoints);
    OuterBarrier.CreateBarrier (Data.OuterBarrierPoints);

    FoliageContainer.SetActive (true);
    StaticBatchingUtility.Combine (FoliageContainer);
  }
}