using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingLineCreator{
  User user;
  List<Vector2> CornerMidPoints = new List<Vector2>();
  List<Vector2> RacingLine;

  int racingLineFreq = 10; 

  List<float> fDisplacement = new List<float>();

  float trackWidth;
  
  bool isLooped = true;
  
  //number of times the racing line algo is run to get the line.
  int nIterations = 35;
	
	//set from 0-1, it controls t parameterization in CatmulRom. (chordal, centripetal, uniform)
	public float alpha = 0.5f;
	

  //privately get the data needed to generate racing line based on trackPoints data.
  void GetCornerMidpoints(Track track){
    List<Vector2> result = new List<Vector2>();
    //walk along the track points data at the freqency it was generated at, and grab the start middle and end of each curve 
    
    for(int i = 1;i<track.TrackPoints.Count;i+=track.TrackPtFrequency/2){
      result.Add(track.TrackPoints[i]);     
      fDisplacement.Add(0);
    }
    CornerMidPoints = result;
    
  }  

  //get track thickness from passed track data (used for racing line clamping)
  void GetTrackWidth(Track t){
    trackWidth = t.TrackWidth /28;//yes, that's a magic number divisor to make the mystery racing line formula work
  }

  List<Vector2> GenerateSplinePoints(List<Vector2> controlPts){
    List<Vector2> result = new List<Vector2>();

    for(int i = 0; i<controlPts.Count;i++){
      int A = (i+controlPts.Count-1) % controlPts.Count;
      int B = i;
      int C = (i+1) % controlPts.Count;
      int D = (i+2) % controlPts.Count;
      //generates all points at the racing line freq
      result.AddRange(CatmulRom(controlPts[A],controlPts[B],controlPts[C],controlPts[D]));
      
    }

    return result;
  }

//this is used by map renderer to generate the racing line
  public List<Vector2> createRacingLine(Track track){
    GetCornerMidpoints(track);
    GetTrackWidth(track);
    RacingLineCalc();
    RacingLine = GenerateSplinePoints(CornerMidPoints);
    return RacingLine;
  }

  //returns all points on the curve at racingLineFreq frequency.
	List<Vector2> CatmulRom(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
	{
    List<Vector2> result = new List<Vector2>();
		float t0 = 0.0f;
		float t1 = GetT(t0, p0, p1);
		float t2 = GetT(t1, p1, p2);
		float t3 = GetT(t2, p2, p3);

		for(float t=t1; t<t2; t+=((t2-t1)/(float)racingLineFreq))
		{
		    Vector2 A1 = (t1-t)/(t1-t0)*p0 + (t-t0)/(t1-t0)*p1;
		    Vector2 A2 = (t2-t)/(t2-t1)*p1 + (t-t1)/(t2-t1)*p2;
		    Vector2 A3 = (t3-t)/(t3-t2)*p2 + (t-t2)/(t3-t2)*p3;
		    
		    Vector2 B1 = (t2-t)/(t2-t0)*A1 + (t-t0)/(t2-t0)*A2;
		    Vector2 B2 = (t3-t)/(t3-t1)*A2 + (t-t1)/(t3-t1)*A3;
		    
		    Vector2 C = (t2-t)/(t2-t1)*B1 + (t-t1)/(t2-t1)*B2;
		    
		    result.Add(C);
		}
    return result;
	}
//helper to parameterize time used by catmul
	float GetT(float t, Vector2 p0, Vector2 p1)
	{
	    float a = Mathf.Pow((p1.x-p0.x), 2.0f) + Mathf.Pow((p1.y-p0.y), 2.0f);
	    float b = Mathf.Pow(a, 0.5f);
	    float c = Mathf.Pow(b, alpha);
	
	    return (c + t);
	}

//used by racing line calc
Vector2 GetSplineGradient(float t)
	{
		int p0, p1, p2, p3;
		if (!isLooped)
		{
			p1 = (int)t + 1;
			p2 = p1 + 1;
			p3 = p2 + 1;
			p0 = p1 - 1;
		}
		else
		{
			p1 = ((int)t) % CornerMidPoints.Count;
			p2 = (p1 + 1) % CornerMidPoints.Count;
			p3 = (p2 + 1) % CornerMidPoints.Count;
			p0 = p1 >= 1 ? p1 - 1 : CornerMidPoints.Count - 1;
		}

		t = t - (int)t;

		float tt = t * t;
		float ttt = tt * t;

		float q1 = -3.0f * tt + 4.0f*t - 1.0f;
		float q2 = 9.0f*tt - 10.0f*t;
		float q3 = -9.0f*tt + 8.0f*t + 1.0f;
		float q4 = 3.0f*tt - 2.0f*t;

		float tx = 0.5f * (CornerMidPoints[p0].x * q1 + CornerMidPoints[p1].x * q2 + CornerMidPoints[p2].x * q3 + CornerMidPoints[p3].x * q4);
		float ty = 0.5f * (CornerMidPoints[p0].y * q1 + CornerMidPoints[p1].y * q2 + CornerMidPoints[p2].y * q3 + CornerMidPoints[p3].y * q4);

		return new Vector2(tx,ty);
	}

//moves the original data points around to produce control points for catmull racing line.
void RacingLineCalc(){
  for (int n = 0; n < nIterations; n++)
    {
      for (int i = 0+n; i < CornerMidPoints.Count+n; i++)
      {
        int ii = i % CornerMidPoints.Count;
        // Get locations of neighbour nodes
        Vector2 pointRight = CornerMidPoints[(ii + 1) % CornerMidPoints.Count];
        Vector2 pointLeft = CornerMidPoints[(ii + CornerMidPoints.Count - 1) % CornerMidPoints.Count];
        Vector2 pointMiddle = CornerMidPoints[ii];

        // Create vectors to neighbours
        Vector2 vectorLeft = new Vector2(pointLeft.x - pointMiddle.x, pointLeft.y - pointMiddle.y);
        Vector2 vectorRight = new Vector2(pointRight.x - pointMiddle.x, pointRight.y - pointMiddle.y);

        // Normalise neighbours
        Vector2 leftn = vectorLeft.normalized;
        Vector2 rightn = vectorRight.normalized; 

        // Add together to create bisector vector
        Vector2 vectorSum = leftn + rightn;
        float len = Mathf.Sqrt(vectorSum.x*vectorSum.x + vectorSum.y*vectorSum.y);
        vectorSum.x /= len; vectorSum.y /= len;

        // Get point gradient and normalise
        Vector2 g = GetSplineGradient(ii);
        float glen = Mathf.Sqrt(g.x*g.x + g.y*g.y);
        g.x /= glen; g.y /= glen;

        // Project required correction onto point tangent to give displacment
        float dp = -g.y*vectorSum.x + g.x * vectorSum.y;

        // Shortest path
        fDisplacement[ii] += (dp * 0.003f);

        // Curvature
        fDisplacement[(ii + 1) % CornerMidPoints.Count] += dp * -0.001f;
        fDisplacement[(ii - 1 + CornerMidPoints.Count) % CornerMidPoints.Count] += dp * -0.001f;

      }

      // Clamp displaced points to track width
      for (int i = 0; i < CornerMidPoints.Count; i++)
      {
        if (fDisplacement[i] >= trackWidth) fDisplacement[i] = trackWidth;
        if (fDisplacement[i] <= -trackWidth) fDisplacement[i] = -trackWidth;
        Vector2 g = GetSplineGradient(i);
        float glen = Mathf.Sqrt(g.x*g.x + g.y*g.y);
        g.x /= glen; g.y /= glen;

        CornerMidPoints[i] = new Vector2(CornerMidPoints[i].x + -g.y * fDisplacement[i],CornerMidPoints[i].y + g.x * fDisplacement[i]);
      }
    }
  }
}
//https://github.com/OneLoneCoder/videos/blob/master/OneLoneCoder_RacingLines.cpp