using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chickMove : MonoBehaviour {

    public Animator Anim;
    private Rigidbody2D RB;
    public GameObject ChickSplat;
    public GameObject BloodSplatterGO;
    public ParticleSystem bloodSpatter;
	// Use this for initialization
	void Start () {
        Anim = gameObject.GetComponent<Animator>();
        RB = gameObject.GetComponent<Rigidbody2D>();
        bloodSpatter.Stop();
	}
	
    bool jumpStarted = false;
	// Update is called once per frame
	void Update () {
        if (!jumpStarted)
        {
            StartCoroutine("ChickJump");
            jumpStarted = true;
            Anim.SetBool("Jump",true);
        }
        if (collided == true && !bloodSpatter.isPlaying)
        {
            Destroy(gameObject);
        }

                   


	}
    IEnumerator ChickJump()
    {
        RB.AddRelativeForce(-Vector2.up * 0.0001f,ForceMode2D.Impulse);
        yield return new WaitForSeconds(1.292f);
        jumpStarted = false;
    }

    bool collided = false;
    void OnCollisionEnter2D(Collision2D Col)
    {
        if (Col.gameObject.tag == "Player" || Col.gameObject.tag == "AI")
        {
            //GameObject bloodDecal = GameObject.Instantiate(ChickSplat);
            //bloodDecal.transform.position = gameObject.transform.position;
            //gameObject.transform.position += new Vector3(0,0,0.1f);
            BloodSplatterGO.transform.rotation = Quaternion.Euler(BloodSplatterGO.transform.eulerAngles.x, BloodSplatterGO.transform.eulerAngles.y, Col.transform.eulerAngles.z -45);
            BloodSplatterGO.transform.SetParent(null);
            bloodSpatter.Play();
            collided = true;
            gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            
        }
    }

}
