using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_GameLoop : MonoBehaviour {
	Animator Anim;

	[SerializeField]
	User user;

	[SerializeField]
	RaceStatsManager statsManager;
	CritterMobManager critterManager;

	//these should all be updated through events 
	//BUT MUST BE UNSUBBED AFTER ANY EXIT CONDITION.!
	[SerializeField]
	Text BestLapBigTxt, BestLapSmlTxt,
	LastLapBigTxt, LastLapSmlTxt,
	CurrLapTxt, LapsInRaceTxt,
	CurrPlaceTxt, PlacesInRaceTxt,
	CurrLapTimeBigTxt, CurrLapTimeSmallTxt,
	CoinScoreTxt,
	NewCurrLapAlert, LapsInRaceAlert;

	//event sender
	[SerializeField]
	Button PauseBtn;

	[SerializeField]
	StartingLight startingLight;

	[SerializeField]
	GameObject WrongWayIndicator;

	[SerializeField]
	InputManager inputManager;
	[SerializeField]
	RaceStatsManager raceStatsManager;
	CarPolePositionData playerRaceData;

	public delegate void ButtonClick ();
	public event ButtonClick OnClickPause;

	bool RaceStarted;

	void OnEnable () {
		UpdateUI = false;
		Anim = gameObject.GetComponent<Animator> ();
		startingLight.OnStartingLightsComplete += BeginRace;
		user.OnGameLoopCoinsAdded += updateCoinScore;

		//wrong way blinker
		raceStatsManager.OnFacingWrongWay += FacingWrongWay;
		raceStatsManager.OnFacingForward += FacingForward;

		PauseBtn.onClick.AddListener (delegate { ClickPause (); });
		raceStatsManager.OnLapAlert += TriggerLapAlert;
	}

	List<string> BaseStates = new List<string> () {
		"GameloopScreenIn"

	};

	List<string> Layer1 = new List<string> () {
		"NewLapAlert"

	};

	void PlayAnim (int statesIndex, int layer) {
		List<string> animLayer = new List<string> ();
		switch (layer) {
			case 0:
				animLayer = BaseStates;
				break;
			case 1:
				animLayer = Layer1;
				break;
		}
		string anim = animLayer[statesIndex];
		Anim.Play (anim, layer);
	}

	void OnDisable () {
		raceStatsManager.OnLapAlert -= TriggerLapAlert;
	}

	bool UpdateUI;
	void Update () {
		if (!UpdateUI && RaceStarted) {
			StartCoroutine ("UpdateUIStats");
			UpdateUI = true;
		}
	}

	//pause event sender
	void ClickPause () {
		Debug.Log ("pauseBtn");
		if (OnClickPause != null) {
			OnClickPause ();
		}
	}

	//listener for new lap event
	void TriggerLapAlert (int newLapNumber) {
		NewCurrLapAlert.text = newLapNumber.ToString ();
		PlayAnim (0, 1);
	}

	void updateCoinScore (int val) {
		CoinScoreTxt.text = val.ToString ();
	}

	IEnumerator UpdateUIStats () {
		//populate current time
		string[] CurrLapTime = playerRaceData.Curr_LapTime.ToString ("F2").Split ('.');
		CurrLapTimeBigTxt.text = CurrLapTime[0];
		CurrLapTimeSmallTxt.text = "." + CurrLapTime[1];

		//populate best and last times
		if (playerRaceData.Curr_LapNumber < 2) {
			LastLapBigTxt.text = "-";
			LastLapSmlTxt.text = ".--";
			BestLapBigTxt.text = "-";
			BestLapSmlTxt.text = ".--";

		} else {
			string[] LastLapTime = playerRaceData.LastLapTime.ToString ("F2").Split ('.');
			LastLapBigTxt.text = LastLapTime[0];
			LastLapSmlTxt.text = "." + LastLapTime[1];

			string[] BestLapTime = playerRaceData.FastestLapTime.ToString ("F2").Split ('.');
			BestLapBigTxt.text = BestLapTime[0];
			BestLapSmlTxt.text = "." + BestLapTime[1];
		}
		//populate Pole position
		CurrPlaceTxt.text = playerRaceData.Curr_PolePosition.ToString ();
		//FIX ME
		PlacesInRaceTxt.text = statsManager.CarPoleData.Count.ToString ();

		//populate current lap
		if (playerRaceData.Curr_LapNumber < 1) {
			CurrLapTxt.text = "1";
		} else {
			CurrLapTxt.text = playerRaceData.Curr_LapNumber.ToString ();
		}

		yield return new WaitForSeconds (0.05f);
		UpdateUI = false;
	}

	void BeginRace () {
		LapsInRaceTxt.text = user.lapsInRace.ToString ();
		LapsInRaceAlert.text = user.lapsInRace.ToString ();
		//pole data 0 is always the player
		playerRaceData = statsManager.CarPoleData[0];
		RaceStarted = true;
		PlayAnim (0, 0);
	}

	void FacingWrongWay () {

		StartCoroutine ("blinkWrongWay");
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
		StopCoroutine ("blinkWrongWay");
		WrongWayIndicator.SetActive (false);
	}
	//--------------------------------------------------
}