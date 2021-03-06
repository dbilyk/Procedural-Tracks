﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//small helper to trigger a delegate on the parent StartingLight Script from the animation attached to this GO.
public class RaceStartedEvent : MonoBehaviour {
	[SerializeField]
	StartingLight startingLight;

	public void RaceStarted () {
		startingLight.animComplete ();
	}

//called from animation to disable the lights when done
	public void Disable (){
		gameObject.transform.parent.gameObject.SetActive (false);
	}
}