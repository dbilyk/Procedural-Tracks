using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritterMobManager : MonoBehaviour {
    [Tooltip("How many points ahead of players current closest point on track does the mob spawn")]
    public int SpawnPointsLookahead = 2;
    public float SmlCritterSpawnAreaWidth = 0.5f;
    public float MedCritterSpawnAreaWidth = 0.7f;
    public float LgCritterSpawnAreaWidth = 1f;
    public float LegendarySpawnAreaWidth = 0.5f;

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
    private int LastNearestPointToPlayer;
    void OnEnable()
    {
        Spawned = false;
        for(int i = 0; i<Data.Curr_TrackPoints.Count; i += Mathf.RoundToInt((int)Data.MeshTrackPointFreq/4))
        {
            thinnedTrackPoints.Add(Data.Curr_TrackPoints[i]);
        }
    }


    private bool Spawned = false;
	void Update () {
        if (!Spawned)
        {
            StartCoroutine("SpawnMob");
            Spawned = true;
        }
        
	}

    IEnumerator SpawnMob()
    {
        yield return new WaitForSeconds(Random.Range(SecsBetweenSpawns-1f, SecsBetweenSpawns+1f));
        //grab nearest track point to player
        int nearestTrackIndex = ExtensionMethods.GetNearestInList((Vector2)Player.transform.position, thinnedTrackPoints);
        Vector2 NearestPointToPlayer = Data.Curr_TrackPoints[nearestTrackIndex];
        if (nearestTrackIndex == LastNearestPointToPlayer)
        {
            Spawned = false;
            yield break;
        }
        LastNearestPointToPlayer = nearestTrackIndex;
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
        SpawnCenterPoint = new Vector2(SpawnCenterPoint.x + Random.Range(-1,1),SpawnCenterPoint.y + Random.Range(-1,1));


        List<GameObject> TargetCritterType = new List<GameObject>();
        int TargetCritterDensity;
        float targetSpawnAreaWidth;

        //picks the type of critter and density for the current Mob.
        float RandomCritterSelector = Random.value;
        if (RandomCritterSelector < SmlCritterOdds)
        {
            TargetCritterType = SmlCritters;
            TargetCritterDensity = SmlCritterDensity;
            targetSpawnAreaWidth = SmlCritterSpawnAreaWidth;
        }

        else if (RandomCritterSelector > SmlCritterOdds && RandomCritterSelector < MedCritterOdds)
        {
            TargetCritterType = MedCritters;
            TargetCritterDensity = MedCritterDensity;
            targetSpawnAreaWidth = MedCritterSpawnAreaWidth;
        }
        else if (RandomCritterSelector > MedCritterOdds && RandomCritterSelector < LgCritterOdds)
        {
            TargetCritterType = LgCritters;
            TargetCritterDensity = LgCritterDensity;
            targetSpawnAreaWidth = LgCritterSpawnAreaWidth;
        }
        else
        {
            TargetCritterType = LegendaryCritters;
            TargetCritterDensity = LegendaryCritterDensity;
            targetSpawnAreaWidth = LegendarySpawnAreaWidth;
        }

        //trigger
        for (int i =0; i < TargetCritterDensity; i++)
        {
            
            Vector2 SpawnPosition = Random.insideUnitCircle* targetSpawnAreaWidth;
            GameObject newCritter = Instantiate(TargetCritterType[Data.Curr_TrackSkin],CritterContainer.transform);
            newCritter.transform.position = SpawnPosition + thinnedTrackPoints[mobSpawnIndex];
            newCritter.transform.rotation = Quaternion.Euler(0,0,Random.Range(0,300));
            yield return new WaitForSeconds(0.1f);
        }
        Spawned = false;
    }

   



}
