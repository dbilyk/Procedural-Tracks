using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCar : MonoBehaviour {
    public GameObject target;
    public bool rotatesWithTarget;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.position = (Vector2)target.transform.position;
        if (rotatesWithTarget)
        {
            gameObject.transform.rotation = target.transform.rotation * Quaternion.Euler(0,90,-90);
        }
	}
}
