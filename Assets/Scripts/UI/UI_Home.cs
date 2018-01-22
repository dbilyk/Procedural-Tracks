using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Home_Btns {
	PlayBtn,
	SocialBtn,
	CarSelectBtn,
	OnlineBtn,
	FaceCamBtn,
	ScreenRecBtn,
	ShareBtn
}

public class UI_Home : MonoBehaviour {
	[SerializeField]
	StartScreenController IntroReel;

	[SerializeField]
	Button PlayBtn, SocialBtn, CarSelectBtn, OnlineBtn, FaceCamBtn, ScreenRecBtn, ShareBtn;

	Animator Anim;
	bool IntroAnimPlayed = false;

	[SerializeField]
	UI_Header header;

	//public home screen events
	public delegate void ButtonClick (Home_Btns btn);
	public event ButtonClick OnClickButton;

	//Housekeeping
	void OnEnable () {
		Anim = gameObject.GetComponent<Animator> ();
		IntroReel.OnEndIntro += SlideIn;
		header.OnClickSkinsBack += SlideIn;
		//Home UI button click event handler callbacks
		PlayBtn.onClick.AddListener (delegate { PlayBtnClick (); });
		SocialBtn.onClick.AddListener (delegate { SocialClick (); });
		OnlineBtn.onClick.AddListener (delegate { OnlineBtnClick (); });
		CarSelectBtn.onClick.AddListener (delegate { CarPickerBtnClick (); });

		//------------------------------------UNIMPLEMENTED !!!!!!!!!!!!!!!---------------------------------------------------
		FaceCamBtn.onClick.AddListener (delegate { FaceCamBtnClick (); });
		ScreenRecBtn.onClick.AddListener (delegate { ScreenRecBtnClick (); });
		ShareBtn.onClick.AddListener (delegate { ShareBtnClick (); });

	}

	void OnDisable () {
		IntroReel.OnEndIntro -= SlideIn;
	}

	//these are triggered by buttons and any validation can be done here before emitting the event
	void PlayBtnClick () {
		PlayAnim (2, 0);
		if (OnClickButton != null) OnClickButton (Home_Btns.PlayBtn);
	}

	//callback functions from buttons
	void SocialClick () {
		Debug.Log ("No Social");
		if (OnClickButton != null) {
			OnClickButton (Home_Btns.SocialBtn);
		}
	}
	void OnlineBtnClick () {
		Debug.Log ("No Online");
		if (OnClickButton != null) {
			OnClickButton (Home_Btns.OnlineBtn);
		}
	}
	void CarPickerBtnClick () {
		Debug.Log ("No CarPicker");
		if (OnClickButton != null) {
			OnClickButton (Home_Btns.CarSelectBtn);
		}
	}
	void FaceCamBtnClick () {
		Debug.Log ("No FaceCam");
		if (OnClickButton != null) {
			OnClickButton (Home_Btns.FaceCamBtn);
		}
	}
	void ScreenRecBtnClick () {
		Debug.Log ("No scrnRec");
		if (OnClickButton != null) {
			OnClickButton (Home_Btns.ScreenRecBtn);
		}
	}
	void ShareBtnClick () {
		Debug.Log ("No share");
		if (OnClickButton != null) {
			OnClickButton (Home_Btns.ShareBtn);
		}
	}
	//anim state names
	List<string> BaseStates = new List<string> () {
		"HomeScreenEnable",
		"Home_SlideInL",
		"Home_SlideOutL"
	};

	void PlayAnim (int baseStatesIndex, int layerIndex) {
		Anim.Play (BaseStates[baseStatesIndex], layerIndex);
	}
	//logic
	public void SlideIn () {
		if (!IntroAnimPlayed) {
			EnableScreen ();
			PlayAnim (0, 0);
			IntroAnimPlayed = true;
		} else {
			PlayAnim (1, 0);

		}
	}

	public void SlideOut () {
		PlayAnim (2, 0);

	}

	public void EnableScreen () {
		gameObject.transform.GetChild (0).gameObject.SetActive (true);
	}

	public void DisableScreen () {

		gameObject.transform.GetChild (0).gameObject.SetActive (false);
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