using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnOff : MonoBehaviour {
	public void Enabled (bool val) {
		gameObject.transform.GetChild (0).gameObject.SetActive (val);
	}

}