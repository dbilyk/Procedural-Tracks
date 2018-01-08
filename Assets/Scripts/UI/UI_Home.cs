using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Home : MonoBehaviour {
	[SerializeField]
	GameObject HomeUI;
	[SerializeField]
	private StartScreenController IntroReel;
	private bool IntroAnimPlayed = false;
	private Animator Anim;
	//events from elsewhere that concern this screen

	void OnEnable () {
		Anim = gameObject.GetComponent<Animator> ();
		IntroReel.OnEndIntro += SlideIn;

	}

	void OnDisable () {
		IntroReel.OnEndIntro -= SlideIn;

	}

	//anim state names
	List<string> BaseStates = new List<string> () {
		"HomeScreenEnable",
		"SettingsClick",
		"SlideScreenOutL",
		"SlideScreenInL"
	};

	void PlayAnim (int baseStatesIndex, int layerIndex) {
		Anim.Play (BaseStates[baseStatesIndex], layerIndex);
	}

	void ScreenOn (bool state) {
		HomeUI.SetActive (state);
	}

	public void SlideIn () {
		ScreenOn (true);
		if (!IntroAnimPlayed) {
			PlayAnim (0, 0);
		} else {
			PlayAnim (3, 0);

		}
	}

	public void SlideOut () {
		ScreenOn (false);
		PlayAnim (2, 0);
	}

	bool settingsOn = false;
	private void ToggleSettings () {
		if (settingsOn) {

		} else {

		}
	}

	bool socialOn = false;
	private void ToggleSocial () {
		if (socialOn) {

		} else {

		}
	}

	bool IntroPlayed;
	void PlayIntroAnim () { }
}