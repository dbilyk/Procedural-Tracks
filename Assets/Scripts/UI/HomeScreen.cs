using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IScreen {
  void AnimateIn ();
  void AnimateOut ();

}

public interface IHomeScreen : IScreen {
  void OnSettings (bool isOpen);
  void OnPlay ();
  void OnCarSelector ();
  void OnSocial (bool isOpen);
  void OnNetwork ();

}

//structure for UI as follows.  Input is recieved from the user which can be done through the Unity event.
//Upon reception of this input, a 

public class HomeScreen : MonoBehaviour, IScreen, IHomeScreen {
  public event OnAnimateIn AnimateIn;
  public delegate void OnAnimateIn ();
  public event OnAnimateOut AnimateOut;
  public delegate void OnAnimateOut ();
  void IScreen.AnimateIn () {

  }

  void IScreen.AnimateOut () {

  }
  void IHomeScreen.OnSettings (bool isOpen) {

  }
  void IHomeScreen.OnPlay () {

  }
  void IHomeScreen.OnCarSelector () {

  }
  void IHomeScreen.OnSocial (bool isOpen) {

  }
  void IHomeScreen.OnNetwork () {

  }
}