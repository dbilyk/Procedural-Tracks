using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Header : MonoBehaviour {
	Animator Anim;
	[SerializeField]
	Button SkinsBackBtn, TrackPickerBackBtn, SettingsBtn;

	[SerializeField]
	Text CurrencyCount;

	[SerializeField]
	UI_Home HomeUI;

	[SerializeField]
	UI_MapSelector MapSelectorUI;

	[SerializeField]
	UI_Skins SkinsUI;

	public delegate void ButtonClick ();
	public event ButtonClick OnClickHomeSettings, OnClickSkinsBack, OnClickTrackPickerBack;

	//anim state names
	List<string> BaseStates = new List<string> () {
		"",
		"",
		"",
		""
	};

	//utility
	void PlayAnim (int baseStatesIndex, int layerIndex) {
		Anim.Play (BaseStates[baseStatesIndex], layerIndex);
	}

	void ScreenOn (bool state) {
		gameObject.transform.GetChild (0).gameObject.SetActive (state);
	}
	//Housekeeping
	void OnEnable () {
		Anim = gameObject.GetComponent<Animator> ();

		//settings button event callbacks
		SettingsBtn.onClick.AddListener (delegate { ClickSettings (); });
		SkinsBackBtn.onClick.AddListener (delegate { ClickSkinsBack (); });
		TrackPickerBackBtn.onClick.AddListener (delegate { ClickTrackPickerBack (); });
	}

	void OnDisable () {

	}

	void ClickSettings () {
		Debug.Log ("home settings");
		if (OnClickHomeSettings != null) {
			OnClickHomeSettings ();
		}
	}

	void ClickSkinsBack () {
		Debug.Log ("Skins Back Btn");
		if (OnClickSkinsBack != null) {
			OnClickSkinsBack ();
		}

	}

	void ClickTrackPickerBack () {
		Debug.Log ("Track Picker Back btn");
		if (OnClickTrackPickerBack != null) {
			OnClickTrackPickerBack ();
		}

	}

}