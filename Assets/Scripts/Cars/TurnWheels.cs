using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnWheels : MonoBehaviour {
    public CarMovement CarMovement;
    public InputManager input;
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
            CarMovement.GetSteeringAngle();
            currentRotation = wheel.transform.localRotation.eulerAngles;
            float SteeringInput = input.GetSteering();
            float desiredRotation = SteeringInput * (MaxRotation - Mathf.Clamp(CarMovement.Velocity.sqrMagnitude * CarMovement.Velocity.sqrMagnitude, 0f, MaxRotation - (MaxRotation / 2)));
            //if (CarMovement.Velocity.sqrMagnitude > 5)
            //{
            //    currentRotation.z = Mathf.LerpAngle(currentRotation.z, SteeringInput *MaxRotation * Mathf.Clamp(SteeringAngle,0f,1), LerpSpeed*Time.deltaTime);
            //    wheel.transform.localEulerAngles = currentRotation;
            //}
            if (currentRotation.z != desiredRotation)
            {
                currentRotation.z = Mathf.LerpAngle(currentRotation.z, desiredRotation, LerpSpeed * Time.deltaTime);
                wheel.transform.localEulerAngles = currentRotation;

            }
        }
    }
}
