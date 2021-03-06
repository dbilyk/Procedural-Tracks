using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRenderer : MapCreator {
  [SerializeField]
  BarrierCreator barrierCreator;
  [SerializeField]
  FoliageCreator foliageCreator;
  [SerializeField]
  User user;

  RacingLineCreator racingLineCreator = new RacingLineCreator();

  List<GameObject> _meshHelpers = new List<GameObject> ();
  public List<GameObject> MeshHelpers {
    get {
      return _meshHelpers;
    }
    private set {
      _meshHelpers = value;
    }
  }

  float CornerLerpStep = 0.1f;
  int PtsPerQuad = 50;

  //these are the configurable track params that affect the algorithm_______________
  float PointSpacing = 4f;
  int CornerWidth = 100;

//implement this when UI changes are applied so that track can be capped by number of corners.
  int CornerCount = 30;

  float MapWidth = 100;
  float MapHeight = 100;
  //______________________________________________________________________________________________________________

  public readonly short MeshTrackPointFreq = 30;
  public readonly float MeshThickness = 1.2f;
  int TrackColliderResolution = 13;

  //berm Decal creation settings
  int BermLength = 30;
  float BermWidth = 0.2f, BermOffset = 1.2f;

  //barrier creation settings
  public readonly float InnerBarrierOffset = 2f, OuterBarrierOffset = 2f;
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

  public delegate void NewTrackCreated();
  public event NewTrackCreated OnNewTrackCreated;
  
  public void GenerateNewTrackData (Track track) {
    track.TrackPtFrequency = this.MeshTrackPointFreq;
    track.TrackWidth = this.MeshThickness;
    track.RawPoints = new List<Vector2> ();

    track.RawPoints = CreateRawUnsortedPoints (MapWidth, MapHeight, PtsPerQuad);
    track.RawPoints = SortPoints (track.RawPoints);
    
    //have to run point thinning and angle adjustment several times because they recursively affect each other.
    StopAllCoroutines();
    StartCoroutine(ApplyTrackConstraints(track));
    
  }

  public void afterRawPointsCalc(Track track){
    track.CornerCount = track.RawPoints.Count-1;
    track.Length = GetTrackLength(track.RawPoints);
    track.FastestLap = 0;
    track.ControlPoints = CreateControlPoints (track.RawPoints);
    track.TrackPoints = CreateTrackPoints (track.ControlPoints, MeshTrackPointFreq);

    //track.RacingLinePoints = CreateRacingLinePoints (track.RawPoints, RacingLineWaypointFreq, RacingLineTightness);
    track.RacingLinePoints = racingLineCreator.createRacingLine(track);
    MeshHelpers = CreateOrSetMeshHelperObjects (track.TrackPoints, MeshHelpers);
    RotateTrackObjectsAlongCurves (MeshHelpers);
    //populates track object with Tforms of all 
    CreateStartingGridData (
      MeshHelpers,
      StartingGridLength,
      StartingGridWidth,
      NumberOfGridPositions,
      MeshTrackPointFreq,
      track);

    //create barrier data
    track.InnerBarrierRawPoints = barrierCreator.CreateOutline (track.RawPoints, InnerBarrierOffset, "inner");
    track.OuterBarrierRawPoints = barrierCreator.CreateOutline (track.RawPoints, OuterBarrierOffset, "outer");
    foliageCreator.GenerateEnvironmentData (track);

    //when all this is done, trigger an event to allow the ui to render the line drawing of this track.
    if(OnNewTrackCreated != null) OnNewTrackCreated();
  }

   IEnumerator ApplyTrackConstraints(Track track){
    List<Vector2> result = new List<Vector2>(track.RawPoints);
    bool keepGoing = true;
    bool firstCheckDone = false;
    int LastPointCount;
    while(keepGoing){
      LastPointCount = result.Count;
      RawAnglesDataAndChangeCount anglesData;
      //do a round of these.
      result = RemovePointsTooClose (result, PointSpacing);
      if(result.Count < 5){
        result = new List<Vector2>(CreateRawUnsortedPoints(MapWidth,MapHeight,PtsPerQuad));
        result = SortPoints (result);
        result = RemovePointsTooClose (result, PointSpacing);
      }
      anglesData = CheckControlPointAngles (result, CornerLerpStep, CornerWidth);
      result = anglesData.Data;
      
      if(LastPointCount == anglesData.Data.Count && anglesData.ChangeCount == 1){
        
        if(!firstCheckDone){
          firstCheckDone = true;
        }
        else{
          keepGoing = false;
          track.RawPoints = result;
        }
      }
      else{
        firstCheckDone = false;
      }
      
      yield return new WaitForSecondsRealtime(0.01f);
    }
    //this proceeds to do everything else pertaining to creating the track
    afterRawPointsCalc(track);
    
  }





  public void GenerateLevel (Track track) {
    CreateOrSetMeshHelperObjects (track.TrackPoints, MeshHelpers);

    RotateTrackObjectsAlongCurves (MeshHelpers);

    RenderStartingGrid (track, user.OpponentQty);

    //mesh creation
    CreateTrackBerms (
      MeshHelpers,
      BermWidth,
      BermOffset,
      BermLength,
      BermDecals.GetComponent<MeshFilter> (),
      track.RawPoints,
      MeshTrackPointFreq);

    //populates this track's 
    CreateTrackMesh (
      MeshHelpers,
      MeshThickness,
      CurrentGameTrack.gameObject.GetComponent<MeshFilter> (),
      track.InnerTrackPoints,
      track.OuterTrackPoints
      //track.trackMeshData
    );

    CreateColliderForTrack (
      track.OuterTrackPoints,
      track.InnerTrackPoints,
      TrackColliderResolution,
      CurrentGameTrack.GetComponent<PolygonCollider2D> ());

    //populates current racing line with correct data

    CreateBarriers (track.InnerBarrierRawPoints, track, InnerBarrier);
    CreateBarriers (track.OuterBarrierRawPoints, track, OuterBarrier);

    foliageCreator.RenderEnvironment (track);

  }

  void CreateBarriers (List<Vector2> barrierRawPointData, Track track, GameObject barrierObj) {
    List<Vector2> trackPoints = CreateControlPoints (barrierRawPointData);
    trackPoints = CreateTrackPoints (trackPoints, BarrierMeshPointFrequency);
    CreateOrSetMeshHelperObjects (trackPoints, MeshHelpers);
    RotateTrackObjectsAlongCurves (MeshHelpers);

    //lists to create colliders for barriers.
    List<Vector2> outerBarrierPts = new List<Vector2> ();
    List<Vector2> innerBarrierPts = new List<Vector2> ();

    CreateTrackMesh (
      MeshHelpers,
      BarrierThickness,
      barrierObj.GetComponent<MeshFilter> (),
      outerBarrierPts,
      innerBarrierPts
      //track.outerBarrierData
    );

    CreateColliderForTrack (
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