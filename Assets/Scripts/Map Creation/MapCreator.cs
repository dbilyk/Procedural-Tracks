using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour {
    //Mesh Helper objects 
    public GameObject MeshHelperContainer;
    public GameObject PointObject;
    public List<GameObject> MeshHelperObjects;
    
    // HELPER : Creates Random Points based on specs-----------------------------------------------------------------
    List<Vector2> CreateQuadrantPoints(int PointCt, float MapW, float MapH, Vector2 Quad)
    {
        List<Vector2> Points = new List<Vector2>();

        float QuadWidth = MapW / 2;
        float QuadHeight = MapH / 2;

        for (int i = 0; i < PointCt; i++)
        {
            Points.Add(new Vector2(Quad.x * Random.Range(0, QuadWidth), Quad.y * Random.Range(0, QuadHeight)));
        }
        return Points;
    }

    //HELPER to check angle of point B between points A,B, C returns answer in degrees.
    float AngleBetweenThreePoints(Vector2 p0, Vector2 p1, Vector2 p2)
    {
        float angleInDeg = Vector2.Angle(p0 - p1, p2 - p1);
        return angleInDeg;
    }

    //HELPER function that gives a point on a curve based on three control points and the T var (distance along the curve)
    Vector2 CalculateTrackPoint(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        float x = (1 - t) * (1 - t) * p0.x + 2 * (1 - t) * t * p1.x + t * t * p2.x;
        float y = (1 - t) * (1 - t) * p0.y + 2 * (1 - t) * t * p1.y + t * t * p2.y;
        Vector2 TrackPoint = new Vector2(x, y);
        return TrackPoint;
    }
    
    //____________________________________________________________________________________________________________________________________________________

    
    //Sorts points into a circular sequence-----------------------------------------------------------------
    public List<Vector2> CreateRawPoints()
    {
        List<Vector2> Points = new List<Vector2>();
        Vector2 UR = new Vector2(1, 1);
        Vector2 LR = new Vector2(1, -1);
        Vector2 LL = new Vector2(-1, -1);
        Vector2 UL = new Vector2(-1, 1);

        List<Vector2> URL = CreateQuadrantPoints(Data.PtCtPerQuad, Data.MapWidth, Data.MapHeight, UR);
        List<Vector2> LRL = CreateQuadrantPoints(Data.PtCtPerQuad, Data.MapWidth, Data.MapHeight, LR);
        List<Vector2> LLL = CreateQuadrantPoints(Data.PtCtPerQuad, Data.MapWidth, Data.MapHeight, LL);
        List<Vector2> ULL = CreateQuadrantPoints(Data.PtCtPerQuad, Data.MapWidth, Data.MapHeight, UL);

        List<Vector2> URLS = URL.OrderBy(c => c.x).ToList();
        List<Vector2> LRLS = LRL.OrderByDescending(c => c.x).ToList();
        List<Vector2> LLLS = LLL.OrderByDescending(c => c.x).ToList();
        List<Vector2> ULLS = ULL.OrderBy(c => c.x).ToList();

        Points.AddRange(URLS);
        Points.AddRange(LRLS);
        Points.AddRange(LLLS);
        Points.AddRange(ULLS);
        Points.Add(Points[0]);
       
        //remove points that are too close to properly draw a mesh
        for (int i = Points.Count - 3; i > 0; i--)
        {
            if (Mathf.Abs(Points[i].x - Points[i + 2].x)< Data.PointSpacing)
            {
                Points.RemoveAt(i+1);
            }

        }
       
        return Points;

    }

    //inserts midpoints into the optimized list.
    public List<Vector2> CreateControlPoints(List<Vector2> currentRawPts)
    {
        List<Vector2> AllBezierPts = new List<Vector2>(currentRawPts);

        for (int i = currentRawPts.Count -2; i >=0; i--)
        {
            Vector2 MidPoint = (currentRawPts[i] + currentRawPts[i+1])/2;
            AllBezierPts.Insert(i+1, MidPoint);
        }
        AllBezierPts.Add(AllBezierPts[1]);
        return AllBezierPts;
    }

    //Checks all angles between each three INITIAL control points before midpoint insertion, 
    //and if angle is greater than the minimum angle, 
    //move point B half way towards the midpoint between pt A and C, 
    //thereby increasing the angle.
    public List<Vector2> CheckControlPointAngles(List<Vector2> currentCtrlPts)
    {
        List<Vector2> newPoints = new List<Vector2>(currentCtrlPts);
        
        for (int i = 0; i<newPoints.Count-2; i++)
        {
            //in degrees
            float Angle = AngleBetweenThreePoints(newPoints[i], newPoints[i+1], newPoints[i+2]);
            
            while (Angle < Data.MinCornerWidth) {
                Vector2 MidpointAC = new Vector2((newPoints[i].x + newPoints[i + 2].x) / 2, (newPoints[i].y + newPoints[i + 2].y) / 2);
                Vector2 MidpointBToMidpointAC = new Vector2((newPoints[i + 1].x + MidpointAC.x) / 2, (newPoints[i + 1].y + MidpointAC.y) / 2);
                Vector2 originalB = newPoints[i + 1];
                newPoints[i + 1] = MidpointBToMidpointAC;
                Angle = AngleBetweenThreePoints(newPoints[i], newPoints[i + 1], newPoints[i + 2]);
                
           
            }

        }

        return newPoints;
    }

    //creates the array of track points with a passed in point frequency and a list of control points.
    public List<Vector2> CreateTrackPoints(List<Vector2> ControlPts, float trackPtFreq)
    {
        List<Vector2> TrackPts = new List<Vector2>();
        for (int j = 1; j < ControlPts.Count -2; j += 2)
        {
            for (int i = 1; i <= trackPtFreq; i++)
            {
                float t = (float)i / trackPtFreq;
                Vector2 pt = CalculateTrackPoint(ControlPts[j], ControlPts[j + 1], ControlPts[j + 2], t);
                TrackPts.Add(pt);

            }
        }
        return TrackPts;
    }

    //calls all the functions to actually generate the data that the above functions work with, and returns an array of all the track gameobject pieces.
    
    //Creates Track mesh
    public void CreateTrackMesh(List<GameObject> TPs)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> UVs = new List<Vector2>();
        List<int> indicies = new List<int>();

        //mesh is created starting with the inner point, going to the corresponding outer point of the racetrack. 
       
        for (int i = 0; i < TPs.Count; i++)
        {
            //add lower point mesh data
            vertices.Add(TPs[i].transform.GetChild(0).transform.position + (TPs[i].transform.GetChild(0).transform.up*-Data.TrackMeshThickness));
            UVs.Add(new Vector2((float)(i) / TPs.Count, 0));
            normals.Add(Vector3.back);
            
            //add upper point mesh data
            vertices.Add(TPs[i].transform.GetChild(1).transform.position + (TPs[i].transform.GetChild(0).transform.up * Data.TrackMeshThickness));
            UVs.Add(new Vector2((float)(i) / TPs.Count, 1));
            normals.Add(Vector3.back);
        }
        //add starting point to the end in order to close the seal the loop!
        vertices.Add(vertices[0]);
        UVs.Add(new Vector2(1,0));
        normals.Add(Vector3.back);

        vertices.Add(vertices[1]);
        UVs.Add(new Vector2(1, 1));
        normals.Add(Vector3.back);

        for (int i = 3; i < vertices.Count; i++)
        {

            //add two triangles every time four verticies are created
            if (i % 2 != 0)
            {
                //triangle 1 vertex indicies
                indicies.Add(i - 3);
                indicies.Add(i - 2);
                indicies.Add(i);
                //triangle 2 vertex indicies
                indicies.Add(i - 3);
                indicies.Add(i);
                indicies.Add(i - 1);

            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.uv = UVs.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = indicies.ToArray();


        mesh.RecalculateBounds();
        
        MeshFilter filter = GetComponent<MeshFilter>();
        if (filter != null)
        {
            filter.sharedMesh = mesh;

        }

        //Generate polygonCollider While were at it!

        List<Vector2> OuterColliderPath = new List<Vector2>();
        List<Vector2> InnerColliderPath = new List<Vector2>();
        PolygonCollider2D col = GetComponent<PolygonCollider2D>();
        if (Data.TrackColliderResolution%2 != 0)
        {
            Data.TrackColliderResolution += 1;
        }
        //create list of outer points at a specified sampling frequency
        for(int i = 1; i < vertices.Count-1; i += Data.TrackColliderResolution)
        {
            OuterColliderPath.Add(vertices[i]);
        }
        for (int i = 0; i < vertices.Count - 1; i += Data.TrackColliderResolution)
        {
            InnerColliderPath.Add(vertices[i]);
        }
        //set collider paths at index 0 and 1 to our new point lists (paths)
        col.SetPath(0, OuterColliderPath.ToArray());
        col.SetPath(1, InnerColliderPath.ToArray());


    }

    //Track Mesh Helper: rotates all track objects to face towards the next point, thereby following the curvature of the bezier curves.
    public void RotateTrackObjectsAlongCurves(List<GameObject> TrackObjs)
    {
        for (int i = 0; i < TrackObjs.Count; i++)
        {
            if (i < TrackObjs.Count - 1)
            {
                Vector3 dir = TrackObjs[i + 1].transform.position - TrackObjs[i].transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                TrackObjs[i].transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            else
            {
                Vector3 dir = TrackObjs[0].transform.position - TrackObjs[i].transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                TrackObjs[i].transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }


    public List<Vector2> CreateRacingLinePoints(List<Vector2> currentRawPts, float WaypointFreq, float LerpTightness)
    {

        //plots control points with angles shrunk to try and create a racing line.
        List<Vector2> RacingLine = new List<Vector2>(currentRawPts);
        
        //move starting corner towards apex
        Vector2 MpAC = new Vector2((RacingLine[RacingLine.Count - 2].x + RacingLine[1].x) / 2, (RacingLine[RacingLine.Count - 2].y + RacingLine[1].y) / 2);
        RacingLine[0] = Vector2.Lerp(RacingLine[0], MpAC, LerpTightness);
        
        //move the apex of each B control point towards the midpoint of the line between AC. (shifts the racing line towards the midpoint of each corner)
        for (int i = 0; i < RacingLine.Count -2; i++)
        {
            float angle = AngleBetweenThreePoints(RacingLine[i], RacingLine[i + 1], RacingLine[i + 2]);
            Vector2 MidpointAC = new Vector2((RacingLine[i].x + RacingLine[i + 2].x) / 2, (RacingLine[i].y + RacingLine[i + 2].y) / 2);
            RacingLine[i + 1] = Vector2.Lerp(RacingLine[i + 1], MidpointAC, LerpTightness);

        };

        //ceate midpoints for modified Data.Curr_RawPoints
        List<Vector2> midpoints = CreateControlPoints(RacingLine);
        //create Data.Curr_TrackPoints out of new data
        List<Vector2> trkpts = CreateTrackPoints(midpoints, WaypointFreq);
        

        //visualize racingline
        
        // DebugPlot(trkpts, new Color32(0, 250, 0, 255));


        return trkpts;
    }
    
}
