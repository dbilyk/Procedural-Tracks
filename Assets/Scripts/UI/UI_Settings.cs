using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour {
	Animator Anim;

	[SerializeField]
	Button TutorialBtn, CreditsBtn;

	[SerializeField]
	Slider MusicVolSlider, SFXSlider;

	public delegate void ButtonClick ();
	public event ButtonClick OnClickTutorial, OnClickCredits;

	public delegate void SliderChanged (float newValue);
	public event SliderChanged OnSFXVolChanged, OnMusicVolChanged;

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
	//Housekeeping
	void OnEnable () {
		Anim = gameObject.GetComponent<Animator> ();

		//settings button event callbacks
		TutorialBtn.onClick.AddListener (delegate { ClickTutorial (); });
		CreditsBtn.onClick.AddListener (delegate { ClickCredits (); });
		MusicVolSlider.onValueChanged.AddListener (delegate { MusicVolChanged (); });
		SFXSlider.onValueChanged.AddListener (delegate { SFXVolChanged (); });
	}

	void OnDisable () {

	}

	//UI events, check for gotchas here before sending
	public void ClickTutorial () {
		Debug.Log ("Tutorial");
		if (OnClickTutorial != null) {
			OnClickTutorial ();
		}
	}

	public void ClickCredits () {
		Debug.Log ("Credits");
		if (OnClickCredits != null) {
			OnClickCredits ();
		}
	}

	public void MusicVolChanged () {
		Debug.Log ("Music Slider");
		if (OnMusicVolChanged != null) {

			OnMusicVolChanged (MusicVolSlider.value);
		}
	}
	public void SFXVolChanged () {
		Debug.Log ("SFX Slider");
		if (OnSFXVolChanged != null) {
			OnSFXVolChanged (SFXSlider.value);
		}
	}
}