using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoliageCreator : MonoBehaviour {

    public List<GameObject> Foliage = new List<GameObject>();

    public List<Vector2> InnerFoliagePath;
    public List<Vector2> OuterFoliagePath;

    public List<GameObject> InstantiatedFoliage = new List<GameObject>();
    public float FoliageDensity;
    public BarrierCreator barrierCreator;
    public MapCreator mapCreator;
	// Use this for initialization
	void OnEnable () {
        InnerFoliagePath = barrierCreator.CreateOutline(Data.Curr_RawPoints,Data.InnerBarrierOffset +1f, "inner");
        InnerFoliagePath = mapCreator.CreateTrackPoints(InnerFoliagePath,10);
        OuterFoliagePath = barrierCreator.CreateOutline(Data.Curr_RawPoints,Data.InnerBarrierOffset +1f, "outer");
        OuterFoliagePath = mapCreator.CreateTrackPoints(OuterFoliagePath, 10);

        for (int i = 0; i < InnerFoliagePath.Count; i ++)
        {
            float rand = Random.value;

            if (rand < FoliageDensity)
            {
                int randIndex = Random.Range(0, Foliage.Count);
                GameObject newItem = GameObject.Instantiate(Foliage[randIndex],gameObject.transform);
                newItem.transform.position = InnerFoliagePath[i] + Random.insideUnitCircle * 0.2f;
                InstantiatedFoliage.Add(newItem);
            }
        }
        for (int i = 0; i < OuterFoliagePath.Count; i++)
        {
            float rand = Random.value;

            if (rand < FoliageDensity)
            {
                int randIndex = Random.Range(0, Foliage.Count);
                GameObject newItem = GameObject.Instantiate(Foliage[randIndex], gameObject.transform);
                newItem.transform.position = OuterFoliagePath[i] + Random.insideUnitCircle * 0.2f;
                InstantiatedFoliage.Add(newItem);

            }
        } 
    }
    void OnDisable()
    {
        foreach (GameObject GO in InstantiatedFoliage)
        {
            GameObject.Destroy(GO);
        }

    }
	
	
}
