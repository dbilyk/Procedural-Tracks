using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreenIntroToggle : MonoBehaviour {
    private Animator anim;

    void OnEnable()
    {
        anim = gameObject.GetComponent<Animator>();
    }
	// Update is called once per frame
	void DisableAnimator () {
        anim.enabled = false;
	}
}
