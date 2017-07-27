using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowCam : MonoBehaviour {
    public GameObject Player;

    public float PosLerpRate;
    public float RotLerpRate;
    private Transform PlayerTrans;
    public float CameraOffset;
	// Use this for initialization
	void Start () {
        PlayerTrans = Player.transform;
        
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        
        transform.position = Vector3.Lerp(transform.position, PlayerTrans.position + PlayerTrans.right* CameraOffset, PosLerpRate  * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, PlayerTrans.rotation* Quaternion.Euler(0,0,-90), RotLerpRate * Time.fixedDeltaTime);
	}
}
