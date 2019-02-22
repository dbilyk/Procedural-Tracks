using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//create two pools, active and inactive
//create handler for voxel collision with ground -> trigger animation
//create delegate in the controller that takes critter location and collision velocity vector as arguments
//randomize the velocity and force a bit
//re-add voxel to unused pool after animation is done. 

public class VoxelBloodController : MonoBehaviour {
	[SerializeField]
	List<GameObject> BloodVoxels = new List<GameObject>();

	[SerializeField]
	GameObject ActivePool, InactivePool; 

	//delegate signature called by animal controller on collision
	public delegate void Impact(Vector2 Velocity, float carVelMag, Vector2 carPosition, Vector2 critterPosition);
	
	//an instance of the delegate
	public Impact SpawnBlood;

[SerializeField]
	int poolSize = 50, VoxelsPerCritter = 5;
	void OnEnable(){
		//initializing the delegate that actually gets called from AnimalController.
		SpawnBlood = new Impact(_SpawnBlood);
		StartCoroutine(CreatePool());
	}


	//slowly populates the blood pool.
	IEnumerator CreatePool(){
		while(InactivePool.transform.childCount + ActivePool.transform.childCount < poolSize){
			yield return new WaitForSecondsRealtime(0.2f);
			int random = Random.Range(0,3);
			GameObject current = GameObject.Instantiate(BloodVoxels[random],InactivePool.transform);
			current.SetActive(false);

		}
	}


//triggered every time an animalController detects a collision.
	void _SpawnBlood(Vector2 velocity, float velMag,Vector2 carPos,Vector2 critterPos){
		int i = 0;
		while(InactivePool.transform.childCount>0 && i<VoxelsPerCritter){
			i++;
			GameObject voxel = InactivePool.transform.GetChild(0).gameObject;
			voxel.transform.parent = ActivePool.transform;
			voxel.transform.position = (Vector3)critterPos + new Vector3(0,0,-0.5f);
			voxel.SetActive(true);
			Rigidbody rb = voxel.GetComponent<Rigidbody>();
			rb.AddExplosionForce(velMag*10*(Random.value*3),critterPos+Random.insideUnitCircle*1f,100f,0f);
			Vector3 forceVector = new Vector3(0,0,Random.Range(-3f,1f)) + (Vector3)(velocity.normalized*(Random.Range(3,6f))*Mathf.Clamp(velocity.sqrMagnitude/20,0.1f,100f)); 
			rb.AddForce(forceVector,ForceMode.Impulse);
		}
	}
}
