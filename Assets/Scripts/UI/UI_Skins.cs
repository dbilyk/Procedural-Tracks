using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TrackSkins { Farm, Mountains, Desert, Snow }

public class UI_Skins : MonoBehaviour {
  Animator Anim;

  [SerializeField]
  Button FarmBtn, MtnBtn, DesertBtn, SnowBtn, BackBtn;

  [SerializeField]
  UI_Home HomeScreen;

  [SerializeField]
  UI_Header header;

  [SerializeField]
  UI_MapSelector MapSelector;

  public delegate void ButtonClick (TrackSkins trackSkin);
  public event ButtonClick OnClickSkin;

  //anim state names
  List<string> BaseStates = new List<string> () {
    "Skins_SlideInL",
    "Skins_SlideInR",
    "Skins_SlideOutL",
    "Skins_SlideOutR",
  };

  //Housekeeping
  void OnEnable () {
    Anim = gameObject.GetComponent<Animator> ();

    //event listeners
    HomeScreen.OnClickButton += SlideInR;
    header.OnClickSkinsBack += SlideOutR;
    header.OnClickTrackPickerBack += SlideInL;

    //private button events
    FarmBtn.onClick.AddListener (delegate { ClickFarm (); });
    MtnBtn.onClick.AddListener (delegate { ClickMountains (); });
    DesertBtn.onClick.AddListener (delegate { ClickDesert (); });
    SnowBtn.onClick.AddListener (delegate { ClickSnow (); });
  }

  void PlayAnim (int statesIndex, int layer) {
    string anim = BaseStates[statesIndex];
    Anim.Play (anim, layer);
  }

  void OnDisable () {

  }

  //UI event Handlers
  void ClickFarm () {
    PlayAnim (2, 0);
    if (OnClickSkin != null) {
      OnClickSkin (TrackSkins.Farm);
    }
  }

  void ClickMountains () {
    PlayAnim (2, 0);
    if (OnClickSkin != null) {
      OnClickSkin (TrackSkins.Mountains);
    }
  }
  void ClickDesert () {
    PlayAnim (2, 0);
    if (OnClickSkin != null) {
      OnClickSkin (TrackSkins.Desert);
    }
  }
  void ClickSnow () {
    PlayAnim (2, 0);
    if (OnClickSkin != null) {
      OnClickSkin (TrackSkins.Snow);
    }
  }

  public void SlideInR (Home_Btns btns) {

    if (btns == Home_Btns.PlayBtn) PlayAnim (1, 0);
  }

  public void SlideOutR () {
    PlayAnim (3, 0);
  }

  public void SlideInL () {
    PlayAnim (0, 0);
  }

  public void EnableScreen () {
    gameObject.transform.GetChild (0).gameObject.SetActive (true);
  }

  public void DisableScreen () {
    gameObject.transform.GetChild (0).gameObject.SetActive (false);
  }

}