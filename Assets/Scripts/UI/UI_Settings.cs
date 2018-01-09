using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour {
	Animator Anim;

	[SerializeField]
	Slider MusicVolSlider;

	[SerializeField]
	Slider SFXSlider;

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

	void ScreenOn (bool state) {
		gameObject.transform.GetChild (0).gameObject.SetActive (state);
	}
	//Housekeeping
	void OnEnable () {
		Anim = gameObject.GetComponent<Animator> ();
		MusicVolSlider.onValueChanged.AddListener (delegate { MusicVolChanged (); });
		SFXSlider.onValueChanged.AddListener (delegate { SFXVolChanged (); });
	}

	void OnDisable () {

	}

	//UI events, check for gotchas here before sending
	public void ClickTutorial () {
		if (OnClickTutorial != null) OnClickTutorial ();
	}

	public void ClickCredits () {
		if (OnClickCredits != null) OnClickCredits ();
	}
	public void MusicVolChanged () {
		if (OnMusicVolChanged != null) {

			OnMusicVolChanged (MusicVolSlider.value);
		}
	}
	public void SFXVolChanged () {
		if (OnSFXVolChanged != null) {
			OnSFXVolChanged (SFXSlider.value);
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