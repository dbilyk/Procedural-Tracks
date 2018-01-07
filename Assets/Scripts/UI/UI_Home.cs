using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Home : MonoBehaviour {
	[SerializeField]
	GameObject HomeUI;
	Animator Anim;

	void OnEnable () {
		Anim = gameObject.GetComponent<Animator> ();
		// Trigger my animation states in these functions
	}

	void ScreenOn (bool state) {
		HomeUI.SetActive (state);
	}

	public void OnSlideInStart_L () {
		ScreenOn (true);

	}

	public void OnSlideOutEnd_L () {
		ScreenOn (false);
	}

	bool settingsOn = false;
	public void ToggleSettings () {
		if (settingsOn) {

		} else {

		}
	}

	bool socialOn = false;
	public void ToggleSocial () {
		if (socialOn) {

		} else {

		}
	}
}