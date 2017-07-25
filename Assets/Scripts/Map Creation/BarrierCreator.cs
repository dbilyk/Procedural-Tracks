using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BarrierCreator : MonoBehaviour {

    public MapCreator mapGen;
    public GameObject BarrierContainer;
    public GameObject InnerBarrier;
    public GameObject OuterBarrier;

   
    //takes original track data and divides each point in the data by the trackpointDivisor, thereby shrinking or expanding the entire track
    public List<Vector2> CreateBarriers(List<Vector2> currentRawPts, float trackCenterpointDivisor, float tireRadius, string InnerOrOuter)
    {
        //pass in original control points data
        List<Vector2> BarrierPoints = new List<Vector2>(currentRawPts);
        //DebugPlot(BarrierPoints, new Color32(0, 0, 0, 255));

        List<Vector2> NewCtrlPoints = new List<Vector2>();

        //shifts all control points inward along the normal of the two adjacent points on either side, controlled by the trackCenterpointDivisor Var.
        for (int i = 0; i < BarrierPoints.Count; i++)
        {
            //move first point into position
            if (i == 0)
            {
                Vector2 AC = BarrierPoints[BarrierPoints.Count - 1] - BarrierPoints[i + 1];
                Vector2 NormalAC = new Vector2(-AC.y, AC.x).normalized;
                NewCtrlPoints.Add(BarrierPoints[i] - (NormalAC * -trackCenterpointDivisor));

            }


            //most points
            if (i + 1 < BarrierPoints.Count && i != 0)
            {
                Vector2 AC = BarrierPoints[i - 1] - BarrierPoints[i + 1];
                Vector2 NormalAC = new Vector2(-AC.y, AC.x).normalized;
                NewCtrlPoints.Add(BarrierPoints[i] - (NormalAC * -trackCenterpointDivisor));

            }

            //move the last point in array into position
            if (i + 1 == BarrierPoints.Count)
            {
                Vector2 AC = BarrierPoints[i - 1] - BarrierPoints[0];
                Vector2 NormalAC = new Vector2(-AC.y, AC.x).normalized;
                NewCtrlPoints.Add(BarrierPoints[i] - (NormalAC * -trackCenterpointDivisor));

            }

        }
        //removes one of any two control points that are within a certain distance of one another.
        for (int i = NewCtrlPoints.Count - 1; i >= 1; i--)
        {
            float distBetweenPts;
            if (i == NewCtrlPoints.Count - 1)
            {
                distBetweenPts = Vector2.Distance(NewCtrlPoints[i], NewCtrlPoints[0]);
                if (distBetweenPts <= Data.BarrierCornerKinkFactor)
                {
                    NewCtrlPoints.RemoveAt(i);
                }
            }
            else
            {
                distBetweenPts = Vector2.Distance(NewCtrlPoints[i], NewCtrlPoints[i - 1]);
                if (distBetweenPts <= Data.BarrierCornerKinkFactor)
                {
                    NewCtrlPoints.RemoveAt(i);
                }
            }
            
        }
        
        List<Vector2> MPs = mapGen.CreateControlPoints(NewCtrlPoints);
        
        int currentTirePosition = 0;
        List<int> PointIndexesToDelete = new List<int>();

        BarrierPoints = mapGen.CreateTrackPoints(MPs, 100);
        //shrink the data
        //for (int i = 0; i < BarrierPoints.Count; i++)
        //{
        //    BarrierPoints[i] = new Vector2(BarrierPoints[i].x /1.1f , BarrierPoints[i].y / (trackCenterpointDivisor + (trackCenterpointDivisor/8)));
        //}

        //add point indexs that are too close together to place the tires to a new array;
        for (int i = 0; i < BarrierPoints.Count; i++)
        {
            if (Vector2.Distance(BarrierPoints[currentTirePosition], BarrierPoints[i]) < tireRadius * 2)
            {
                PointIndexesToDelete.Add(i);
            }
            else
            {
                currentTirePosition = i;
            }
            
        }
        //remove points that are too close together for placing tire barriers via above array
        for (int i = PointIndexesToDelete.Count - 1; i >= 0; i--)
        {
            BarrierPoints.RemoveAt(PointIndexesToDelete[i]);
        }


        foreach (Vector2 pt in BarrierPoints)
        {
            GameObject barrier = Instantiate(Barrier, BarrierContainer.transform);
            barrier.transform.position = pt;
        }

        return BarrierPoints;
    }
}
