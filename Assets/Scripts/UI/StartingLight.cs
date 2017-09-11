using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingLight : MonoBehaviour {
    public GameObject MainCam;
    public Animation StartingLightsAnim;
    public GameManager gameManager;
    private bool animstarted;

	// Use this for initialization
	void OnEnable()
    {
        StartingLightsAnim.Play();
        animstarted = true;
    }
    void Update()
    {
        gameObject.transform.position = new Vector3(MainCam.transform.position.x, MainCam.transform.position.y, -1) + MainCam.transform.up;
        gameObject.transform.rotation = MainCam.transform.localRotation;

        if(animstarted && !StartingLightsAnim.isPlaying){
            gameObject.SetActive(false);
            animstarted = false;
        }
    }
	
}
