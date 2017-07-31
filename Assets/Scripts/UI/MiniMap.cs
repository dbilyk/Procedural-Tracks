using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour {
    public MapCreator MapCreator;
    public GameObject MinimapContainer;
    public GameObject MinimapPoint;
    public int minimapResolution;

    //redo this either as a mesh or by pooling the minimap sprites
    public void CreateMinimap (List<Vector2> passedControlPoints) {
        //this is shit, need to redo this into a pool.
        for (int i = MinimapContainer.transform.childCount -1; i >0; i--)
        {
            Destroy(MinimapContainer.transform.GetChild(i));
        }
        List<Vector2> passedData = new List<Vector2>(passedControlPoints);
        passedData = MapCreator.CreateTrackPoints(passedData, minimapResolution);
        MapCreator.CreateOrSetMeshHelperObjects(passedData);
         
        foreach (GameObject pt in Data.CurrentMeshHelperObjects)
        {
            GameObject newPt = Instantiate(MinimapPoint, MinimapContainer.transform);
            newPt.transform.position = pt.transform.position;
        }
}


}
