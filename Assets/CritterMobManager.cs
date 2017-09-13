using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritterMobManager : MonoBehaviour {
    [Tooltip("How many points ahead of players current closest point on track does the mob spawn")]
    public int SpawnPointsLookahead = 2;
    public float SpawnAreaWidth = 1.4f;

    [Tooltip("Odds must add up to 1")]
    public float SmlCritterOdds = 0.5f;
    public int SmlCritterDensity = 20;

    [Tooltip("Odds must add up to 1")]
    public float MedCritterOdds = 0.3f;
    public int MedCritterDensity = 10;

    [Tooltip("Odds must add up to 1")]
    public float LgCritterOdds = 0.18f;
    public int LgCritterDensity = 5;

    [Tooltip("Odds must add up to 1")]
    public float LegendaryCritterOdds = 0.02f;
    public int LegendaryCritterDensity = 3;


    [Tooltip("List index must correspond with TrackSkin Enum in Data class")]
    public List<GameObject> SmlCritters = new List<GameObject>();
    [Tooltip("List index must correspond with TrackSkin Enum in Data class")]
    public List<GameObject> MedCritters = new List<GameObject>();
    [Tooltip("List index must correspond with TrackSkin Enum in Data class")]
    public List<GameObject> LgCritters = new List<GameObject>();

    public List<GameObject> LegendaryCritters = new List<GameObject>();

    public float SecsBetweenSpawns = 10f;

    public GameObject Player;

    public GameObject CritterContainer;
    private List<Vector2> thinnedTrackPoints = new List<Vector2>();

    void OnEnable()
    {
        for(int i = 0; i<Data.Curr_TrackPoints.Count; i += (int)Data.MeshTrackPointFreq)
        {
            thinnedTrackPoints.Add(Data.Curr_TrackPoints[i]);
        }
    }


    private bool Spawned = false;
        // Update is called once per frame
	void Update () {
        if (!Spawned)
            StartCoroutine("SpawnMob");
        
        
	}

    IEnumerator SpawnMob()
    {
        yield return new WaitForSeconds(SecsBetweenSpawns);
        //grab nearest track point to player
        int nearestTrackIndex = ExtensionMethods.GetNearestInList((Vector2)Player.transform.position, thinnedTrackPoints);
        Vector2 NearestPointToPlayer = Data.Curr_TrackPoints[nearestTrackIndex];
        int mobSpawnIndex;
        //makes sure mob spawn points loop across zero index
        if (nearestTrackIndex + SpawnPointsLookahead > thinnedTrackPoints.Count-1)
        {
            mobSpawnIndex = (nearestTrackIndex + SpawnPointsLookahead) - thinnedTrackPoints.Count;
        }
        else
        {
            mobSpawnIndex = nearestTrackIndex + SpawnPointsLookahead;
        }
        Vector2 SpawnCenterPoint = thinnedTrackPoints[mobSpawnIndex];

        List<GameObject> TargetCritterType;
        int TargetCritterDensity;

        //picks the type of critter and density for the current Mob.
        float RandomCritterSelector = Random.value;
        if (RandomCritterSelector < SmlCritterOdds)
        {
            TargetCritterType = SmlCritters;
            TargetCritterDensity = SmlCritterDensity;
        }

        else if (RandomCritterSelector > SmlCritterOdds && RandomCritterSelector < MedCritterOdds)
        {
            TargetCritterType = MedCritters;
            TargetCritterDensity = MedCritterDensity;
        }
        else if (RandomCritterSelector > MedCritterOdds && RandomCritterSelector < LgCritterOdds)
        {
            TargetCritterType = LgCritters;
            TargetCritterDensity = LgCritterDensity;
        }
        else
        {
            TargetCritterType = LegendaryCritters;
            TargetCritterDensity = LegendaryCritterDensity;
        }

        //trigger
        for (int i =0; i < TargetCritterDensity; i++)
        {
            Vector2 SpawnPosition = Random.insideUnitCircle* SpawnAreaWidth;
            GameObject newCritter = Instantiate(TargetCritterType[Data.Curr_TrackSkin],CritterContainer.transform);
            newCritter.transform.position = SpawnPosition;
            newCritter.transform.rotation = Quaternion.Euler(0,0,Random.Range(0,300));
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));    
        }
    }

   



}
