using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPolePositionData {
    private int _prevChkPtInd;
    public RaceStatsManager statsManager;
    public GameObject CarObject; //constant
    public int IndexInDataArray; //constant
    public bool FacingForward, AllowCheckpointUpdates;
    public int Curr_CheckpointIndex; //needs to be updated on schedule
    public int PrevCheckpointIndex {
        get {
            if (this.Curr_CheckpointIndex == 0) {
                _prevChkPtInd = statsManager.Curr_PoleCheckpoints.Count - 1;

            } else {
                _prevChkPtInd = this.Curr_CheckpointIndex - 1;
            }
            return _prevChkPtInd;
        }
    }
    public int LastValidCheckpointIndex,
    Curr_PolePosition, Curr_LapNumber,
    TotalCheckpointsPassedThisLap;

    public float Curr_LapStartTime, Curr_LapTime,
    LastLapTime = -1, FastestLapTime = -1;

}

public class RaceStatsManager : MonoBehaviour {

    public MapCreator mapCreator;
    public GameObject AIContainer;
    public GameObject StartingGridContainer;
    public GameObject Player;
    public float PctOfCheckpointsThatConstitutesALap = 80f;
    public int CheckpointFreq = 1;
    public CarPolePositionData PlayerPoleData = new CarPolePositionData ();

    [SerializeField]
    User user;

    public List<Vector2> Curr_PoleCheckpoints { get; private set; }
    public List<CarPolePositionData> CarPoleData { get; private set; }

    List<Vector2> Checkpoints = new List<Vector2> ();
    int TotalCheckpointsOnMap;

    bool hasInitialized = false;

    //call at the start of a race ot populate currentPoleData list
    void CurrentPoleDataInit () {
        CarPoleData = new List<CarPolePositionData> ();
        Curr_PoleCheckpoints = new List<Vector2> ();
        if (!hasInitialized) {
            Checkpoints = mapCreator.CreateTrackPoints (user.ActiveTrack.ControlPoints, CheckpointFreq);
            int closestChkptToStartingLine = ExtensionMethods.GetNearestInList (GameObject.FindGameObjectWithTag ("StartingLine").transform.position, Checkpoints);
            if (closestChkptToStartingLine != Checkpoints.Count - 1) {
                closestChkptToStartingLine += 1;
            } else {
                closestChkptToStartingLine = 0;
            }
            List<Vector2> RearrangeCheckpoints = new List<Vector2> ();

            for (int i = closestChkptToStartingLine; i < Checkpoints.Count; i++) {
                RearrangeCheckpoints.Add (Checkpoints[i]);
            }
            for (int i = 0; i < closestChkptToStartingLine; i++) {
                RearrangeCheckpoints.Add (Checkpoints[i]);

            }
            Curr_PoleCheckpoints = RearrangeCheckpoints;

            TotalCheckpointsOnMap = Checkpoints.Count;

            //populate player struct and push to list
            PlayerPoleData.CarObject = Player.gameObject;
            int playerNearestWPindex = ExtensionMethods.GetNearestInList (Player.transform.position, Checkpoints);
            PlayerPoleData.Curr_CheckpointIndex = playerNearestWPindex;
            //make sure we get both the current closest and WP before this one so that we can use that vector to make sure the player is driving the right way.
            PlayerPoleData.statsManager = this;
            PlayerPoleData.LastValidCheckpointIndex = 0;
            PlayerPoleData.Curr_LapNumber = 0;
            PlayerPoleData.TotalCheckpointsPassedThisLap = 0;
            PlayerPoleData.FacingForward = true;
            PlayerPoleData.IndexInDataArray = 0;
            PlayerPoleData.AllowCheckpointUpdates = true;
            CarPoleData.Add (PlayerPoleData);

            //AI struct creation
            for (int i = 0; i < AIContainer.transform.childCount; i++) {
                CarPolePositionData AIData = new CarPolePositionData ();
                AIData.CarObject = AIContainer.transform.GetChild (i).gameObject;
                AIData.statsManager = this;
                AIData.Curr_CheckpointIndex = ExtensionMethods.GetNearestInList (AIData.CarObject.transform.position, Checkpoints);

                AIData.LastValidCheckpointIndex = AIData.Curr_CheckpointIndex;
                AIData.Curr_LapNumber = 0;
                AIData.TotalCheckpointsPassedThisLap = 0;
                AIData.IndexInDataArray = i + 1;
                AIData.FacingForward = true;
                AIData.AllowCheckpointUpdates = true;

                CarPoleData.Add (AIData);

            }
            LapTrigger.OnLapComplete += LapComplete;
            hasInitialized = true;
        } else {
            //reset values of existing set of racestats classes
            PlayerPoleData = CarPoleData[0];
            int playerNearestWPindex = ExtensionMethods.GetNearestInList (Player.transform.position, Checkpoints);
            PlayerPoleData.Curr_CheckpointIndex = playerNearestWPindex;
            PlayerPoleData.LastValidCheckpointIndex = 0;
            PlayerPoleData.Curr_LapTime = 0;
            PlayerPoleData.Curr_LapNumber = 0;
            PlayerPoleData.TotalCheckpointsPassedThisLap = 0;
            PlayerPoleData.FacingForward = true;
            PlayerPoleData.IndexInDataArray = 0;
            PlayerPoleData.AllowCheckpointUpdates = true;

            for (int i = 0; i < AIContainer.transform.childCount; i++) {
                CarPolePositionData AIData = CarPoleData[i + 1];
                AIData.CarObject = AIContainer.transform.GetChild (i).gameObject;

                AIData.Curr_CheckpointIndex = ExtensionMethods.GetNearestInList (AIData.CarObject.transform.position, Checkpoints);

                AIData.LastValidCheckpointIndex = AIData.Curr_CheckpointIndex;
                AIData.Curr_LapNumber = 0;
                AIData.TotalCheckpointsPassedThisLap = 0;
                AIData.IndexInDataArray = i + 1;
                AIData.FacingForward = true;
                AIData.AllowCheckpointUpdates = true;
            }
        }

    }
    //private bool initComplete = false;
    private bool CheckFacingForward = false,
        UpdatePlayerCheckpts = false,
        UpdateAICheckpoints = false,
        UpdatePolePos = false,
        UpdateLapTimer = false;
    void OnEnable () {
        CurrentPoleDataInit ();

        CheckFacingForward = false;
        UpdatePlayerCheckpts = false;
        UpdateAICheckpoints = false;
        UpdatePolePos = false;
        UpdateLapTimer = false;

    }

