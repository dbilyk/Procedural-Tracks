using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Skins : MonoBehaviour {
  Animator Anim;

  [SerializeField]
  Button FarmBtn, MtnBtn, DesertBtn, SnowBtn;

  [SerializeField]
  UI_Home HomeScreen;

  [SerializeField]
  UI_MapSelector MapSelector;

  public delegate void ButtonClick (TrackSkins trackSkin);
  public event ButtonClick OnClickSkin;

  //anim state names
  List<string> BaseStates = new List<string> () {
    "",
    "",
    "",
    ""
  };

  void ScreenOn (bool state) {
    gameObject.transform.GetChild (0).gameObject.SetActive (state);
  }

  //Housekeeping
  void OnEnable () {
    Anim = gameObject.GetComponent<Animator> ();

    //private button events
    FarmBtn.onClick.AddListener (delegate { ClickFarm (); });
    MtnBtn.onClick.AddListener (delegate { ClickMountains (); });
    DesertBtn.onClick.AddListener (delegate { ClickDesert (); });
    SnowBtn.onClick.AddListener (delegate { ClickSnow (); });
  }

  void OnDisable () {

  }

  //UI event Handlers
  void ClickFarm () {
    if (OnClickSkin != null) {
      OnClickSkin (TrackSkins.Farm);
    }
  }

  void ClickMountains () {
    if (OnClickSkin != null) {
      OnClickSkin (TrackSkins.Mountains);
    }
  }
  void ClickDesert () {
    if (OnClickSkin != null) {
      OnClickSkin (TrackSkins.Desert);
    }
  }
  void ClickSnow () {
    if (OnClickSkin != null) {
      OnClickSkin (TrackSkins.Snow);
    }
  }

  //animations
  void SlideIn_R () {

  }

  void SlideOut_R () {

  }

  void SlideIn_L () {

  }

  void SlideOut_L () {

  }
}