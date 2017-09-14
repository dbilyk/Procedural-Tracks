using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//GARBAGE!!! NEEDS TO BE COMPLETELY REWRITTEN................................................................
public class BodyRoll : MonoBehaviour {
    public CarMovement carMovement;
    public GameObject CarBody;
    bool rolling = false;
    float startingZValue;

    void Start()
    {
        startingZValue = CarBody.transform.rotation.eulerAngles.z;
    }

    // Update is called once per frame
	void Update () {
        if (!rolling)
        {
            StartCoroutine("BodyRoller");
            rolling = true;
        }
	}

    IEnumerator BodyRoller()
    {   
        yield return new WaitForSeconds(0.1f);
        float steeringAngle = carMovement.GetSteeringAngle();
        Debug.Log(steeringAngle);
        if (steeringAngle > 0.005f)
        {
            CarBody.transform.localRotation = Quaternion.Euler(CarBody.transform.localRotation.eulerAngles.x, CarBody.transform.localRotation.eulerAngles.y, startingZValue - (steeringAngle* 20));
        }
        if (steeringAngle < -0.005f)
        {
            CarBody.transform.localRotation = Quaternion.Euler(CarBody.transform.localRotation.eulerAngles.x, CarBody.transform.localRotation.eulerAngles.y, startingZValue + (steeringAngle * 20));
        }

        rolling = false;
    }
}
