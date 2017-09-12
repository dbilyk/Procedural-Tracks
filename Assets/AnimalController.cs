using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour {
    public Rigidbody2D Player;
    public List<Rigidbody> Bones = new List<Rigidbody>();
    public Animator AnimControl;
    public ParticleSystem BloodSplatter;
	void OnTriggerEnter2D(Collider2D col)
    {

        if (col.tag == "Player" || col.tag == "AI")
            AnimControl.enabled = false;
        StartCoroutine("DelayedBlood");
        {
            foreach (Rigidbody RB in Bones)
            {
                GameObject player = col.gameObject;
                RB.isKinematic = false;
                RB.AddForce(((gameObject.transform.position - player.transform.position).normalized + new Vector3(0,0,1f)) * player.GetComponent<Rigidbody2D>().velocity.magnitude*1.3f, ForceMode.Impulse);
            }
            gameObject.GetComponent<CircleCollider2D>().enabled = false;

        }
    }

    IEnumerator DelayedBlood()
    {
        yield return new WaitForSeconds(0.02f);
        BloodSplatter.Play();
       


    }

    
    void FixedUpdate()
    {
        Vector3 AnimalVelocity = Bones[0].GetComponent<Rigidbody>().velocity;

        if (BloodSplatter.isPlaying)
        {
            ParticleSystem.Particle[] BloodParticles = new ParticleSystem.Particle[BloodSplatter.particleCount];
            BloodSplatter.GetParticles(BloodParticles);
            for (int i = 0; i < BloodParticles.Length; i++)
            {
                if (BloodParticles[i].startLifetime - BloodParticles[i].remainingLifetime <0.05f)
                {
                    BloodParticles[i].velocity= Quaternion.Euler(0, 0, Random.Range(40, -40)) * Bones[0].GetComponent<Rigidbody>().velocity* Random.Range(5,20);

                }
                //turn off emission when critter slows down
                if(Bones[0].velocity.sqrMagnitude < 0.5f)
                {
                    ParticleSystem.EmissionModule emmiter = BloodSplatter.emission;
                    emmiter.enabled = false;
                }

                BloodParticles[i].position = new Vector3(BloodParticles[i].position.x, BloodParticles[i].position.y, -0.01f);
                
            }
            BloodSplatter.SetParticles(BloodParticles, BloodParticles.Length);
            //setParticleVelocity = true;

        }
    }

}
