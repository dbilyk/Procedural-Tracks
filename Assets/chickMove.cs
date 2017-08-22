using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chickMove : MonoBehaviour {

    public Animator Anim;
    private Rigidbody2D RB;
    public GameObject ChickSplat;
    public ParticleSystem bloodSpatter;
	// Use this for initialization
	void Start () {
        Anim = gameObject.GetComponent<Animator>();
        RB = gameObject.GetComponent<Rigidbody2D>();
        bloodSpatter = gameObject.GetComponent<ParticleSystem>();
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

        


	}
    IEnumerator ChickJump()
    {
        RB.AddRelativeForce(-Vector2.up * 0.5f,ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        jumpStarted = false;
    }


    void OnTriggerEnter2D(Collider2D Col)
    {
        if (Col.gameObject.tag == "Player")
        {
            GameObject bloodDecal = GameObject.Instantiate(ChickSplat);
            bloodDecal.transform.position = gameObject.transform.position;
            bloodSpatter.Play();
            gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        }
    }

}
