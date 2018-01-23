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

	[SerializeField]
	StartScreenController startScreen;

	public delegate void ButtonClick ();
	public event ButtonClick OnClickHomeSettings, OnClickSkinsBack, OnClickTrackPickerBack;

	//anim state names
	List<string> BaseStates = new List<string> () {
		"HeaderIntro",
		"SwapSettingsToSkinsBack",
		"SwapSkinsToSettings",
		"SwapSkinsToMaps",
		"SwapMapsToSkins",
		"HeaderOutro"
	};

	//anim layer1 names
	List<string> LayerOneStates = new List<string> () {
		"CoinFlash",
		"",
		"",
		""
	};

	//utility
	void PlayAnim (int baseStatesIndex, int layerIndex) {
		List<string> target = new List<string> ();
		switch (layerIndex) {
			case 0:
				target = BaseStates;
				break;
			case 1:
				target = LayerOneStates;
				break;
		}

		Anim.Play (target[baseStatesIndex], layerIndex);
	}

	public void EnableScreen () {
		gameObject.transform.GetChild (0).gameObject.SetActive (true);
	}

	public void DisableScreen () {

		gameObject.transform.GetChild (0).gameObject.SetActive (false);
	}

	//Housekeeping
	void OnEnable () {
		startScreen.OnEndIntro += introAnim;
		HomeUI.OnClickButton += iconSwapHomeToSkins;
		SkinsUI.OnClickSkin += iconSwapSkinsToMap;
		MapSelectorUI.OnClickStartRace += outroAnim;
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
		PlayAnim (2, 0);
		if (OnClickSkinsBack != null) {
			OnClickSkinsBack ();
		}

	}

	void ClickTrackPickerBack () {
		Debug.Log ("Track Picker Back btn");
		if (OnClickTrackPickerBack != null) {
			OnClickTrackPickerBack ();
		}
		PlayAnim (4, 0);

	}

	//these handle the inital animation in
	void introAnim () {
		StartCoroutine ("delayAnim");
		StartCoroutine ("delayCoinFlash");
	}
	IEnumerator delayAnim () {
		yield return new WaitForSecondsRealtime (0.5f);
		PlayAnim (0, 0);

	}

	IEnumerator delayCoinFlash () {
		yield return new WaitForSecondsRealtime (1f);
		PlayAnim (0, 1);

	}

	void iconSwapHomeToSkins (Home_Btns btn) {
		if (btn == Home_Btns.PlayBtn) {
			PlayAnim (1, 0);
		}
	}

	void iconSwapSkinsToMap (TrackSkins skin) {
		PlayAnim (3, 0);
	}

	void outroAnim (Track t, bool b) {
		PlayAnim (5, 0);
	}
}