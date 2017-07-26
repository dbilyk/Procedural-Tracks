using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagForScenePersistence : MonoBehaviour {
    public List<GameObject> PersistenceList;
	// Use this for initialization
	void Start () {

        foreach (GameObject obj in PersistenceList)
        {
            DontDestroyOnLoad(obj);
        }
       
        
	}
}
