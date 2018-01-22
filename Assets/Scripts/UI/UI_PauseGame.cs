using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PauseGame : MonoBehaviour {
	Animator Anim;
	[SerializeField]
	Button RestartBtn, QuitBtn;

	[SerializeField]
	Slider MusicVolSlider, SFXSlider;

	public delegate void ButtonClick ();
	public event ButtonClick OnClickRestart, OnClickExit, OnClickCloseMenu;

	public delegate void SliderChanged (float newValue);
	public event SliderChanged OnSFXVolChanged, OnMusicVolChanged;

	void OnEnable () {
		Anim = gameObject.GetComponent<Animator> ();

	}

	private List<string> BaseStates = new List<string> () {
		"",
		"",
		"",
		""
	};

	//utility methods
	void PlayAnim (int baseStatesIndex, int layerIndex) {
		Anim.Play (BaseStates[baseStatesIndex], layerIndex);
	}

	public void ClickRestart () {
		Debug.Log ("Restart");
		if (OnClickRestart != null) {
			OnClickRestart ();
		}
	}

	public void ClickExit () {
		Debug.Log ("exit");
		if (OnClickExit != null) {
			OnClickExit ();
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