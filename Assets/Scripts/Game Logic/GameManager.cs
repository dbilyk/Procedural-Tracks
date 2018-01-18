using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    //vars
    public float CountdownLength = 2;

    //script objects
    public SmoothFollowCam FollowCam;

    //game object
    public GameObject RaceStatsManager,
    MobManager,
    Player,
    newAI,
    ActiveGameTrack,
    BermDecals,
    AIContainer,
    StartingGridContainer,
    FoliageContainer,
    GameLoopUI,
    MiniMapGroup,
    StartingLights;

    public List<AIInputController> AIInputs = new List<AIInputController> ();

    void Awake () {
        Application.targetFrameRate = 60;
    }

    //small helper to toggle AI input
    private void SetAIInput (bool isActive) {
        for (int i = 0; i < AIInputs.Count; i++) {
            AIInputs[i].enabled = isActive;
        }
    }

    void GenerateAI () {
        //creates a new AI opponent
        for (int i = 0; i < Data.CarStartingPositions.Count - 1; i++) {
            GameObject Ai = Instantiate (newAI, AIContainer.transform);
            Ai.transform.position = Data.CarStartingPositions[i].transform.position;
            Ai.transform.rotation = Data.CarStartingPositions[i].transform.rotation;
            AIInputController aiInput = Ai.GetComponent<AIInputController> ();
            aiInput.enabled = false;
            AIInputs.Add (aiInput);
        }
    }

    void StartingCountdown () {

        //positions player/AIs
        Player.transform.position = Data.CarStartingPositions[Data.CarStartingPositions.Count - 1].transform.position;
        Player.transform.rotation = Data.CarStartingPositions[Data.CarStartingPositions.Count - 1].transform.rotation;
        Player.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
        Player.GetComponent<CarMovement> ().enabled = false;
        //disable UI before start of race
        GameLoopUI.SetActive (false);

        StartCoroutine ("StartRace");
        Vector3 CamStartPosition = new Vector3 (Player.transform.position.x - 5, Player.transform.position.y, -3);
        Quaternion CamStartRotation = Quaternion.Euler (0, 100, 0);
        FollowCam.gameObject.transform.position = CamStartPosition;
        FollowCam.gameObject.transform.rotation = CamStartRotation;
        InvokeRepeating ("StartingCam", 0, 0.02f);
    }

    public void StartingCam () {
        GameObject cam = FollowCam.gameObject;
        cam.transform.position = Vector3.Lerp (cam.transform.position, new Vector3 (Player.transform.position.x, Player.transform.position.y, -18), 0.05f);
        cam.transform.rotation = Quaternion.Lerp (cam.transform.rotation, Quaternion.Euler (Player.transform.rotation.eulerAngles.x, Player.transform.rotation.eulerAngles.y, Player.transform.rotation.eulerAngles.z - 90), 0.05f);
    }

    IEnumerator StartRace () {
        StartingLights.SetActive (true);
        yield return new WaitForSeconds (CountdownLength);
        CancelInvoke ("StartingCam");
        FollowCam.enabled = true;

        //must activate before GameloopUI
        RaceStatsManager.SetActive (true);

        //Enable gameloop UI
        GameLoopUI.SetActive (true);
        MiniMapGroup.SetActive (true);

        //enables AI input
        SetAIInput (true);
        //enables player movement
        Player.GetComponent<CarMovement> ().enabled = true;

        //Start Spawning Critters
        MobManager.SetActive (true);

    }

    //destroys stuff that gets recreated in StartNewGame
    public void ResetGame () {
        StopCoroutine ("StartRace");
        StartingLights.SetActive (false);
        SetAIInput (false);
        //setting these false will re-trigger Initialization in their respective OnEnable functions
        GameLoopUI.SetActive (false);
        RaceStatsManager.SetActive (false);

        //places AI back on starting grid
        for (int i = 0; i < AIContainer.transform.childCount; i++) {
            GameObject AI = AIContainer.transform.GetChild (i).gameObject;

            AI.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
            AI.transform.position = StartingGridContainer.transform.GetChild (i).transform.position;
            AI.transform.rotation = StartingGridContainer.transform.GetChild (i).transform.rotation;
        }
        MobManager.SetActive (false);

    }

    void NewRace () {
        ResetGame ();

    }

}