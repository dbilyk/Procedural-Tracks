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
	[SerializeField]
	MapRenderer mapRenderer;
	Animator Anim;

	[SerializeField]
	UI_Skins skinsScreen;

	[SerializeField]
	UI_Header header;

	[SerializeField]
	UI_MapViewer mapViewer;
	[SerializeField]
	SnapScroller snapScroller;

	[SerializeField]
	UI_Skins UI_SkinSelector;

	[SerializeField]
	Button NewTrackVideoBtn, NewTrackCoinsBtn, StartRaceBtn;

	[SerializeField]
	Scrollbar CornerWidth, CornerCount, OpponentCount;

	[SerializeField]
	Toggle EasyTgl, MediumTgl, HardTgl;

	private List<string> BaseStates = new List<string> () {
		"MapSelector_SlideInR",
		"MapSelector_SlideOutR",
		"",
		""
	};
	//buttons in the screen
	public delegate void ButtonClick ();
	public event ButtonClick OnClickNewTrackCoins, OnClickNewTrackVideo;

	public delegate void startRace (Track t, bool isNew);
	public event startRace OnClickStartRace;

	//sliders in the screen
	public delegate void SliderChanged (float newValue, int totalPositions);
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
		skinsScreen.OnClickSkin += SlideInR;
		header.OnClickTrackPickerBack += SlideOutR;
		Anim = gameObject.GetComponent<Animator> ();
		_newTrackCreated = false;
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

		snapScroller.OnNewTargetTrack += TargetTrackChanged;

	}

	void OnDisable () {

	}

	bool _newTrackCreated;

	//listeners for newTrack btn
	void NewTrackVideo () {
		Debug.Log ("newTrackVideo");
		_newTrackCreated = true;
		StartRaceBtn.interactable = true;
		if (OnClickNewTrackVideo != null) {
			OnClickNewTrackVideo ();
		}
	}

	void NewTrackCoins () {
		Debug.Log ("newTrackCoins");
		_newTrackCreated = true;
		StartRaceBtn.interactable = true;
		if (OnClickNewTrackCoins != null) {
			OnClickNewTrackCoins ();
		}
	}

	

	//listeners for the slider callbacks pass the new value on to the events.
	float prevWidth = 0;
	void WidthValueChanged () {
		Debug.Log ("Width Val");
		if (OnWidthValueChanged != null) {
			OnWidthValueChanged (CornerWidth.value, CornerWidth.numberOfSteps);

		}
	}
	void CornerCtChanged () {
		Debug.Log ("cornerCt");
		if (OnCornerCtChanged != null) {
			OnCornerCtChanged (CornerCount.value, CornerCount.numberOfSteps);

		}
	}
	void OpponentCtChanged () {
		Debug.Log ("OppCt");
		if (OnOpponentCtChanged != null) {
			OnOpponentCtChanged (OpponentCount.value, OpponentCount.numberOfSteps);

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
			if (snapScroller.targetTrackIndex == 0) {
				//send the newly rendered track that the mapViewer created
				OnClickStartRace (mapViewer.SelectedTrack, true);
			} else {
				OnClickStartRace (User.SavedTracks[snapScroller.targetTrackIndex - 1], false);
			}
		}
		PlayAnim (1, 0);
	}

	//---------EVENT LISTENERS FROM OTHER CLASSES---------------

	void TargetTrackChanged (int index) {
		//checks to make sure you cant start a race if you are targeting a NEW track, but havent paid for it yet
		if (index == 0 && !_newTrackCreated) {
			StartRaceBtn.interactable = false;
		} else {
			StartRaceBtn.interactable = true;
		}
	}

	void SlideInR (TrackSkins skins) {
		PlayAnim (0, 0);
	}

	void SlideOutR () {
		PlayAnim (1, 0);
	}

}