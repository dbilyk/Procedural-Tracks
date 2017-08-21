using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowCam : MonoBehaviour {
    public GameObject Player;

    public float PosLerpRate;
    public float RotLerpRate;
    private Transform PlayerTrans;
    private Rigidbody2D playerRB;
    public float CameraOffset;
    
	// Use this for initialization
	void Start () {
        PlayerTrans = Player.transform;
        playerRB = Player.gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        
        transform.position = Vector3.Lerp(transform.position, new Vector3(PlayerTrans.position.x, PlayerTrans.position.y, Mathf.Clamp(-playerRB.velocity.sqrMagnitude/3, -12, -8))/*+ PlayerTrans.right* CameraOffset*/, PosLerpRate  * Time.fixedDeltaTime);
        //transform.position = new Vector3(transform.position.x,transform.position.y, Mathf.Clamp(-playerRB.velocity.sqrMagnitude, -10,-5));
        transform.rotation = Quaternion.Lerp(transform.rotation, PlayerTrans.rotation* Quaternion.Euler(0,0,-90), RotLerpRate * Time.fixedDeltaTime);
	}
}
