using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCritterAnimDelay : MonoBehaviour {

    public Animator Anim;
	void Start () {
        Anim.enabled = false;
        StartCoroutine(StartAnim());
	}

    IEnumerator StartAnim()
    {
        yield return new WaitForSeconds(0.0f);
        Anim.enabled = true;
    }
	
}
