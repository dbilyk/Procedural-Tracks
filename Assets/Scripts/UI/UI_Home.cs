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

	//public home screen events
	public delegate void ButtonClick ();
	public event ButtonClick OnClickPlay;
	public event ButtonClick OnClickSettings;
	public event ButtonClick OnClickSocial;
	public event ButtonClick OnClickOnline;
	public event ButtonClick OnClickCarPicker;
	public event ButtonClick OnClickFaceCam;
	public event ButtonClick OnClickScreenRec;
	public event ButtonClick OnShareBtnClick;

	//these are triggered by buttons and any validation can be done here before emitting the event
	public void PlayBtnClick () {
		if (OnClickPlay != null) OnClickPlay ();
	}
	public void SettingsClick () {
		if (OnClickSettings != null) OnClickSettings ();
	}
	public void SocialClick () {
		if (OnClickSocial != null) OnClickSocial ();
	}
	public void OnlineBtnClick () {
		if (OnClickOnline != null) OnClickOnline ();
	}
	public void CarPickerBtnClick () {
		if (OnClickCarPicker != null) OnClickCarPicker ();
	}
	public void FaceCamBtnClick () {
		if (OnClickFaceCam != null) OnClickFaceCam ();
	}
	public void ScreenRecBtnClick () {
		if (OnClickScreenRec != null) OnClickScreenRec ();
	}
	public void ShareBtnClick () {
		if (OnShareBtnClick != null) OnShareBtnClick ();
	}
	//Housekeeping
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

	bool settingsIn = false;
	private void ToggleSettings () {
		if (settingsIn) {

		} else {

		}
	}

	bool socialIn = false;
	private void ToggleSocial () {
		if (socialIn) {

		} else {

		}
	}
}