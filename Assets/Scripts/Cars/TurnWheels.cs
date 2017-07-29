using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnWheels : MonoBehaviour {
    public CarMovement CarMovement;
    public float MaxRotation;
    public float LerpSpeed;
    public GameObject[] FrontWheels = new GameObject[2];
    
    private float SteeringAngle;
	// Update is called once per frame
	void Update () {
        SteeringAngle = CarMovement.GetSteeringAngle();
        Vector3 currentRotation;
        
        foreach (GameObject wheel in FrontWheels)
        {
            currentRotation = wheel.transform.localRotation.eulerAngles;
            if (CarMovement.FacingRelativeToVelocity == "PushRight")
            {
                currentRotation.z = Mathf.LerpAngle(currentRotation.z, MaxRotation * Mathf.Clamp(SteeringAngle,0,1), LerpSpeed*Time.deltaTime);
                wheel.transform.localEulerAngles = currentRotation;
            }
            else if (CarMovement.FacingRelativeToVelocity == "PushLeft")
            {
                currentRotation.z = Mathf.LerpAngle(currentRotation.z, -MaxRotation * Mathf.Clamp(SteeringAngle, 0, 1), LerpSpeed * Time.deltaTime);
                wheel.transform.localEulerAngles = currentRotation;
            }
            else
            {
                currentRotation.z = Mathf.LerpAngle(currentRotation.z, 0, LerpSpeed/2 * Time.deltaTime);
                wheel.transform.localEulerAngles = currentRotation;
            }
        }
    }
}
