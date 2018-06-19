using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelBloodInstance : MonoBehaviour {
	
	Animator anim;

	void Awake() {
			anim = this.gameObject.transform.GetChild(0).GetComponent<Animator>();
		
	}
	void OnEnable(){
		anim.Play("Default Take",0,0f);
		anim.speed = 0;
		gameObject.transform.localScale = new Vector3(0.2f,0.2f,0.2f) * Random.Range(0.5f,1.2f);
		
	}
	void OnCollisionEnter(Collision col){
		if(col.gameObject.name =="BGMesh"){
			anim.speed = 1;
			anim.Play("Default Take",0,0f);
			StartCoroutine(AnimIsDone());
		}
	}

	IEnumerator AnimIsDone(){
		yield return new WaitForSeconds(1.5f);
		gameObject.SetActive(false);
		transform.parent = transform.parent.parent.GetChild(1);
	}

}
