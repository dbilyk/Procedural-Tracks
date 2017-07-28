using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAIColor : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value);
	}
}
