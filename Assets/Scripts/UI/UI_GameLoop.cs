using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_GameLoop : MonoBehaviour {
	Animator Anim;

	//these should all be updated through events 
	//BUT MUST BE UNSUBBED AFTER ANY EXIT CONDITION.!
	[SerializeField]
	Text BestLapTxt, LastLapTxt, CurrLapTxt, LapsInRaceTxt, CurrPlaceTxt,
	PlacesInRaceTxt, CurrLapTimeBigTxt, CurrLapTimeSmallTxt, CoinScoreTxt;

	[SerializeField]
	Button PauseBtn;

	public delegate void ButtonClick ();
	public event ButtonClick OnClickPause;

	void OnEnable () {
		PauseBtn.onClick.AddListener (delegate { ClickPause (); });
	}

	void ClickPause () {
		Debug.Log ("pauseBTn");
		if (OnClickPause != null) {
			OnClickPause ();
		}
	}

}