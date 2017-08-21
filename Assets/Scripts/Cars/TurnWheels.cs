using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnWheels : MonoBehaviour {
    public CarMovement CarMovement;
    public InputManager input;
    public AIInputController AIInput;
    public float MaxRotation;
    public float LerpSpeed;
    public GameObject[] FrontWheels = new GameObject[2];
    
	// Update is called once per frame
	void Update () {
        Vector3 currentRotation;
        
        foreach (GameObject wheel in FrontWheels)
        {
            CarMovement.GetSteeringAngle();
            currentRotation = wheel.transform.localRotation.eulerAngles;
            float SteeringInput;
            if (gameObject.tag == "Player")
            {
                SteeringInput = input.GetSteering();

            }
            else
            {
                SteeringInput = AIInput.GetSteeringInput(AIInput.SteeringTarget);

            }
            float desiredRotation = SteeringInput * (MaxRotation - Mathf.Clamp(CarMovement.Velocity.sqrMagnitude * CarMovement.Velocity.sqrMagnitude, 0f, MaxRotation - (MaxRotation / 2)));
            //if (CarMovement.Velocity.sqrMagnitude > 5)
            //{
            //    currentRotation.z = Mathf.LerpAngle(currentRotation.z, SteeringInput *MaxRotation * Mathf.Clamp(SteeringAngle,0f,1), LerpSpeed*Time.deltaTime);
            //    wheel.transform.localEulerAngles = currentRotation;
            //}
            float movingDirection = Vector2.Dot(CarMovement.Velocity,transform.right);
            if (currentRotation.z != desiredRotation && movingDirection >0)
            {
                currentRotation.z = Mathf.LerpAngle(currentRotation.z, desiredRotation, LerpSpeed * Time.deltaTime);
                wheel.transform.localEulerAngles = currentRotation;

            }
            if (currentRotation.z != desiredRotation && movingDirection <= 0)
            {
                currentRotation.z = Mathf.LerpAngle(currentRotation.z, desiredRotation*0.4f, LerpSpeed * Time.deltaTime);
                wheel.transform.localEulerAngles = currentRotation;
            }
        }
    }
}
