using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCarMove : MonoBehaviour {
    public List<WheelCollider> WheelCols = new List<WheelCollider>();
    public List<GameObject> VisualWheels = new List<GameObject>();

    public float initialCarBodyAccel;
    public Rigidbody CarBody;
    public bool ApplyAccel = false;
	
    public StartScreenController StartScreenCtrl;
    public void ApplyRotationToVisualWheels(WheelCollider collider, GameObject visualWheel)
    {
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }


    bool initialAccel = false;
	void Update () {
        if (ApplyAccel)
        {
            WheelCols[0].motorTorque = 100;
            WheelCols[1].motorTorque = 100;
           
            if (!initialAccel)
            {
                StartCoroutine(ForcePushCarBody());
                initialAccel = true;
            }


        }
        

        for (int i = 0; i < 4; i++)
        {
            ApplyRotationToVisualWheels(WheelCols[i], VisualWheels[i]);
        }


    }

    IEnumerator ForcePushCarBody()
    {
        CarBody.velocity = new Vector3(-initialCarBodyAccel,0,0);
        yield return new WaitForSeconds(1f);
        CarBody.constraints -= RigidbodyConstraints.FreezeRotationX;
    }
    bool FirstCollisionDone = false;
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name == "Intro Cow")
        {
            if (!FirstCollisionDone)
            {
                StartScreenCtrl.StartSlowMo = true;
                FirstCollisionDone = true;
            }
            else
            {
                StartScreenCtrl.CarCollidedWIthCow = true;

            }
        }
    }



}
