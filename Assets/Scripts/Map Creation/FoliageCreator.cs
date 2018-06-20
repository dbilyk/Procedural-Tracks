using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoliageCreator : MonoBehaviour {
  [SerializeField]
  MapRenderer mapRenderer;

  [SerializeField]
  User user;

  [SerializeField]
  List<GameObject> SmallAssets = new List<GameObject> (),
  LargeAssets = new List<GameObject>();
  
  [SerializeField]
  GameObject LandmarkGO;

  [SerializeField]
  float MinSmallObjectSpacing = 5f,
        MinFoliageSpacing = 10f,
        SmallEnvDataOffset = 0.1f,
        LrgEnvDataOffset = 0.3f;
  
  [SerializeField]
  BarrierCreator barrierCreator;
  [SerializeField]
  MapCreator mapCreator;

  [SerializeField]
  GameObject FoliageGroup;

[SerializeField][Tooltip("how many objects to batch into one mesh")]
  int RenderBatchSize = 10;
  
  // Use this for initialization
  public void GenerateEnvironmentData (Track track) {
    short freq = 1000;
    //populates the actual foliage data into track object
    track.SmallEnvModelLocsInner = MakeEquidistantData(track,mapRenderer.InnerBarrierOffset,true, freq, SmallEnvDataOffset);
    track.SmallEnvModelLocsOuter = MakeEquidistantData(track,mapRenderer.OuterBarrierOffset,false, freq,  SmallEnvDataOffset);
    track.LrgEnvModelLocsInner   = MakeEquidistantData(track, mapRenderer.InnerBarrierOffset,true, freq, LrgEnvDataOffset);
    track.LrgEnvModelLocsOuter   = MakeEquidistantData(track, mapRenderer.OuterBarrierOffset,false, freq, LrgEnvDataOffset);

    SetLandmarkPositions(track);

  }

  private List<Vector2> MakeEquidistantData(Track track,float barrierOffset, bool isInner, short frequency,float offset){
    List<Vector2> FoliagePath       = barrierCreator.CreateOutline (track.RawPoints, barrierOffset + offset, (isInner)?"inner":"outer");
                  FoliagePath       = mapRenderer.CreateControlPoints(FoliagePath);
                  FoliagePath       = mapCreator.CreateTrackPoints (FoliagePath, frequency);
    List<Vector2> Result            = new List<Vector2>(); 
                  
    Result.Add(FoliagePath[0]);
    //walk along the path and add only points greater than a certain space apart to results.
    for(int i = 1; i<FoliagePath.Count;i++){
      float diffVector = Vector2.SqrMagnitude(FoliagePath[i] - Result[Result.Count-1]);
      if(diffVector > ((offset == SmallEnvDataOffset)?MinSmallObjectSpacing:MinFoliageSpacing)){
        Result.Add(FoliagePath[i]);
      }
    }
    //logic for determining which dataset we derived and assigning to correct track props.
    return Result;
  }

  [Tooltip("how many points to clear on either side of the desired point.")]
  [SerializeField]
  int LandmarkDataThinningFactor = 3;

  //sets landmark loc track data
  private void SetLandmarkPositions(Track track){
    
    int closeToZero = GetClosestOrFarthestFromZero(track.LrgEnvModelLocsInner, distFromZero.closest);
    int farFromZero = GetClosestOrFarthestFromZero(track.LrgEnvModelLocsOuter, distFromZero.farthest);

    track.LandmarkLocs.Add(GetLandmarkTform(envData.inner,track.LrgEnvModelLocsInner,closeToZero));
    track.LandmarkLocs.Add(GetLandmarkTform(envData.outer,track.LrgEnvModelLocsOuter,farFromZero));
  }
//helper enum for clarity
  enum distFromZero{
    closest = 0,
    farthest = 1
  }

  enum envData{
    inner =0,
    outer =1
  }

  //used to get specific points for landmark target locations
  private int GetClosestOrFarthestFromZero(List<Vector2> data, distFromZero dist){
    int result = 0;
    for(int i=1;i<data.Count; i++){
      
      if(dist == distFromZero.closest){
        if(data[i].sqrMagnitude < data[result].sqrMagnitude){
          result = i;
        }
      }
      else{
        if(data[i].sqrMagnitude > data[result].sqrMagnitude){
          result = i;
        }
      }

    }
    return result;
  }

  //generates the position and rotation Tform for landmarks
  private Tform GetLandmarkTform(envData innerOrOuter, List<Vector2> data, int targetIndex){
    Tform result = new Tform();
   
    //wrap around zero
    int AdjacentIndex = (targetIndex==0)?data.Count-1:targetIndex-1;
    Debug.Log("target index: " + targetIndex);
    //determine normal
    Vector2 normal = data[targetIndex]-data[AdjacentIndex];    
    normal = normal.normalized;

    if(innerOrOuter == envData.inner){
      normal = new Vector2(-normal.y,normal.x);
    }
    else{
      normal = new Vector2(normal.y,-normal.x);
    }
    result.position = data[targetIndex];
    result.rotation = Quaternion.Euler(0,0,Mathf.Round(Mathf.Atan2(normal.y,normal.x)*57.2958f));

    return result;
  }

  
  enum AssetType {
    small = 0,
    large = 1
  }

  public void RenderEnvironment (Track track) {
    //joined small data
    List<Vector2> AllSmallData = new List<Vector2>(track.SmallEnvModelLocsInner);
    AllSmallData.AddRange(track.SmallEnvModelLocsOuter);

    //joined large data
    List<Vector2> AllLargeData = new List<Vector2>(track.LrgEnvModelLocsInner);
    AllLargeData.AddRange(track.LrgEnvModelLocsOuter);


    RenderFoliageHandler(AllSmallData, AssetType.small);
    RenderFoliageHandler(AllLargeData, AssetType.large);
    RenderLandmarks(track.LandmarkLocs);
  }

  private void RenderFoliageHandler(List<Vector2> data,AssetType type){
    List<GameObject> batches = new List<GameObject>();

    batches.Add(GameObject.Instantiate(FoliageGroup,gameObject.transform)); 

    for (int i = 0; i < data.Count; i++) {
      int randIndex;
      List<GameObject> pool;
      //define pool and random index in that pool
      if(type ==AssetType.small){
        pool = SmallAssets;
        randIndex = Random.Range (0, SmallAssets.Count);
      }
      else{
        pool = LargeAssets;
        randIndex = Random.Range (0, LargeAssets.Count);
      }
      
      GameObject newItem = GameObject.Instantiate (pool[randIndex], batches[batches.Count-1].transform);

      newItem.transform.position = new Vector2(Random.insideUnitCircle.x + data[i].x,Random.insideUnitCircle.y + data[i].y);
      newItem.transform.rotation = Quaternion.Euler(0,0,Random.value * 360);
      newItem.transform.localScale *= Random.Range(0.8f,1.2f);
      //if there are too many children under parent, create a new parent before adding to last parent 
      if(batches[batches.Count - 1].transform.childCount > RenderBatchSize){
        StaticBatchingUtility.Combine (batches[batches.Count - 1]);
        batches.Add(GameObject.Instantiate(FoliageGroup)); 
      }
    }
  }

  private void RenderLandmarks(List<Tform> data){
    for(int i=0;i<data.Count;i++){
      GameObject Landmark = GameObject.Instantiate(LandmarkGO, gameObject.transform);
      Landmark.transform.position = data[i].position;
      Landmark.transform.rotation = data[i].rotation;
    }
  }

}