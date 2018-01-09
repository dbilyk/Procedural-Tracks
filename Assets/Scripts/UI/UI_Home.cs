using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Home : MonoBehaviour {
	[SerializeField]
	StartScreenController IntroReel;

	[SerializeField]
	Button PlayBtn, SocialBtn, CarSelectBtn, OnlineBtn, FaceCamBtn, ScreenRecBtn, ShareBtn;

	Animator Anim;
	bool IntroAnimPlayed = false;

	//public home screen events
	public delegate void ButtonClick ();
	public event ButtonClick OnClickPlay, OnClickSettings, OnClickSocial, OnClickOnline,
	OnClickCarPicker, OnClickFaceCam, OnClickScreenRec, OnShareBtnClick;

	//Housekeeping
	void OnEnable () {
		Anim = gameObject.GetComponent<Animator> ();
		IntroReel.OnEndIntro += SlideIn;

		//Home UI button click event handler callbacks
		PlayBtn.onClick.AddListener (delegate { PlayBtnClick (); });
		SocialBtn.onClick.AddListener (delegate { SocialClick (); });
		OnlineBtn.onClick.AddListener (delegate { OnlineBtnClick (); });
		CarSelectBtn.onClick.AddListener (delegate { CarPickerBtnClick (); });

		//------------------------------------UNIMPLEMENTED !!!!!!!!!!!!!!!---------------------------------------------------
		//FaceCamBtn.onClick.AddListener (delegate { FaceCamBtnClick (); });
		//ScreenRecBtn.onClick.AddListener (delegate { ScreenRecBtnClick (); });
		//ShareBtn.onClick.AddListener (delegate { ShareBtnClick (); });

	}

	void OnDisable () {
		IntroReel.OnEndIntro -= SlideIn;
	}

	//these are triggered by buttons and any validation can be done here before emitting the event
	void PlayBtnClick () {
		Debug.Log ("No Play");
		if (OnClickPlay != null) OnClickPlay ();
	}

	//callback functions from buttons
	void SocialClick () {
		Debug.Log ("No Social");
		if (OnClickSocial != null) {
			OnClickSocial ();
		}
	}
	void OnlineBtnClick () {
		Debug.Log ("No Online");
		if (OnClickOnline != null) {
			OnClickOnline ();
		}
	}
	void CarPickerBtnClick () {
		Debug.Log ("No CarPicker");
		if (OnClickCarPicker != null) {
			OnClickCarPicker ();
		}
	}
	void FaceCamBtnClick () {
		Debug.Log ("No FaceCam");
		if (OnClickFaceCam != null) {
			OnClickFaceCam ();
		}
	}
	void ScreenRecBtnClick () {
		Debug.Log ("No scrnRec");
		if (OnClickScreenRec != null) {
			OnClickScreenRec ();
		}
	}
	void ShareBtnClick () {
		Debug.Log ("No share");
		if (OnShareBtnClick != null) {
			OnShareBtnClick ();
		}
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
		gameObject.transform.GetChild (0).gameObject.SetActive (state);
	}

	//logic
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