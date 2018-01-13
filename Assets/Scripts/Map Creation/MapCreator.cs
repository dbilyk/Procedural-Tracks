using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapCreator : MonoBehaviour {
    float MapWidth { get; set; } = 80;
    float MapHeight { get; set; } = 80;
    float CornerLerpStep { get; } = 0.1f;
    int PtsPerQuad { get; } = 500;

    float PointSpacing { get; set; } = 3f;
    int CornerWidth { get; set; } = 125;

    List<GameObject> MeshHelpers = new List<GameObject> ();

    //Mesh Helper objects 
    public GameObject MeshHelperContainer;
    public GameObject MeshHelperObject;
    public GameObject StartingGridContainer;
    public List<GameObject> StartingOutlines = new List<GameObject> ();
    public GameObject ActiveGameTrack;

    // HELPER : Creates Random Points based on specs-----------------------------------------------------------------
    List<Vector2> CreateQuadrantPoints (int PointCt, float MapW, float MapH, Vector2 Quad) {
        List<Vector2> Points = new List<Vector2> ();

        float QuadWidth = MapW / 2;
        float QuadHeight = MapH / 2;

        for (int i = 0; i < PointCt; i++) {
            Points.Add (new Vector2 (Quad.x * Random.Range (0, QuadWidth), Quad.y * Random.Range (0, QuadHeight)));
        }
        return Points;
    }

    //HELPER to check angle of point B between points A,B, C returns answer in degrees.
    float AngleBetweenThreePoints (Vector2 p0, Vector2 p1, Vector2 p2) {
        float angleInDeg = Vector2.Angle (p0 - p1, p2 - p1);
        return angleInDeg;
    }

    //HELPER function that gives a point on a curve based on three control points and the T var (distance along the curve)
    Vector2 CalculateTrackPoint (Vector2 p0, Vector2 p1, Vector2 p2, float t) {
        float x = (1 - t) * (1 - t) * p0.x + 2 * (1 - t) * t * p1.x + t * t * p2.x;
        float y = (1 - t) * (1 - t) * p0.y + 2 * (1 - t) * t * p1.y + t * t * p2.y;
        Vector2 TrackPoint = new Vector2 (x, y);
        return TrackPoint;
    }

    //____________________________________________________________________________________________________________________________________________________

    //Sorts points into a circular sequence-----------------------------------------------------------------
    public List<Vector2> CreateRawUnsortedPoints () {
        List<Vector2> Points = new List<Vector2> ();
        Vector2 UR = new Vector2 (1, 1);
        Vector2 LR = new Vector2 (1, -1);
        Vector2 LL = new Vector2 (-1, -1);
        Vector2 UL = new Vector2 (-1, 1);

        List<Vector2> URL = CreateQuadrantPoints (Data.PtCtPerQuad, Data.MapWidth, Data.MapHeight, UR);
        List<Vector2> LRL = CreateQuadrantPoints (Data.PtCtPerQuad, Data.MapWidth, Data.MapHeight, LR);
        List<Vector2> LLL = CreateQuadrantPoints (Data.PtCtPerQuad, Data.MapWidth, Data.MapHeight, LL);
        List<Vector2> ULL = CreateQuadrantPoints (Data.PtCtPerQuad, Data.MapWidth, Data.MapHeight, UL);

        Points.AddRange (URL);
        Points.AddRange (LRL);
        Points.AddRange (LLL);
        Points.AddRange (ULL);

        return Points;

    }

    public List<Vector2> SortPoints (List<Vector2> anyPoints) {
        List<Vector2> pts = new List<Vector2> (anyPoints);
        //List<Vector2> ptsRemovalCopy = new List<Vector2>(anyPoints);
        List<int> indexesToRemove = new List<int> ();
        List<Vector2> URL = pts.Where (c => c.x > 0 && c.y > 0).ToList ();
        List<Vector2> LRL = pts.Where (c => c.x > 0 && c.y < 0).ToList ();
        List<Vector2> LLL = pts.Where (c => c.x < 0 && c.y < 0).ToList ();
        List<Vector2> ULL = pts.Where (c => c.x < 0 && c.y > 0).ToList ();

        List<Vector2> URLS = URL.OrderBy (c => c.x).ToList ();
        List<Vector2> LRLS = LRL.OrderByDescending (c => c.x).ToList ();
        List<Vector2> LLLS = LLL.OrderByDescending (c => c.x).ToList ();
        List<Vector2> ULLS = ULL.OrderBy (c => c.x).ToList ();

        pts.Clear ();
        pts.AddRange (URLS);
        pts.AddRange (LRLS);
        pts.AddRange (LLLS);
        pts.AddRange (ULLS);

        if (pts[pts.Count - 1] != pts[0]) {
            pts.Add (pts[0]);
        }
        return pts;

    }

    public List<Vector2> RemovePointsTooClose (List<Vector2> anyPoints, float spacing) {
        List<Vector2> pts = new List<Vector2> (anyPoints);

        bool noMoreConflicts = false;

        while (!noMoreConflicts) {

            int counter = 0;
            for (int i = pts.Count - 1; i >= 0; i--) {
                if (i == pts.Count - 1) {

                    if (Mathf.Abs (pts[i].x - pts[0].x) < spacing) {
                        pts.RemoveAt (i);
                        counter += 1;
                    }
                } else {
                    if (Mathf.Abs (pts[i].x - pts[i + 1].x) < spacing) {
                        pts.RemoveAt (i);
                        counter += 1;
                    }
                }
                if (i == 0 && counter == 0) {
                    noMoreConflicts = true;
                }

            }

        }

        if (pts[pts.Count - 1] != pts[0]) {
            pts.Add (pts[0]);
        }

        return pts;
    }

    //inserts midpoints into the optimized list.
    public List<Vector2> CreateControlPoints (List<Vector2> currentRawPts) {
        List<Vector2> AllBezierPts = new List<Vector2> (currentRawPts);

        for (int i = currentRawPts.Count - 2; i >= 0; i--) {
            Vector2 MidPoint = (currentRawPts[i] + currentRawPts[i + 1]) / 2;
            AllBezierPts.Insert (i + 1, MidPoint);
        }
        AllBezierPts.Add (AllBezierPts[1]);
        return AllBezierPts;
    }

    //Checks all angles between each three INITIAL control points before midpoint insertion, 
    //and if angle is greater than the minimum angle, 
    //move point B half way towards the midpoint between pt A and C, 
    //thereby increasing the angle.
    public List<Vector2> CheckControlPointAngles (List<Vector2> currentCtrlPts, float lerpStep) {
        List<Vector2> newPoints = new List<Vector2> (currentCtrlPts);

        for (int i = 0; i < newPoints.Count; i++) {
            //in degrees
            int indexA = i;
            int indexB = i + 1;
            int indexC = i + 2;

            if (i == newPoints.Count - 2) {
                indexC = 0;
            }
            if (i == newPoints.Count - 1) {
                indexB = 0;
                indexC = 1;
            }

            float Angle = AngleBetweenThreePoints (newPoints[indexA], newPoints[indexB], newPoints[indexC]);

            while (Angle < Data.MinCornerWidth) {
                Vector2 MidpointAC = new Vector2 ((newPoints[indexA].x + newPoints[indexC].x) / 2, (newPoints[indexA].y + newPoints[indexC].y) / 2);
                Vector2 MoveBTowardsAC = Vector2.Lerp (newPoints[indexB], MidpointAC, lerpStep);
                Vector2 originalB = newPoints[indexB];
                newPoints[indexB] = MoveBTowardsAC;
                Angle = AngleBetweenThreePoints (newPoints[indexA], newPoints[indexB], newPoints[indexC]);

            }

        }

        if (newPoints[newPoints.Count - 1] != newPoints[0]) {
            newPoints[newPoints.Count - 1] = newPoints[0];
        }

        return newPoints;
    }

    //creates the array of track points with a passed in point frequency and a list of control points.
    public List<Vector2> CreateTrackPoints (List<Vector2> ControlPts, float trackPtFreq) {
        List<Vector2> TrackPts = new List<Vector2> ();
        for (int j = 1; j < ControlPts.Count - 2; j += 2) {
            for (int i = 1; i <= trackPtFreq; i++) {
                float t = (float) i / trackPtFreq;
                Vector2 pt = CalculateTrackPoint (ControlPts[j], ControlPts[j + 1], ControlPts[j + 2], t);
                TrackPts.Add (pt);

            }
        }
        return TrackPts;
    }

    //cant find a use for this method whatsoever...
    //public List<Vector2> ShrinkData(List<Vector2> passedData, float XShrinkFactor, float YShrinkFactor)
    //{
    //    List<Vector2> shrunkData = new List<Vector2>(passedData);

    //    for (int i = 0; i < shrunkData.Count; i++)
    //    {
    //        shrunkData[i] = new Vector2(shrunkData[i].x / XShrinkFactor, shrunkData[i].y / YShrinkFactor);
    //    }
    //    return shrunkData;
    //}

    //applies the same rotation (random) to each point in the list
    public List<Vector2> ApplyRandomRotation (List<Vector2> points) {
        List<Vector2> pts = new List<Vector2> (points);
        int randomAngle = Random.Range (0, 180);
        for (int i = 0; i < pts.Count; i++) {
            pts[i] = Quaternion.Euler (0, 0, randomAngle) * (pts[i] - Vector2.zero);
        }

        return pts;

    }

    //creates mesh helpers if there aren't enough already instantiated, and positions them according to the data.
    public void CreateOrSetMeshHelperObjects (List<Vector2> trackPoints) {
        //creates the object pool from which to creat mesh if no pool exists
        if (Data.CurrentMeshHelperObjects == null) {
            Data.CurrentMeshHelperObjects = new List<GameObject> ();
            foreach (Vector2 pt in trackPoints) {
                GameObject point = Instantiate (MeshHelperObject, MeshHelperContainer.transform);
                point.transform.position = pt;
                Data.CurrentMeshHelperObjects.Add (point);

            }
        }
        //if pool exists...
        else {
            int ObjectsCt = Data.CurrentMeshHelperObjects.Count;
            int DataCt = trackPoints.Count;

            //make sure there are enough existing helpers in the pool, and if not,
            //create or destroy before repositioning objects in pool
            if (ObjectsCt < DataCt) {
                for (int i = 0; i < DataCt - ObjectsCt; i++) {
                    GameObject point = Instantiate (MeshHelperObject, MeshHelperContainer.transform);
                    Data.CurrentMeshHelperObjects.Add (point);
                }
            }

            if (ObjectsCt > DataCt) {
                for (int i = 0; i < ObjectsCt - DataCt; i++) {
                    Destroy (Data.CurrentMeshHelperObjects[Data.CurrentMeshHelperObjects.Count - 1]);
                    Data.CurrentMeshHelperObjects.RemoveAt (Data.CurrentMeshHelperObjects.Count - 1);
                }
            }

            for (int i = 0; i < DataCt; i++) {
                Data.CurrentMeshHelperObjects[i].transform.position = trackPoints[i];
            }
        }
    }

    //Track Mesh Helper: rotates all track objects to face towards the next point, thereby following the curvature of the bezier curves.
    public void RotateTrackObjectsAlongCurves (List<GameObject> TrackObjs) {
        for (int i = 0; i < TrackObjs.Count; i++) {
            if (i < TrackObjs.Count - 1) {
                Vector3 dir = TrackObjs[i + 1].transform.position - TrackObjs[i].transform.position;
                float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
                TrackObjs[i].transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
            } else {
                Vector3 dir = TrackObjs[0].transform.position - TrackObjs[i].transform.position;
                float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
                TrackObjs[i].transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
            }
        }
    }

    //Creates Track mesh
    public void CreateTrackMesh (List<GameObject> TPs, float Thickness, MeshFilter targetMeshFilter) {
        Mesh mesh = new Mesh ();
        List<Vector3> vertices = new List<Vector3> ();
        List<Vector3> normals = new List<Vector3> ();
        List<Vector2> UVs = new List<Vector2> ();
        List<int> indicies = new List<int> ();

        //mesh is created starting with the inner point, going to the corresponding outer point of the racetrack. 

        for (int i = 0; i < TPs.Count; i++) {
            //add lower point mesh data
            vertices.Add (TPs[i].transform.position + (TPs[i].transform.up * -Thickness));
            UVs.Add (new Vector2 ((float) (i) / TPs.Count, 0));
            normals.Add (Vector3.back);

            //add upper point mesh data
            vertices.Add (TPs[i].transform.position + (TPs[i].transform.up * Thickness));
            UVs.Add (new Vector2 ((float) (i) / TPs.Count, 1));
            normals.Add (Vector3.back);

            //adds vert data
            Data.Curr_InnerTrackPoints.Add (TPs[i].transform.position + (TPs[i].transform.up * -Thickness));
            Data.Curr_OuterTrackPoints.Add (TPs[i].transform.position + (TPs[i].transform.up * Thickness));

        }
        //add starting point to the end in order to close the seal the loop!
        vertices.Add (vertices[0]);
        UVs.Add (new Vector2 (1, 0));
        normals.Add (Vector3.back);

        vertices.Add (vertices[1]);
        UVs.Add (new Vector2 (1, 1));
        normals.Add (Vector3.back);

        for (int i = 3; i < vertices.Count; i++) {
            //add two triangles every time four verticies are created
            if (i % 2 != 0) {
                //triangle 1 vertex indicies
                indicies.Add (i - 3);
                indicies.Add (i - 2);
                indicies.Add (i);
                //triangle 2 vertex indicies
                indicies.Add (i - 3);
                indicies.Add (i);
                indicies.Add (i - 1);
            }
        }

        //populate current data with all mesh info for saving option later.
        Data.Curr_InnerTrackPoints.Add (vertices[0]);
        Data.Curr_InnerTrackPoints.Add (vertices[1]);
        Data.Curr_Verts = vertices;
        Data.Curr_Normals = normals;
        Data.Curr_UVs = UVs;
        Data.Curr_Indicies = indicies;

        mesh.vertices = vertices.ToArray ();
        mesh.uv = UVs.ToArray ();
        mesh.normals = normals.ToArray ();
        mesh.triangles = indicies.ToArray ();

        mesh.RecalculateBounds ();

        MeshFilter filter = targetMeshFilter.GetComponent<MeshFilter> ();
        if (filter != null) {
            filter.sharedMesh = mesh;

        }
    }
    //OVERLOAD for creating mesh with existing data
    public void CreateTrackMesh (List<Vector3> Vertices, List<Vector3> Normals, List<Vector2> UVs, List<int> Indicies, MeshFilter targetMeshFilter) {
        Mesh mesh = new Mesh ();

        mesh.vertices = Vertices.ToArray ();
        mesh.normals = Normals.ToArray ();
        mesh.uv = UVs.ToArray ();
        mesh.triangles = Indicies.ToArray ();

        mesh.RecalculateBounds ();

        MeshFilter filter = targetMeshFilter.GetComponent<MeshFilter> ();
        if (filter != null) {
            filter.sharedMesh = mesh;

        }

    }

    public void CreateTrackBerms (List<GameObject> TPs, float Thickness, float OffsetFromTrack, int LengthInPoints, MeshFilter targetMeshFilter) {
        List<GameObject> passedData = new List<GameObject> (TPs);
        List<float[]> CornerAngles = new List<float[]> ();
        Mesh mesh = new Mesh ();
        List<Vector3> vertices = new List<Vector3> ();
        List<Vector3> normals = new List<Vector3> ();
        List<Vector2> UVs = new List<Vector2> ();
        List<int> indicies = new List<int> ();

        //create corner angles list 
        for (int i = 0; i < Data.Curr_RawPoints.Count - 1; i++) {
            int ptA = i;
            int ptB = i + 1;
            int ptC = i + 2;
            if (i == Data.Curr_RawPoints.Count - 2) {
                ptC = 0;
            }

            float ang = AngleBetweenThreePoints (Data.Curr_RawPoints[ptA], Data.Curr_RawPoints[ptB], Data.Curr_RawPoints[ptC]);

            float angDir = ExtensionMethods.AngularDirection (Data.Curr_RawPoints[ptC] - Data.Curr_RawPoints[ptA], Data.Curr_RawPoints[ptB] - Data.Curr_RawPoints[ptA]);
            float[] val = new float[2] { ang, angDir };
            CornerAngles.Add (val);
        }
        //at this point, we have an array of corner data with corner width, and direction of each corner

        float prevCornerDirection = 0, currCornerDirection;
        for (int i = 0; i < passedData.Count - Data.MeshTrackPointFreq; i += (int) Data.MeshTrackPointFreq) {
            currCornerDirection = CornerAngles[i / (int) Data.MeshTrackPointFreq][1];
            //check if corner is narrow enough for decal, currently not doing anything....
            if (Mathf.Abs (CornerAngles[i / (int) Data.MeshTrackPointFreq][0]) < Data.MinCornerWidth + 100) {
                float sign;
                if (CornerAngles[i / (int) Data.MeshTrackPointFreq][1] < 0) {
                    sign = -1;
                } else {
                    sign = 1;
                }
                if (LengthInPoints > Data.MeshTrackPointFreq) {
                    LengthInPoints = (int) Data.MeshTrackPointFreq;
                }
                if (LengthInPoints % 2 != 0) {
                    LengthInPoints -= 1;
                }
                for (int j = ((int) Data.MeshTrackPointFreq - LengthInPoints) / 2; j <= LengthInPoints + (((int) Data.MeshTrackPointFreq - LengthInPoints) / 2); j += 2) {

                    //POPULATE VERTS and OTHER STUFF
                    if (j == ((int) Data.MeshTrackPointFreq - LengthInPoints) / 2) {
                        //check if the current corner is on the same side of track as previous corner
                        if (currCornerDirection > 0 && prevCornerDirection > 0 || currCornerDirection < 0 && prevCornerDirection < 0) {
                            vertices.Add (passedData[i + j].transform.position + (passedData[i + j].transform.up * sign * OffsetFromTrack));
                            vertices.Add (passedData[i + j].transform.position + (passedData[i + j].transform.up * sign * (OffsetFromTrack + Thickness)));
                        } else {
                            vertices.Add (passedData[i + j].transform.position + (passedData[i + j].transform.up * sign * OffsetFromTrack));
                            vertices.Add (passedData[i + j].transform.position + (passedData[i + j].transform.up * sign * (OffsetFromTrack + 0.05f)));
                        }
                        UVs.Add (new Vector2 (0, 0));
                        UVs.Add (new Vector2 (0, 1));

                        normals.Add (Vector3.back);
                        normals.Add (Vector3.back);
                    }
                    if (j != ((int) Data.MeshTrackPointFreq - LengthInPoints) / 2 && j != (LengthInPoints + ((int) Data.MeshTrackPointFreq - LengthInPoints) / 2)) {
                        vertices.Add (passedData[i + j].transform.position + (passedData[i + j].transform.up * sign * OffsetFromTrack));
                        vertices.Add (passedData[i + j].transform.position + (passedData[i + j].transform.up * sign * (OffsetFromTrack + Thickness)));
                        UVs.Add (new Vector2 (((float) j / (float) LengthInPoints), 0));
                        UVs.Add (new Vector2 (((float) j / (float) LengthInPoints), 1));
                        normals.Add (Vector3.back);
                        normals.Add (Vector3.back);

                        if (sign >= 0) {
                            //triangle 1 vertex indicies
                            indicies.Add (vertices.Count - 4);
                            indicies.Add (vertices.Count - 3);
                            indicies.Add (vertices.Count - 1);
                            //triangle 2 vertex indicies
                            indicies.Add (vertices.Count - 4);
                            indicies.Add (vertices.Count - 1);
                            indicies.Add (vertices.Count - 2);
                        } else {
                            //triangle 1 vertex indicies
                            indicies.Add (vertices.Count - 4);
                            indicies.Add (vertices.Count - 1);
                            indicies.Add (vertices.Count - 3);
                            //triangle 2 vertex indicies
                            indicies.Add (vertices.Count - 4);
                            indicies.Add (vertices.Count - 2);
                            indicies.Add (vertices.Count - 1);
                        }

                    }
                    if (j == (LengthInPoints + ((int) Data.MeshTrackPointFreq - LengthInPoints) / 2)) {
                        //check if the current corner is on the same side of track as next corner, dont put an endcap on
                        if (currCornerDirection > 0 && CornerAngles[(i / (int) Data.MeshTrackPointFreq) + 1][1] > 0 || currCornerDirection < 0 && CornerAngles[(i / (int) Data.MeshTrackPointFreq) + 1][1] < 0) {
                            vertices.Add (passedData[i + j].transform.position + (passedData[i + j].transform.up * sign * OffsetFromTrack));
                            vertices.Add (passedData[i + j].transform.position + (passedData[i + j].transform.up * sign * (OffsetFromTrack + Thickness)));
                        } else {
                            vertices.Add (passedData[i + j].transform.position + (passedData[i + j].transform.up * sign * OffsetFromTrack));
                            vertices.Add (passedData[i + j].transform.position + (passedData[i + j].transform.up * sign * (OffsetFromTrack + 0.05f)));
                        }
                        UVs.Add (new Vector2 (1, 0));
                        UVs.Add (new Vector2 (1, 1));

                        normals.Add (Vector3.back);
                        normals.Add (Vector3.back);

                        if (sign >= 0) {
                            //triangle 1 vertex indicies
                            indicies.Add (vertices.Count - 4);
                            indicies.Add (vertices.Count - 3);
                            indicies.Add (vertices.Count - 1);
                            //triangle 2 vertex indicies
                            indicies.Add (vertices.Count - 4);
                            indicies.Add (vertices.Count - 1);
                            indicies.Add (vertices.Count - 2);
                        } else {
                            //triangle 1 vertex indicies
                            indicies.Add (vertices.Count - 4);
                            indicies.Add (vertices.Count - 1);
                            indicies.Add (vertices.Count - 3);
                            //triangle 2 vertex indicies
                            indicies.Add (vertices.Count - 4);
                            indicies.Add (vertices.Count - 2);
                            indicies.Add (vertices.Count - 1);
                        }

                    }
                }
                prevCornerDirection = CornerAngles[i / (int) Data.MeshTrackPointFreq][1];
            }

        }

        mesh.vertices = vertices.ToArray ();
        mesh.uv = UVs.ToArray ();
        mesh.normals = normals.ToArray ();
        mesh.triangles = indicies.ToArray ();

        mesh.RecalculateBounds ();

        MeshFilter filter = targetMeshFilter;
        if (filter != null) {
            filter.sharedMesh = mesh;
        }
    }

    public void CreateColliderForTrack (List<Vector2> outerColliderPath, List<Vector2> innerColliderPath, int resolution, PolygonCollider2D targetCollider) {
        List<Vector2> OuterColliderPath = new List<Vector2> ();
        List<Vector2> InnerColliderPath = new List<Vector2> ();
        PolygonCollider2D col = targetCollider.GetComponent<PolygonCollider2D> ();
        for (int i = 0; i < outerColliderPath.Count; i += resolution) {
            OuterColliderPath.Add (outerColliderPath[i]);
        }
        for (int i = 0; i < innerColliderPath.Count; i += resolution) {
            InnerColliderPath.Add (innerColliderPath[i]);
        }
        //to make sure the collider completes full circle
        OuterColliderPath.Add (outerColliderPath[0]);
        InnerColliderPath.Add (innerColliderPath[0]);
        //set collider paths at index 0 and 1 to our new point lists (paths)
        col.SetPath (0, OuterColliderPath.ToArray ());
        col.SetPath (1, InnerColliderPath.ToArray ());

    }

    public List<Vector2> CreateRacingLinePoints (List<Vector2> currentRawPts, float WaypointFreq, float LerpTightness) {

        //plots control points with angles shrunk to try and create a racing line.
        List<Vector2> RacingLine = new List<Vector2> (currentRawPts);
        //move starting corner towards apex
        Vector2 MpAC = new Vector2 ((RacingLine[RacingLine.Count - 2].x + RacingLine[1].x) / 2, (RacingLine[RacingLine.Count - 2].y + RacingLine[1].y) / 2);
        RacingLine[0] = Vector2.Lerp (RacingLine[0], MpAC, LerpTightness);

        //move the apex of each B control point towards the midpoint of the line between AC. (shifts the racing line towards the midpoint of each corner)
        for (int i = 0; i < RacingLine.Count - 2; i++) {
            Vector2 MidpointAC = new Vector2 ((RacingLine[i].x + RacingLine[i + 2].x) / 2, (RacingLine[i].y + RacingLine[i + 2].y) / 2);
            RacingLine[i + 1] = Vector2.Lerp (RacingLine[i + 1], MidpointAC, LerpTightness);

        };

        //ceate midpoints for modified Data.Curr_RawPoints
        List<Vector2> midpoints = CreateControlPoints (RacingLine);
        //create Data.Curr_TrackPoints out of new data
        List<Vector2> trkpts = CreateTrackPoints (midpoints, WaypointFreq);

        return trkpts;
    }
    /// <summary>
    /// Must happen before MeshHelpers are reset to something else...
    /// </summary>
    public void CreateStartingGrid (List<GameObject> TrackMeshHelpers, float gridLength, float gridWidth, int numberOfPositions) {
        List<GameObject> passedData = new List<GameObject> (TrackMeshHelpers);
        Data.CarStartingPositions = new List<GameObject> ();
        int randomStartingPointIndex = Random.Range ((int) Data.MeshTrackPointFreq * 2, (int) passedData.Count - (int) (Data.MeshTrackPointFreq * 2));
        bool firstLoop = true;

        Vector2 CurrentGridPairCenterpoint = passedData[randomStartingPointIndex - (int) Data.MeshTrackPointFreq / 6].transform.position;

        bool reachedTargetPosQty = false;
        for (int i = randomStartingPointIndex - (int) Data.MeshTrackPointFreq / 6; i > 0; i--) {
            GameObject innerGO;
            GameObject outerGO;
            if ((CurrentGridPairCenterpoint - (Vector2) passedData[i].transform.position).sqrMagnitude > gridLength || firstLoop) {
                for (int j = 0; j < numberOfPositions; j++) {
                    if (StartingOutlines[j].gameObject.activeSelf == false && StartingOutlines[j + 1].gameObject.activeSelf == false) {
                        innerGO = StartingOutlines[j].gameObject;
                        outerGO = StartingOutlines[j + 1].gameObject;
                        Vector2 innerPos = passedData[i].transform.position + (passedData[i].transform.up * -gridWidth);
                        Vector2 outerPos = passedData[i].transform.position + (passedData[i].transform.up * gridWidth);
                        Quaternion rotation = passedData[i].transform.rotation;

                        innerGO.transform.position = (Vector3) innerPos + new Vector3 (0, 0, -0.001f);
                        outerGO.transform.position = (Vector3) outerPos + new Vector3 (0, 0, -0.001f);
                        innerGO.transform.rotation = rotation;
                        outerGO.transform.rotation = rotation;

                        innerGO.SetActive (true);
                        Data.CarStartingPositions.Add (innerGO);
                        if (Data.CarStartingPositions.Count == numberOfPositions) { reachedTargetPosQty = true; break; }

                        outerGO.SetActive (true);
                        Data.CarStartingPositions.Add (outerGO);
                        if (Data.CarStartingPositions.Count == numberOfPositions) { reachedTargetPosQty = true; break; }

                        CurrentGridPairCenterpoint = passedData[i].transform.position;

                        break;
                    }
                }
            }

            if (reachedTargetPosQty) {
                break;
            }
            firstLoop = false;
        }

        GameObject startingLine = StartingGridContainer.transform.Find ("StartingLine").gameObject;
        startingLine.SetActive (true);
        Data.StartingLine = startingLine;
        Data.StartingLine.transform.position = passedData[randomStartingPointIndex].transform.position + new Vector3 (0, 0, -0.001f);
        Data.StartingLine.transform.rotation = passedData[randomStartingPointIndex].transform.rotation;

    }

}