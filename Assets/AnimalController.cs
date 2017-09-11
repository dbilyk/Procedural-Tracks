using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour {
    public List<Rigidbody> Bones = new List<Rigidbody>();
	
	void OnTriggerEnter2D(Collider2D col)
    {

        if (col.tag == "Player" || col.tag == "AI")
        {
            foreach (Rigidbody RB in Bones)
            {
                GameObject player = col.gameObject;
                RB.AddForce(((gameObject.transform.position - player.transform.position).normalized + new Vector3(0,0,1f)) * player.GetComponent<Rigidbody2D>().velocity.magnitude, ForceMode.Impulse);
            }
            gameObject.GetComponent<CircleCollider2D>().enabled = false;

        }
        Debug.Log("here");
    }

}
