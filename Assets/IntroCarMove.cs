using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCarMove : MonoBehaviour {
    public List<WheelCollider> WheelCols = new List<WheelCollider>();
    public List<GameObject> VisualWheels = new List<GameObject>();

    // Use this for initialization
    void OnEnable () {
		
	}
	
    public void ApplyRotationToVisualWheels(WheelCollider collider, GameObject visualWheel)
    {
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }


	// Update is called once per frame
	void Update () {


        if (Input.GetKeyDown(KeyCode.Space))
        {
            WheelCols[0].motorTorque = -500f;
            WheelCols[1].motorTorque = -500f;



        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            WheelCols[0].motorTorque = 500f;
            WheelCols[1].motorTorque = 500f;

        }


        for (int i = 0; i < 4; i++)
        {
            ApplyRotationToVisualWheels(WheelCols[i], VisualWheels[i]);
        }


    }



}
