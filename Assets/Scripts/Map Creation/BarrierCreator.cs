using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BarrierCreator : MonoBehaviour {

    public MapCreator mapCreator;

    //takes original track data and divides each point in the data by the trackpointDivisor, thereby shrinking or expanding the entire track

    //public List<Vector2> CreateBarriers(List<Vector2> currentRawPts, float trackCenterpointDivisor, float tireRadius, string InnerOrOuter)
    //{
    //    Debug.LogWarning("TO DO: Implement Outer Barriers and fix barrier generation");
    //    //pass in original control points data
    //    List<Vector2> BarrierPoints = new List<Vector2>(currentRawPts);
    //    //DebugPlot(BarrierPoints, new Color32(0, 0, 0, 255));

    //    List<Vector2> NewCtrlPoints = new List<Vector2>();

    //    //shifts all control points inward along the normal of the two adjacent points on either side, controlled by the trackCenterpointDivisor Var.
    //    for (int i = 0; i < BarrierPoints.Count; i++)
    //    {
    //        //move first point into position
    //        if (i == 0)
    //        {
    //            Vector2 AC = BarrierPoints[BarrierPoints.Count - 1] - BarrierPoints[i + 1];
    //            Vector2 NormalAC = new Vector2(-AC.y, AC.x).normalized;
    //            NewCtrlPoints.Add(BarrierPoints[i] - (NormalAC * -trackCenterpointDivisor));

    //        }


    //        //most points
    //        if (i + 1 < BarrierPoints.Count && i != 0)
    //        {
    //            Vector2 AC = BarrierPoints[i - 1] - BarrierPoints[i + 1];
    //            Vector2 NormalAC = new Vector2(-AC.y, AC.x).normalized;
    //            NewCtrlPoints.Add(BarrierPoints[i] - (NormalAC * -trackCenterpointDivisor));

    //        }

    //        //move the last point in array into position
    //        if (i + 1 == BarrierPoints.Count)
    //        {
    //            Vector2 AC = BarrierPoints[i - 1] - BarrierPoints[0];
    //            Vector2 NormalAC = new Vector2(-AC.y, AC.x).normalized;
    //            NewCtrlPoints.Add(BarrierPoints[i] - (NormalAC * -trackCenterpointDivisor));

    //        }

    //    }
    //    //removes one of any two control points that are within a certain distance of one another.
    //    for (int i = NewCtrlPoints.Count - 1; i >= 1; i--)
    //    {
    //        float distBetweenPts;
    //        if (i == NewCtrlPoints.Count - 1)
    //        {
    //            distBetweenPts = Vector2.Distance(NewCtrlPoints[i], NewCtrlPoints[0]);
    //            if (distBetweenPts <= Data.BarrierCornerKinkFactor)
    //            {
    //                NewCtrlPoints.RemoveAt(i);
    //            }
    //        }
    //        else
    //        {
    //            distBetweenPts = Vector2.Distance(NewCtrlPoints[i], NewCtrlPoints[i - 1]);
    //            if (distBetweenPts <= Data.BarrierCornerKinkFactor)
    //            {
    //                NewCtrlPoints.RemoveAt(i);
    //            }
    //        }

    //    }

    //    NewCtrlPoints = mapCreator.SortPoints(NewCtrlPoints);

    //    List<Vector2> MPs = mapCreator.CreateControlPoints(NewCtrlPoints);

    //    int currentTirePosition = 0;
    //    List<int> PointIndexesToDelete = new List<int>();

    //    BarrierPoints = mapCreator.CreateTrackPoints(MPs, 100);

    //    //shrink the data
    //    //for (int i = 0; i < BarrierPoints.Count; i++)
    //    //{
    //    //    BarrierPoints[i] = new Vector2(BarrierPoints[i].x /1.1f , BarrierPoints[i].y / (trackCenterpointDivisor + (trackCenterpointDivisor/8)));
    //    //}

    //    //add point indexs that are too close together to place the tires to a new array;
    //    //for (int i = 0; i < BarrierPoints.Count; i++)
    //    //{
    //    //    if (Vector2.Distance(BarrierPoints[currentTirePosition], BarrierPoints[i]) < tireRadius * 2)
    //    //    {
    //    //        PointIndexesToDelete.Add(i);
    //    //    }
    //    //    else
    //    //    {
    //    //        currentTirePosition = i;
    //    //    }

    //    //}
    //    //remove points that are too close together for placing tire barriers via above array
    //    for (int i = PointIndexesToDelete.Count - 1; i >= 0; i--)
    //    {
    //        BarrierPoints.RemoveAt(PointIndexesToDelete[i]);
    //    }


    //    foreach (Vector2 pt in BarrierPoints)
    //    {
    //        GameObject barrier = Instantiate(InnerBarrier, InnerBarrierContainer.transform);
    //        barrier.transform.position = pt;
    //    }

    //    return BarrierPoints;
    //}
    /// <summary>
    /// expansionMultiplier must be positive value
    /// </summary>
    public void CreateBarriers(List<Vector2> currentRawPts, float barrierOffset, string innerOrOuter)
    {
        List<Vector2> passedData = new List<Vector2>(currentRawPts);
        List<Vector2> newData = new List<Vector2>();

        //ORIGINAL SOLUTION ---------------------------------------------------------------------------------------------------------------
        //passedData.RemoveAt(passedData.Count-1);
        //List<Vector2> AboveY = passedData.Where(c => c.y > 0).ToList();
        //List<Vector2> BelowY = passedData.Where(c => c.y < 0).ToList();

        ////stores the multiply or divide operator in a lambda depending on whether this is the inner or outer barrier
        //Func<float, float> UpperAction;
        //Func<float, float> LowerAction;

        //if (innerOrOuter.ToLower() == "inner")
        //{
        //    UpperAction = x => x - expansionMultiplier;
        //    LowerAction = x => x + expansionMultiplier;
        //}
        //else if (innerOrOuter.ToLower() == "outer")
        //{
        //    UpperAction = x => x + expansionMultiplier;
        //    LowerAction = x => x - expansionMultiplier;
        //}
        //else
        //{
        //    throw new Exception("variable innerOrOuter in CreateBarriers() is invalid. Check the passed string.");
        //}
        //if (expansionMultiplier < 0)
        //{
        //    throw new Exception("exceptionMultiplier cannot be negative!");
        //}

        ////manipulate all positive Y values
        //for (int i = 0; i < AboveY.Count; i++)
        //{
        //    AboveY[i] = new Vector2(AboveY[i].x, UpperAction(AboveY[i].y));
        //}
        ////manipulate all negative Y values

        //for (int i = 0; i < BelowY.Count; i++)
        //{
        //    BelowY[i] = new Vector2(BelowY[i].x, LowerAction(BelowY[i].y));
        //}
        ////sort all values in ascending X order
        //AboveY = AboveY.OrderBy(v => v.x).ToList();
        //BelowY = BelowY.OrderBy(v => v.x).ToList();

        ////move the outermost four points into position
        //Vector2 AboveLeft = AboveY.First();
        //Vector2 AboveRight = AboveY.Last();
        //Vector2 BelowLeft = BelowY.First();
        //Vector2 BelowRight = BelowY.Last();
        //int index;

        //index = AboveY.FindIndex(c => c == AboveLeft);
        //AboveY[index] = new Vector2(LowerAction(AboveLeft.x),AboveLeft.y);
        //index = AboveY.FindIndex(c => c == AboveRight);
        //AboveY[index]  = new Vector2(UpperAction(AboveRight.x), AboveRight.y);
        //index = BelowY.FindIndex(c => c == BelowLeft);
        //BelowY[index] = new Vector2(LowerAction(BelowLeft.x), BelowLeft.y);
        //index = BelowY.FindIndex(c => c == BelowRight);
        //BelowY[index] = new Vector2(UpperAction(BelowRight.x), BelowRight.y);



        ////reconstruct the new barrier points list
        //passedData.Clear();
        //passedData.AddRange(AboveY);
        //passedData.AddRange(BelowY);
        //passedData = mapCreator.SortPoints(passedData);
        //END ORIGINAL SOLUTION ---------------------------------------------------------------------------------------------------------------

        
        passedData.RemoveAt(passedData.Count - 1);
        float offset = barrierOffset;

        for (int i = 0; i < passedData.Count - 1; i++)
        {
            //points of lines logic
            int Aloc = i - 1;
            int Bloc = i;
            int Cloc = i + 1;
            if (i == 0)
            {
                Aloc = passedData.Count - 1;
            }
            if (i == passedData.Count - 1)
            {
                Cloc = 0;
            }
            //points were working with on this iteration
            Vector2 ptA = new Vector2(passedData[Aloc].x, passedData[Aloc].y);
            Vector2 ptB = new Vector2(passedData[Bloc].x, passedData[Bloc].y);
            Vector2 ptC = new Vector2(passedData[Bloc].x, passedData[Bloc].y);
            Vector2 ptD = new Vector2(passedData[Cloc].x, passedData[Cloc].y);

            //calculate normal unit vectors
            Vector2 lineANormal;
            Vector2 lineBNormal;
            if (innerOrOuter.ToLower() == "inner")
            {
                lineANormal = (ptB - ptA);
                lineANormal = new Vector2(-lineANormal.y, lineANormal.x).normalized;
                lineBNormal = (ptD - ptC);
                lineBNormal = new Vector2(-lineBNormal.y, lineBNormal.x).normalized;
            }
            else if (innerOrOuter.ToLower() == "outer")
            {
                lineANormal = (ptB - ptA);
                lineANormal = new Vector2(lineANormal.y, -lineANormal.x).normalized;
                lineBNormal = (ptD - ptC);
                lineBNormal = new Vector2(lineBNormal.y, -lineBNormal.x).normalized;
            }
            else
            {
                throw new Exception("variable innerOrOuter in CreateBarriers() is invalid. Check the passed string.");
            }

            //move these original points in the desired direction along the normal
            ptA = ptA + lineANormal * offset;
            ptB = ptB + lineANormal * offset;
            ptC = ptC + lineBNormal * offset;
            ptD = ptD + lineBNormal * offset;

            //slopes of my two lines (m in y= Mx +B)
            float mAB = (ptB.y - ptA.y) / (ptB.x - ptA.x);
            float mCD = (ptD.y - ptC.y) / (ptD.x - ptC.x);

            //y offset (B in y = Mx +B)
            float bAB = (ptA.y - mAB*ptA.x);
            float bCD = (ptC.y - mCD*ptC.x);


            //my desired x and y!
            float x = mCD * ptC.x + bCD - bAB;
            float y = mAB * x + bAB;
            Vector2 RESULT = new Vector2(x,y);
            newData.Add(RESULT);
        }
        //close the loop
        if (newData[0] != newData[newData.Count - 1])
        {
            newData.Add(newData[0]);
        }



        //converts our newly adjusted raw points into mesh!
        newData = mapCreator.CreateControlPoints(newData);
        newData = mapCreator.CreateTrackPoints(newData,Data.BarrierMeshPointFrequency);
        mapCreator.CreateOrSetMeshHelperObjects(newData);
        mapCreator.RotateTrackObjectsAlongCurves(Data.CurrentMeshHelperObjects);

        //this shouldnt be here, needs refactoring...
        Data.Curr_InnerTrackPoints.Clear();
        Data.Curr_OuterTrackPoints.Clear();
        mapCreator.CreateTrackMesh(Data.CurrentMeshHelperObjects, Data.BarrierThickness, this.GetComponent<MeshFilter>());
        
        mapCreator.CreateColliderForTrack(Data.Curr_OuterTrackPoints,Data.Curr_InnerTrackPoints, Data.BarrierColliderResolution, this.GetComponent<PolygonCollider2D>());
    }
}