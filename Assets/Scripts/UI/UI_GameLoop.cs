using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_GameLoop : MonoBehaviour {
	Animator Anim;

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
	GameObject WrongWayIndicator;

	[SerializeField]
	InputManager inputManager;
	[SerializeField]
	RaceStatsManager raceStatsManager;
	CarPolePositionData playerRaceData;

	public delegate void ButtonClick ();
	public event ButtonClick OnClickPause;

	void OnEnable () {
		UpdateUI = false;

		LapsInRaceAlert.text = Data.Curr_NumberOfLapsInRace.ToString ();

		PauseBtn.onClick.AddListener (delegate { ClickPause (); });
		raceStatsManager.OnLapAlert += TriggerLapAlert;
		//pole data 0 is always the player
		playerRaceData = Data.CarPoleData[0];
	}

	void OnDisable () {
		raceStatsManager.OnLapAlert -= TriggerLapAlert;
	}

	bool UpdateUI;
	void Update () {
		if (!UpdateUI) {
			StartCoroutine ("UpdateUIStats");
			UpdateUI = true;
		}
	}

	//pause event sender
	void ClickPause () {
		Debug.Log ("pauseBTn");
		if (OnClickPause != null) {
			OnClickPause ();
		}
	}

	//listener for new lap event
	void TriggerLapAlert (int newLapNumber) {
		CurrLapTxt.text = newLapNumber.ToString ();
		NewCurrLapAlert.text = newLapNumber.ToString ();
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
		PlacesInRaceTxt.text = Data.CarPoleData.Count.ToString ();

		//populate current lap
		if (playerRaceData.Curr_LapNumber < 1) {
			CurrLapTxt.text = "1";
		} else {
			CurrLapTxt.text = playerRaceData.Curr_LapNumber.ToString ();
		}
		LapsInRaceTxt.text = Data.Curr_NumberOfLapsInRace.ToString ();

		yield return new WaitForSeconds (0.05f);
		UpdateUI = false;
	}
}