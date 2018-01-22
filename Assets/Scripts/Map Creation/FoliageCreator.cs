using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoliageCreator : MonoBehaviour {
    [SerializeField]
    MapRenderer mapRenderer;

    [SerializeField]
    User user;

    [SerializeField]
    List<GameObject> Foliage = new List<GameObject> ();

    [SerializeField]
    float FoliageDensity = 0.5f;

    [SerializeField]
    BarrierCreator barrierCreator;
    [SerializeField]
    MapCreator mapCreator;
    // Use this for initialization
    public void GenerateFoliageData (Track track) {
        List<Vector2> InnerFoliagePath = barrierCreator.CreateOutline (track.RawPoints, mapRenderer.InnerBarrierOffset + 1f, "inner");
        InnerFoliagePath = mapCreator.CreateTrackPoints (InnerFoliagePath, 10);
        track.InnerFoliageLocs = InnerFoliagePath;
        List<Vector2> OuterFoliagePath = barrierCreator.CreateOutline (track.RawPoints, mapRenderer.OuterBarrierOffset + 1f, "outer");
        OuterFoliagePath = mapCreator.CreateTrackPoints (OuterFoliagePath, 10);
        track.OuterFoliageLocs = OuterFoliagePath;
        Debug.Log ("fol" + InnerFoliagePath.Count);
    }

    public void RenderFoliage (Track track) {

        for (int i = 0; i < track.InnerFoliageLocs.Count; i++) {
            float rand = Random.value;

            if (rand < FoliageDensity) {
                int randIndex = Random.Range (0, Foliage.Count);
                GameObject newItem = GameObject.Instantiate (Foliage[randIndex], gameObject.transform);
                newItem.transform.rotation = Random.rotation;
                newItem.transform.position = track.InnerFoliageLocs[i] + Random.insideUnitCircle * 0.2f;
            }
        }
        for (int i = 0; i < track.OuterFoliageLocs.Count; i++) {
            float rand = Random.value;

            if (rand < FoliageDensity) {
                int randIndex = Random.Range (0, Foliage.Count);
                GameObject newItem = Instantiate (Foliage[randIndex], gameObject.transform);
                newItem.transform.position = track.OuterFoliageLocs[i] + Random.insideUnitCircle * 0.2f;
                newItem.transform.rotation = Random.rotation;

            }
        }
        StaticBatchingUtility.Combine (gameObject);
    }

}