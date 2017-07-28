using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateSkids : MonoBehaviour {
    public CarMovement CarMovement;
    public GameObject SkidPrefab;
    public GameObject SkidContainer;
    public GameObject[] Tires = new GameObject[4];
    

    public float skidMinimumTractionForce;
    public float skidLifetime;
    public float skidThickness;


    private List<GameObject> CurrentSkidmarks = new List<GameObject>();
    private float CurrentTraction = 0;
   
    void Start()
    {
        SkidPrefab.GetComponent<TrailRenderer>().time = skidLifetime;
        SkidPrefab.GetComponent<TrailRenderer>().widthMultiplier = skidThickness;
    }

	
	// Update is called once per frame
	void Update () {

        CurrentTraction = CarMovement.GetTractionVector(CarMovement.MaxTractionForce).SqrMagnitude();
        if(CurrentTraction > skidMinimumTractionForce && CurrentSkidmarks.Count < 4)
        {
            for(int i = 0; i<Tires.Length; i ++)
            {
                GameObject skidmark = Instantiate(SkidPrefab, SkidContainer.transform);
                CurrentSkidmarks.Insert(i,skidmark);
                skidmark.transform.SetPositionAndRotation(Tires[i].transform.position,Tires[i].transform.rotation);
            }
        }

        if (CurrentTraction > skidMinimumTractionForce && CurrentSkidmarks.Count == 4)
        {
            for (int i = 0; i < Tires.Length; i++)
            {
                CurrentSkidmarks[i].transform.SetPositionAndRotation(Tires[i].transform.position, Tires[i].transform.rotation);
            }
        }
        else
        {
            CurrentSkidmarks.Clear();
        } 
        
    }
}
