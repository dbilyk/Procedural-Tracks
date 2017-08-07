using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentCreator : MonoBehaviour
{
    /*
     * inner mountain creation
     * VARS:
     * Inner Mountain Curve;
     * Mountain centerpoint 1
     * Mountain Centerpoint 2
     * 
     * 
     * UR smallest X point
     * LR smallest X point
     * set of three points on left of this line
     * set of three points on the right of this line
     * 
     * OuterMountainCurveUpper;
     * OuterMountainCurveLower;
     * 
     * 
     * 1) get EVENLY spaced curve data of X point freq for slightly inside inner barrier.
       2) find midpoint between (0,0) and the smallest and largest x points of the above data. These are the two peaks.
       3) get first point in UR and last point in LR quadrants, and three points on either side of this line.  these are for the mountain sides.
       4) create mesh that winds around the two center points to form two mountain tops
       5) optionally get points along the mesh edges to create a second mesh to color the mountain top with snow.

        6) get evenly spaced data for upper and lower bottom of the mountain at 2X the frequency of the inner data.
        7) create mesh between inner and top outer data. (this is the surface the road is "paved" on)
        8) create mesh between upper and lower outer curves.

     */


}
