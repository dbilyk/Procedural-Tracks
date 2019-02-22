using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowCam : MonoBehaviour {
    public GameObject PositionTarget, RotationTarget;

    public float PosLerpRate;
    public float RotLerpRate;
    private Transform PosTrans, RotTrans;
    private Rigidbody2D playerRB;
    public float YOffset = 1f;
    public float MinVertOffset = -18;
    public float MaxVertOffset = -25;
    public Vector3 CurrentDesiredPosition;
	// Use this for initialization
	void Start () {
        PosTrans = PositionTarget.transform;
        RotTrans = RotationTarget.transform;
        playerRB = PositionTarget.gameObject.GetComponentInParent<Rigidbody2D>();
        CurrentDesiredPosition = new Vector3(PosTrans.position.x, PosTrans.position.y, Mathf.Clamp(-playerRB.velocity.sqrMagnitude, -MaxVertOffset, -MinVertOffset));

    }

    //Update is called once per frame
    void FixedUpdate () {
        CurrentDesiredPosition = new Vector3(PosTrans.position.x, PosTrans.position.y, Mathf.Clamp(-playerRB.velocity.sqrMagnitude/10, -MaxVertOffset, -MinVertOffset));

        transform.position = Vector3.Lerp(transform.position, CurrentDesiredPosition, PosLerpRate  * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, RotTrans.rotation, RotLerpRate * Time.fixedDeltaTime);
        //transform.position = Vector3.Lerp(transform.position, transform.position * ((PlayerTrans.right * Vector3.Cross(Player.transform.right, playerRB.velocity).z).sqrMagnitude /4),Time.fixedDeltaTime);
        //transform.position = new Vector3(transform.position.x,transform.position.y, Mathf.Clamp(-playerRB.velocity.sqrMagnitude, -10,-5));
    }
}