    void Update () {
        //perioodically checking if player is facing forward
        if (!CheckFacingForward) {
            StartCoroutine ("GetFacingForward");
            CheckFacingForward = true;
        }
        //periodically recalculating player checkpoints
        if (!UpdatePlayerCheckpts) {
            StartCoroutine ("UpdatePlayerCheckpoints");
            UpdatePlayerCheckpts = true;
        }
        //recalc nearest AI WPs
        if (!UpdateAICheckpoints) {
            StartCoroutine ("UpdateOpponentCheckpoints");
            UpdateAICheckpoints = true;
        }

        if (!UpdatePolePos) {
            StartCoroutine ("UpdatePolePosition");
            UpdatePolePos = true;
        }
        if (!UpdateLapTimer && CarPoleData[0].Curr_LapNumber != 0) {
            StartCoroutine ("UpdateLapTime");
            UpdateLapTimer = true;
        }

    }

    public delegate void FacingWrongWay ();
    public event FacingWrongWay OnFacingWrongWay;
    public delegate void FacingForward ();
    public event FacingForward OnFacingForward;
    public delegate void LapAlert (int lapNumber);
    public event LapAlert OnLapAlert;

    IEnumerator GetFacingForward () {
        CarPolePositionData player = CarPoleData[0];
        List<Vector2> ChkPts = Curr_PoleCheckpoints;
        Vector2 playerVelocity = player.CarObject.GetComponent<Rigidbody2D> ().velocity;
        if (Vector2.Dot (playerVelocity, ChkPts[player.Curr_CheckpointIndex] - ChkPts[player.PrevCheckpointIndex]) > 0f) {
            //call delegate to turn off wrong way blinker the first time that this turns true
            if (!player.FacingForward) {
                if (OnFacingForward != null) OnFacingForward ();
            }
            player.FacingForward = true;
        } else {
            //call delegate to turn on Wrong Way blinker in UI MANAGER the first time that this is false
            if (player.FacingForward && playerVelocity.sqrMagnitude > 0.01f) {

                if (OnFacingWrongWay != null) OnFacingWrongWay ();
            }
            player.FacingForward = false;
            player.AllowCheckpointUpdates = false;

        }

        yield return new WaitForSeconds (0.1f);
        CheckFacingForward = false;
    }

