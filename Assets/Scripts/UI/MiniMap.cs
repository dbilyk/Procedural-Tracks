using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour {
    public MapCreator MapCreator;
    public int minimapResolution;
    public LineRenderer minimapLine;


    void OnEnable()
    {
        CreateMinimap(Data.Curr_ControlPoints);
    }

    public void CreateMinimap (List<Vector2> passedControlPoints) {
        
        List<Vector2> passedData = new List<Vector2>(passedControlPoints);
        passedData = MapCreator.CreateTrackPoints(passedData, minimapResolution);
        minimapLine.positionCount = passedData.Count;
        for(int i =0; i <passedData.Count; i ++)
        {
            minimapLine.SetPosition(i, new Vector3(passedData[i].x, passedData[i].y, 10));
        }
    }


}
