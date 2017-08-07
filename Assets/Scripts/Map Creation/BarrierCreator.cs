using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BarrierCreator : MonoBehaviour {

    public MapCreator mapCreator;

    
    /// <summary>
    /// barrierOffset must be a positive value. 
    /// </summary>
    public List<Vector2> CreateOutline(List<Vector2> currentRawPts, float barrierOffset, string innerOrOuter)
    {
        List<Vector2> passedData = new List<Vector2>(currentRawPts);
        List<Vector2> newData = new List<Vector2>();
        
        passedData.RemoveAt(passedData.Count - 1);
        float offset = barrierOffset;

        for (int i = 0; i < passedData.Count; i++)
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
                lineANormal = new Vector2(lineANormal.y, -lineANormal.x).normalized;
                lineBNormal = (ptD - ptC);
                lineBNormal = new Vector2(lineBNormal.y, -lineBNormal.x).normalized;
            }
            else if (innerOrOuter.ToLower() == "outer")
            {
                lineANormal = (ptB - ptA);
                lineANormal = new Vector2(-lineANormal.y, lineANormal.x).normalized;
                lineBNormal = (ptD - ptC);
                lineBNormal = new Vector2(-lineBNormal.y, lineBNormal.x).normalized;
            }
            else
            {
                throw new Exception("variable innerOrOuter in CreateBarriers() is invalid. Check the passed string.");
            }

            //move these original points in the desired direction along the normal
            ptA = ptA + (lineANormal * offset);
            ptB = ptB + (lineANormal * offset);
            ptC = ptC + (lineBNormal * offset);
            ptD = ptD + (lineBNormal * offset);
            
            //slopes of my two lines (m in y= Mx +B)
            float mAB = (ptB.y - ptA.y) / (ptB.x - ptA.x);
            float mCD = (ptD.y - ptC.y) / (ptD.x - ptC.x);

            //y offset (B in y = Mx +B)
            float bAB = (ptA.y - mAB*ptA.x);
            float bCD = (ptC.y - mCD*ptC.x);


            //my desired x and y
            float x = (bCD - bAB) / (mAB - mCD);
            float y = mAB * x + bAB;
            Vector2 RESULT = new Vector2(x,y);
            newData.Add(RESULT);
            
        }
        //newData = mapCreator.CheckControlPointAngles(newData,0.1f);
        //newData = mapCreator.RemovePointsTooClose(newData,1);
        //close the loop
        if (newData[0] != newData[newData.Count - 1])
        {
            newData.Add(newData[0]);
        }
        return newData;
        //converts our newly adjusted raw points into mesh!
        }

    public void CreateBarrier(List<Vector2> barrierRawPointData)
    {
        barrierRawPointData = mapCreator.CreateControlPoints(barrierRawPointData);
        barrierRawPointData = mapCreator.CreateTrackPoints(barrierRawPointData, Data.BarrierMeshPointFrequency);
        mapCreator.CreateOrSetMeshHelperObjects(barrierRawPointData);
        mapCreator.RotateTrackObjectsAlongCurves(Data.CurrentMeshHelperObjects);

        //this shouldnt be here, needs refactoring...
        Data.Curr_InnerTrackPoints.Clear();
        Data.Curr_OuterTrackPoints.Clear();
        mapCreator.CreateTrackMesh(Data.CurrentMeshHelperObjects, Data.BarrierThickness, this.GetComponent<MeshFilter>());

        mapCreator.CreateColliderForTrack(Data.Curr_OuterTrackPoints, Data.Curr_InnerTrackPoints, Data.BarrierColliderResolution, this.GetComponent<PolygonCollider2D>());

    }



}