using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackTriggerLogic : MonoBehaviour {
    public GameObject player;
    public CarMovement CarMovement;

    private float StartingTractionForce;
    private float StartingSpeed;
    private float StartingBrakes;
    private float StartingSteeringResponse;

    void Start()
    {
        StartingTractionForce = CarMovement.MaxTractionForce;
        StartingSpeed = CarMovement.MaxSpeed;
        StartingBrakes = CarMovement.MaxBrake;
        StartingSteeringResponse = CarMovement.steeringResponse;


    }
	void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject == player )
        {
            CarMovement.MaxTractionForce = StartingTractionForce;
            CarMovement.MaxSpeed = StartingSpeed;
            CarMovement.MaxBrake = StartingBrakes;
            CarMovement.steeringResponse = StartingSteeringResponse;


        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject == player)
        {
            CarMovement.MaxTractionForce /= 2;
            CarMovement.MaxSpeed /= 2;
            CarMovement.MaxBrake /= 2;
            CarMovement.steeringResponse /= 1.5f;
        }
    }
}