    IEnumerator UpdatePlayerCheckpoints () {
        //player is index 0
        List<CarPolePositionData> carData = CarPoleData;
        CarPolePositionData player = carData[0];
        List<Vector2> ChkPts = Curr_PoleCheckpoints;
        //IF facing forward 

        if (!player.AllowCheckpointUpdates) {
            //check that the most recent checkpoint is the same as the last valid checkpoint
            if (player.Curr_CheckpointIndex == player.LastValidCheckpointIndex && player.FacingForward) {
                player.AllowCheckpointUpdates = true;
            } else {
                //update current nearest checkpoint
                player.Curr_CheckpointIndex = ExtensionMethods.GetNearestInList (player.CarObject.transform.position, Curr_PoleCheckpoints);
            }
        }

        if (player.AllowCheckpointUpdates) {
            player.Curr_CheckpointIndex = ExtensionMethods.GetNearestInList (player.CarObject.transform.position, Curr_PoleCheckpoints);
            if (player.Curr_CheckpointIndex != player.LastValidCheckpointIndex) {
                player.TotalCheckpointsPassedThisLap += 1;
            }

            //player.PrevCheckpointIndex = player.Curr_CheckpointIndex -1;
            player.LastValidCheckpointIndex = player.Curr_CheckpointIndex;

        }

        yield return new WaitForSeconds (0.1f);
        UpdatePlayerCheckpts = false;
    }

    IEnumerator UpdateOpponentCheckpoints () {
        for (int i = 1; i < CarPoleData.Count; i++) {
            CarPolePositionData CarData = CarPoleData[i];
            int nearestIndex = ExtensionMethods.GetNearestInList (CarData.CarObject.transform.position, Curr_PoleCheckpoints);
            if (nearestIndex != CarData.Curr_CheckpointIndex) {
                CarData.Curr_CheckpointIndex = nearestIndex;
                CarData.TotalCheckpointsPassedThisLap += 1;
            }

        }
        yield return new WaitForSeconds (0.1f);
        UpdateAICheckpoints = false;
    }

    IEnumerator UpdatePolePosition () {
        CarPolePositionData PlayerData = CarPoleData[0];
        int CarsAheadOfPlayer = 0;
        for (int i = 1; i < CarPoleData.Count; i++) {
            CarPolePositionData AIData = CarPoleData[i];
            if (AIData.Curr_LapNumber > PlayerData.Curr_LapNumber) {
                CarsAheadOfPlayer += 1;
            } else if (AIData.Curr_LapNumber == PlayerData.Curr_LapNumber && AIData.Curr_CheckpointIndex > PlayerData.Curr_CheckpointIndex) {

                CarsAheadOfPlayer += 1;

            } else {
                Transform pPos = PlayerData.CarObject.transform;
                Transform aiPos = AIData.CarObject.transform;
                if (AIData.Curr_CheckpointIndex == PlayerData.Curr_CheckpointIndex && Vector2.Dot (pPos.position - aiPos.position, aiPos.right) < 0) {
                    CarsAheadOfPlayer += 1;

                }
            }
        }
        PlayerData.Curr_PolePosition = CarsAheadOfPlayer + 1;

        yield return new WaitForSeconds (0.1f);
        UpdatePolePos = false;
    }

    IEnumerator UpdateLapTime () {
        CarPoleData[0].Curr_LapTime = Time.time - CarPoleData[0].Curr_LapStartTime;
        yield return new WaitForSeconds (0.04f);
        UpdateLapTimer = false;
    }

    //this delegate subscriber is called each time a car passes the lap line.
    void LapComplete (int PoleDataIndex) {
        CarPolePositionData thisCar = CarPoleData[PoleDataIndex];
        //if the player completes lap, trigger UI event
        if (PoleDataIndex == 0) {
            if (OnLapAlert != null) OnLapAlert (thisCar.Curr_LapNumber + 1);
        }

        if (thisCar.Curr_LapNumber == 0) {
            thisCar.Curr_LapNumber += 1;
            thisCar.Curr_LapStartTime = Time.time;
        }

        if (thisCar.TotalCheckpointsPassedThisLap >= Curr_PoleCheckpoints.Count * (PctOfCheckpointsThatConstitutesALap / 100)) {
            if (thisCar.FastestLapTime == -1) {
                thisCar.FastestLapTime = thisCar.Curr_LapTime;
            }
            if (thisCar.Curr_LapTime < thisCar.FastestLapTime) {
                thisCar.FastestLapTime = thisCar.Curr_LapTime;
            }
            thisCar.LastLapTime = thisCar.Curr_LapTime;
            thisCar.TotalCheckpointsPassedThisLap = 0;

            thisCar.Curr_LapNumber += 1;
            thisCar.Curr_LapStartTime = Time.time;

        }

    }
}