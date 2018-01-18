using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public GameManager gameManager;
    public RaceStatsManager raceStatsManager;
    public LapTrigger lapTrigger;
    public GameObject StartScreen;

    //use this to pause game
    private float targetTimescale = 1f;

    public void StartRace () {
        StartScreen.SetActive (false);

        //gameManager.ResetGame ();
        // gameManager.GenerateNewTrackData ();
        //gameManager.GenerateLevel ();
        //gameManager.GenerateAI ();
        //gameManager.StartingCountdown ();
    }

    public void RestartLevel () {
        Time.timeScale = 1;
        //gameManager.ResetGame ();
        //gameManager.StartingCountdown ();
    }

    public void QuitRace () {
        Debug.Log ("Just called Quit race on UIManager, take me back to start screen?");

    }

    public void OpenPauseMenu () {
        targetTimescale = 0f;
        Time.timeScale = 0;

    }

    public void ClosePauseMenu () {
        targetTimescale = 1f;
        Time.timeScale = 1;
    }

    public User user;
    void Awake () {
        //wrong way blinker
        raceStatsManager.OnFacingWrongWay += FacingWrongWay;
        raceStatsManager.OnFacingForward += FacingForward;

    }

    //Wrong way flashing functionality----------   Must unsubscribe after game is done!
    public GameObject WrongWayIndicator;
    IEnumerator blinkReference;

    void FacingWrongWay () {
        blinkReference = blinkWrongWay ();
        StartCoroutine (blinkReference);
    }

    IEnumerator blinkWrongWay () {
        while (true) {
            yield return new WaitForSecondsRealtime (1f);
            if (WrongWayIndicator.activeSelf) {
                WrongWayIndicator.SetActive (false);
            } else {
                WrongWayIndicator.SetActive (true);
            }
        }
    }

    void FacingForward () {
        if (blinkReference != null) {
            StopCoroutine (blinkReference);
        }
        WrongWayIndicator.SetActive (false);
    }
    //--------------------------------------------------

}