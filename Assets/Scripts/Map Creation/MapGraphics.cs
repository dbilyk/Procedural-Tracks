using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGraphics : MonoBehaviour {
    public GameObject Dirt;
    public float DirtSpread;
    public GameObject Oil;
    public float OilSpread;
    public List<GameObject> Foliage;
    public float FoliageSpread;

    public GameObject DirtContainer;
    public GameObject OilContainer;
    public GameObject FoliageContainer;

    public MapCreator MapGen;
    public List<Vector2> MapPoints;
    public List<GameObject> MeshHelperEmpties;

    public float DirtFreq;
    public float OilFreq;
    public float FoliageFreq;
    // Use this for initialization
    void Start () {
        MapPoints = Data.Curr_TrackPoints;
        MeshHelperEmpties = MapGen.MeshHelperObjects;
        Debug.Log("num of map points: " + MapPoints.Count);
        for(int i = 0; i< MapPoints.Count; i++)
        {
            float dirtDice = Random.value;
            float oilDice = Random.value;
            //dirt spawn
            if(dirtDice <= DirtFreq)
            {
               GameObject newDirt = Instantiate(Dirt,DirtContainer.transform);
                newDirt.transform.position = MapPoints[i] + (Random.insideUnitCircle * DirtSpread);

            }
            //oil spawn
            if (oilDice <= OilFreq)
            {
                float size = Random.Range(0.2f, 0.5f);
                GameObject newOil = Instantiate(Oil, OilContainer.transform);
                newOil.transform.eulerAngles = new Vector3(0,0, Random.value * 360f);
                newOil.transform.localScale = new Vector2(size,size);
                newOil.transform.position = MapPoints[i] + (new Vector2() * OilSpread);
            }
            
        }

        for (int i = 0; i < MeshHelperEmpties.Count; i++)
        {
            float foliageDice = Random.value;
            int foliageSpriteIndex = Random.Range(0,Foliage.Count-1);
            //foliage spawn
            if (foliageDice <= FoliageFreq)
            {
                GameObject newFoliage = Instantiate(Foliage[foliageSpriteIndex], OilContainer.transform);
                newFoliage.transform.rotation = MeshHelperEmpties[i].transform.rotation;
                if (Random.value > 0.5f)
                {
                    newFoliage.transform.position = MeshHelperEmpties[i].transform.position + (MeshHelperEmpties[i].transform.up * FoliageSpread * Random.Range(1f, 2.5f));
                }
                else
                {
                    newFoliage.transform.position = MeshHelperEmpties[i].transform.position + (MeshHelperEmpties[i].transform.up * -FoliageSpread * Random.Range(1f, 2.5f));
                }
            }
        }

    }

    // Update is called once per frame
    void Update () {
		
	}
}
