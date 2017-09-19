using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour {
    public User user;
    public Rigidbody2D Player;
    public Camera cam;
    public List<Rigidbody> Bones = new List<Rigidbody>();
    public Animator AnimControl;
    public ParticleSystem BloodSplatter;
    bool animalHit = false;

    void OnEnable()
    {

        //reoptimizes hierarchy for animation playback when the object is reinstantiated from the pool
        Rigidbody[] ExposedBones = AnimControl.gameObject.transform.GetComponentsInChildren<Rigidbody>();
        if (animalHit == true)
        {
            AnimControl.enabled = true;
            gameObject.GetComponent<CircleCollider2D>().enabled = true;
            ParticleSystem.EmissionModule emmiter = BloodSplatter.emission;
            emmiter.enabled = false;
            for (int i = 0; i < ExposedBones.Length; i ++)
            {
                ExposedBones[i].isKinematic = true;

            }
        }

        string[] RBNames = new string[ExposedBones.Length];
        for (int i = 0; i < ExposedBones.Length; i++)
        {
            RBNames[i] = ExposedBones[i].gameObject.name;

        }
        AnimatorUtility.OptimizeTransformHierarchy(AnimControl.gameObject, RBNames);
        user = FindObjectOfType<User>();
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        animalHit = false;

    }

	void OnTriggerEnter2D(Collider2D col)
    {

        if (col.tag == "Player" || col.tag == "AI")
            
        {
            AnimatorUtility.DeoptimizeTransformHierarchy(AnimControl.gameObject);
            AnimControl.enabled = false;
        StartCoroutine("DelayedBlood");
            foreach (Rigidbody RB in Bones)
            {
                GameObject player = col.gameObject;
                RB.isKinematic = false;
                RB.AddForce(((gameObject.transform.position - (player.transform.position- player.transform.right)).normalized + new Vector3(0,0,0.5f)) * player.GetComponent<Rigidbody2D>().velocity.magnitude * 2, ForceMode.Impulse);
            }
            gameObject.GetComponent<CircleCollider2D>().enabled = false;

        }
        if (col.tag == "Player")
        {
            user.UserCurrency += 1;
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
        }
    }

    IEnumerator DelayedBlood()
    {
        yield return new WaitForSeconds(0.02f);
        animalHit = true;
        ParticleSystem.EmissionModule emmiter = BloodSplatter.emission;
        emmiter.enabled = true;
        BloodSplatter.Play();
        StartCoroutine(SetObjectState());


    }

    IEnumerator SetObjectState()
    {
        if (BloodSplatter.isPlaying)
        {
            ParticleSystem.Particle[] BloodParticles = new ParticleSystem.Particle[BloodSplatter.particleCount];
            BloodSplatter.GetParticles(BloodParticles);
            for (int i = 0; i < BloodParticles.Length; i++)
            {
                if (BloodParticles[i].startLifetime - BloodParticles[i].remainingLifetime < 0.05f)
                {
                    BloodParticles[i].velocity = Quaternion.Euler(0, 0, Random.Range(10, -10)) * Bones[0].GetComponent<Rigidbody>().velocity * Random.Range(5, 20);

                }
                

                BloodParticles[i].position = new Vector3(BloodParticles[i].position.x, BloodParticles[i].position.y, 0f);

            }
            BloodSplatter.SetParticles(BloodParticles, BloodParticles.Length);
            //setParticleVelocity = true;

        }

        yield return null;
        StartCoroutine(BakeDeadCritter());
    }

    IEnumerator BakeDeadCritter()
    {
        while (true)
        {
            //turn off emission when critter slows down
            if (animalHit && Bones[0].velocity.sqrMagnitude < 0.01f)
            {
                ParticleSystem.EmissionModule emmiter = BloodSplatter.emission;
                emmiter.enabled = false;
                
            }
            Vector3 AnimalPixelLoc = cam.WorldToScreenPoint(Bones[0].transform.position);
            if (animalHit && Bones[0].velocity.sqrMagnitude < 0.001f && AnimalPixelLoc.x > Screen.width + 20 || AnimalPixelLoc.x < -20 || AnimalPixelLoc.y > Screen.height + 20 || AnimalPixelLoc.y < -20)
            {
                gameObject.SetActive(false);
                break;
            }
        yield return new WaitForSeconds(1f);
        }
    }

}
