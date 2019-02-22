using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingLight : MonoBehaviour {
    public GameObject MainCam;
    public Animation StartingLightsAnim;
    public GameManager gameManager;
    private bool animstarted;
    public AnimComplete animComplete;

    // Use this for initialization
    void OnEnable () {
        //gets called by the starting linght animation EVENT
        animComplete = new AnimComplete (AnimationCompleted);
        StartingLightsAnim.Play ();
        animstarted = true;
    }

    void Update () {
        gameObject.transform.position = new Vector3 (MainCam.transform.position.x, MainCam.transform.position.y, -1) + (MainCam.transform.up *2)+ (MainCam.transform.forward);
        gameObject.transform.rotation = MainCam.transform.localRotation;

    }

    void AnimationCompleted () {

        animstarted = false;
        if (OnStartingLightsComplete != null) {
            OnStartingLightsComplete ();
        }

    }
    public delegate void AnimComplete ();

    public delegate void StartingLightsComplete ();
    public event StartingLightsComplete OnStartingLightsComplete;
}