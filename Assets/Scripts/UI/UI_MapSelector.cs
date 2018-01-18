using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AIDifficulty {
	easy,
	medium,
	hard
}

public class UI_MapSelector : MonoBehaviour {
	Animator Anim;

	[SerializeField]
	UI_MapViewer mapViewer;

	Track selectedTrack;

	[SerializeField]
	UI_Skins UI_SkinSelector;

	[SerializeField]
	Button NewTrackVideoBtn, NewTrackCoinsBtn, StartRaceBtn;

	[SerializeField]
	Scrollbar CornerWidth, CornerCount, OpponentCount;

	[SerializeField]
	Toggle EasyTgl, MediumTgl, HardTgl;

	private List<string> BaseStates = new List<string> () {
		"",
		"",
		"",
		""
	};
	//buttons in the screen
	public delegate void ButtonClick ();
	public event ButtonClick OnClickNewTrackCoins, OnClickNewTrackVideo;

	public delegate void startRace (Track t);
	public event startRace OnClickStartRace;

	//sliders in the screen
	public delegate void SliderChanged (float newValue);
	public event SliderChanged OnWidthValueChanged, OnCornerCtChanged, OnOpponentCtChanged;

	//difficulty toggle event sends an enum
	public delegate void ToggleChanged (AIDifficulty activeDifficulty);
	public event ToggleChanged OnDifficultyChange;

	//utility methods
	void PlayAnim (int baseStatesIndex, int layerIndex) {
		Anim.Play (BaseStates[baseStatesIndex], layerIndex);
	}

	void ScreenOn (bool state) {
		gameObject.transform.GetChild (0).gameObject.SetActive (state);
	}

	//Housekeeping
	void OnEnable () {
		Anim = gameObject.GetComponent<Animator> ();

		//trigger events when UI buttons change
		EasyTgl.onValueChanged.AddListener (delegate { ToggleDiff (EasyTgl); });
		MediumTgl.onValueChanged.AddListener (delegate { ToggleDiff (MediumTgl); });
		HardTgl.onValueChanged.AddListener (delegate { ToggleDiff (HardTgl); });

		NewTrackCoinsBtn.onClick.AddListener (delegate { NewTrackCoins (); });
		NewTrackVideoBtn.onClick.AddListener (delegate { NewTrackVideo (); });
		StartRaceBtn.onClick.AddListener (delegate { StartRace (); });

		CornerWidth.onValueChanged.AddListener (delegate { WidthValueChanged (); });
		CornerCount.onValueChanged.AddListener (delegate { CornerCtChanged (); });
		OpponentCount.onValueChanged.AddListener (delegate { OpponentCtChanged (); });

	}

	void OnDisable () {

	}

	//listeners for newTrack btn
	void NewTrackVideo () {
		selectedTrack = mapViewer.SelectedTrack;
		Debug.Log ("newTrackVideo");
		if (OnClickNewTrackVideo != null) {
			OnClickNewTrackVideo ();
		}
	}

	void NewTrackCoins () {
		Debug.Log ("newTrackCoins");
		selectedTrack = mapViewer.SelectedTrack;
		if (OnClickNewTrackCoins != null) {
			OnClickNewTrackCoins ();
		}
	}

	//listeners for the slider callbacks pass the new value on to the events.
	float prevWidth = 0;
	void WidthValueChanged () {
		Debug.Log ("Width Val");
		if (OnWidthValueChanged != null) {
			OnWidthValueChanged (CornerWidth.value);

		}
	}
	void CornerCtChanged () {
		Debug.Log ("cornerCt");
		if (OnCornerCtChanged != null) {
			OnCornerCtChanged (CornerCount.value);

		}
	}
	void OpponentCtChanged () {
		Debug.Log ("OppCt");
		if (OnOpponentCtChanged != null) {
			OnOpponentCtChanged (OpponentCount.value);

		}
	}

	void ToggleDiff (Toggle m_Toggle) {
		AIDifficulty difficulty;
		if (Object.ReferenceEquals (m_Toggle, EasyTgl)) {
			difficulty = AIDifficulty.easy;
			Debug.Log ("easy");
		} else if (Object.ReferenceEquals (m_Toggle, MediumTgl)) {
			difficulty = AIDifficulty.medium;
			Debug.Log ("Medium");

		} else {
			difficulty = AIDifficulty.hard;
			Debug.Log ("Hard");
		}
		if (OnDifficultyChange != null) {
			OnDifficultyChange (difficulty);
		}

	}

	void StartRace () {
		Debug.Log ("StartRace");
		if (OnClickStartRace != null) {
			OnClickStartRace (selectedTrack);
		}
	}

	//animations
	private void SlideIn_R () {

	}

	private void SlideOut_R () {

	}

	private void SlideIn_L () {

	}

	private void SlideOut_L () {

	}
}