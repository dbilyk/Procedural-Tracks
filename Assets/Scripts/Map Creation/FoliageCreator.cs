using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoliageCreator : MonoBehaviour {
  [SerializeField]
  MapRenderer mapRenderer;

  [SerializeField]
  User user;

  [SerializeField]
  List<GameObject> Foliage = new List<GameObject> ();
  
  [SerializeField]
  float FoliageDensity = 0.5f, 
        MinFoliageSpacing = 10f,
        FoliageOffset = 0.5f;
  
  [SerializeField]
  BarrierCreator barrierCreator;
  [SerializeField]
  MapCreator mapCreator;

  [SerializeField]
  GameObject FoliageGroup;

[SerializeField][Tooltip("how many objects to batch into one mesh")]
  int RenderBatchSize = 10;
  
  // Use this for initialization
  public void GenerateFoliageData (Track track) {
    short freq = 1000;
    MakeEquidistantData(track,mapRenderer.InnerBarrierOffset,true, freq);
    MakeEquidistantData(track,mapRenderer.OuterBarrierOffset,false, freq);
  }

  private void MakeEquidistantData(Track track,float barrierOffset, bool isInner, short frequency){
    List<Vector2> FoliagePath       = barrierCreator.CreateOutline (track.RawPoints, barrierOffset + FoliageOffset, (isInner)?"inner":"outer");
                  FoliagePath       = mapCreator.CreateTrackPoints (FoliagePath, frequency);
    List<Vector2> Result            = new List<Vector2>(); 
                  
    Result.Add(FoliagePath[0]);
    //walk along the path and add only points greater than a certain space apart to results.
    for(int i = 1; i<FoliagePath.Count;i++){
      float diffVector = Vector2.SqrMagnitude(FoliagePath[i] - Result[Result.Count-1]);
      if(diffVector > MinFoliageSpacing){
        Result.Add(FoliagePath[i]);
      }
    }
    //set passed in track data to the quasi-equidistant data.
    if(isInner){
      track.InnerFoliageLocs = Result;
    }
    else{
      track.OuterFoliageLocs = Result;
    }
    
  }

  public void RenderFoliage (Track track) {
    RenderFoliageHandler(track, true);
    RenderFoliageHandler(track, false);
  }
  private void RenderFoliageHandler(Track track, bool isInner){
    List<Vector2> SceneryLocs = (isInner)?track.InnerFoliageLocs:track.OuterFoliageLocs;

    List<GameObject> TreeBatches = new List<GameObject>();
    TreeBatches.Add(GameObject.Instantiate(FoliageGroup)); 
    for (int i = 0; i < SceneryLocs.Count; i++) {
      float rand = Random.value;

      if (rand < FoliageDensity) {
        int        randIndex                  = Random.Range (0, Foliage.Count);
        GameObject newItem                    = GameObject.Instantiate (Foliage[randIndex], gameObject.transform);
                   newItem.transform.position = SceneryLocs[i];
                   newItem.transform.rotation = Quaternion.Euler(0,0,Random.value * 360);
       //if there are too many children under parent, create a new parent before adding to last parent 
        if(TreeBatches[TreeBatches.Count - 1].transform.childCount > RenderBatchSize){
          StaticBatchingUtility.Combine (TreeBatches[TreeBatches.Count - 1]);
          TreeBatches.Add(GameObject.Instantiate(FoliageGroup)); 
        }

        newItem.transform.parent = TreeBatches[TreeBatches.Count-1].transform;
      }
    }
  }
}