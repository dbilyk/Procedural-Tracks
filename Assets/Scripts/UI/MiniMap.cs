using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour {
    public GameObject MinimapContainer;
    public GameObject MinimapPoint;
    public MapCreator MapGen;
    public List<Vector2> TrackPts;
    // Use this for initialization
    void Start () {
       
        TrackPts = Data.Curr_TrackPoints;
        foreach (Vector2 pt in TrackPts)
        {
            GameObject newPt = Instantiate(MinimapPoint, MinimapContainer.transform);
            newPt.transform.position = pt;
        }
}


}
