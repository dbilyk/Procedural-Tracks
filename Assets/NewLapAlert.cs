using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewLapAlert : MonoBehaviour {

    public Animator anim;
	public void DeactivateAnim()
    {
       gameObject.SetActive(false);
    }
}
