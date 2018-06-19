using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableVoxel : MonoBehaviour {
	void Disable(){
		gameObject.transform.parent.gameObject.SetActive(false);
		Debug.Log("boom");
		//gets the inactive pool
		transform.parent = transform.parent.parent.parent.GetChild(1);
	}

}
