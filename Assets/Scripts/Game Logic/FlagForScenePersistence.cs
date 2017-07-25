using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagForScenePersistence : MonoBehaviour {
    public GameObject Data;
    public GameObject GameManager;
    public GameObject MapCreator;
    
	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(Data);
        DontDestroyOnLoad(GameManager);
        DontDestroyOnLoad(MapCreator);
        
	}
}
