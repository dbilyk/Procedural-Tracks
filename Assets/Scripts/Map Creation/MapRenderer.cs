using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRenderer : MapCreator {
  [SerializeField]
  MapCreator mapCreator;
  [SerializeField]
  BarrierCreator barrierCreator;
  [SerializeField]
  FoliageCreator foliageCreator;
  [SerializeField]
  User user;

  List<GameObject> _meshHelpers = new List<GameObject> ();
  public List<GameObject> MeshHelpers {
    get {
      return _meshHelpers;
    }
    private set {
      _meshHelpers = value;
    }
  }

  float MapWidth = 80;
  float MapHeight = 80;
  float CornerLerpStep = 0.1f;
  int PtsPerQuad = 500;

  float PointSpacing = 3f;
  int CornerWidth = 125;

  public readonly short MeshTrackPointFreq = 30;
  float MeshThickness = 1.2f;
  int TrackColliderResolution = 13;

  //berm Decal creation settings
  int BermLength = 30;
  float BermWidth = 0.2f;
  float BermOffset = 1.2f;

  //barrier creation settings
  public readonly float InnerBarrierOffset = 2f;
  public readonly float OuterBarrierOffset = 2f;
  float BarrierThickness = 0.05f;
  int BarrierMeshPointFrequency = 9;
  int BarrierColliderResolution = 2;

  // SET THESE IN THE TRACK OBJECT, and DELETE from here. ---------------------------------------- DELETE ME and refactor into Track obj
  List<Vector3> Curr_Verts = new List<Vector3> ();
  List<Vector3> Curr_Normals = new List<Vector3> ();
  List<Vector2> Curr_UVs = new List<Vector2> ();
  List<int> Curr_Indicies = new List<int> ();

  //starting grid
  [SerializeField]
  GameObject StartingLine, BermDecals, CurrentGameTrack, InnerBarrier, OuterBarrier, FoliageContainer;
  List<GameObject> CarStartingPositions;
  float StartingGridLength = 3;
  float StartingGridWidth = 0.5f;
  //also controls how many AI are on the track
  int NumberOfGridPositions = 8;

  //racing line settings + data
  float RacingLineTightness = 0.3f;
  float RacingLineWaypointFreq = 10;

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

    track.RacingLinePoints = mapCreator.CreateRacingLinePoints (track.RawPoints, RacingLineWaypointFreq, RacingLineTightness);
    MeshHelpers = mapCreator.CreateOrSetMeshHelperObjects (track.TrackPoints, MeshHelpers);
    mapCreator.RotateTrackObjectsAlongCurves (MeshHelpers);
    //populates track object with Tforms of all 
    mapCreator.CreateStartingGridData (
      MeshHelpers,
      StartingGridLength,
      StartingGridWidth,
      NumberOfGridPositions,
      MeshTrackPointFreq,
      track);

    //create barrier data
    track.InnerBarrierRawPoints = barrierCreator.CreateOutline (track.RawPoints, InnerBarrierOffset, "inner");
    track.OuterBarrierRawPoints = barrierCreator.CreateOutline (track.RawPoints, OuterBarrierOffset, "outer");
    foliageCreator.GenerateFoliageData (track);
  }

  public void GenerateLevel (Track track) {
    mapCreator.CreateOrSetMeshHelperObjects (track.TrackPoints, MeshHelpers);

    mapCreator.RotateTrackObjectsAlongCurves (MeshHelpers);

    RenderStartingGrid (track, user.OpponentQty);

    //mesh creation
    mapCreator.CreateTrackBerms (
      MeshHelpers,
      BermWidth,
      BermOffset,
      BermLength,
      BermDecals.GetComponent<MeshFilter> (),
      track.RawPoints,
      MeshTrackPointFreq);

    //populates this track's 
    mapCreator.CreateTrackMesh (
      MeshHelpers,
      MeshThickness,
      CurrentGameTrack.gameObject.GetComponent<MeshFilter> (),
      track.InnerTrackPoints,
      track.OuterTrackPoints
      //track.trackMeshData
    );

    mapCreator.CreateColliderForTrack (
      track.OuterTrackPoints,
      track.InnerTrackPoints,
      TrackColliderResolution,
      CurrentGameTrack.GetComponent<PolygonCollider2D> ());

    //populates current racing line with correct data

    CreateBarriers (track.InnerBarrierRawPoints, track, InnerBarrier);
    CreateBarriers (track.OuterBarrierRawPoints, track, OuterBarrier);

    foliageCreator.RenderFoliage (track);

  }

  void CreateBarriers (List<Vector2> barrierRawPointData, Track track, GameObject barrierObj) {
    List<Vector2> trackPoints = mapCreator.CreateControlPoints (barrierRawPointData);
    trackPoints = mapCreator.CreateTrackPoints (trackPoints, BarrierMeshPointFrequency);
    mapCreator.CreateOrSetMeshHelperObjects (trackPoints, MeshHelpers);
    mapCreator.RotateTrackObjectsAlongCurves (MeshHelpers);

    //lists to create colliders for barriers.
    List<Vector2> outerBarrierPts = new List<Vector2> ();
    List<Vector2> innerBarrierPts = new List<Vector2> ();

    mapCreator.CreateTrackMesh (
      MeshHelpers,
      BarrierThickness,
      barrierObj.GetComponent<MeshFilter> (),
      outerBarrierPts,
      innerBarrierPts
      //track.outerBarrierData
    );

    mapCreator.CreateColliderForTrack (
      outerBarrierPts,
      innerBarrierPts,
      BarrierColliderResolution,
      barrierObj.GetComponent<PolygonCollider2D> ());

  }

  void RenderStartingGrid (Track track, int posQty) {
    //position the starting line
    StartingLine.transform.position = track.StartingLineTform.position;
    StartingLine.transform.rotation = track.StartingLineTform.rotation;

    //disable all outlines first
    for (int i = 0; i < StartingOutlines.Count; i++) {
      StartingOutlines[i].SetActive (false);
    }

    //then enable and set tForm for the desired Qty of positions.
    for (int i = 0; i < posQty; i++) {
      GameObject targetOutline = StartingOutlines[i];
      targetOutline.SetActive (true);
      targetOutline.transform.position = track.CarStartingPositions[i].position;
      targetOutline.transform.rotation = track.CarStartingPositions[i].rotation;
    }
  }
}