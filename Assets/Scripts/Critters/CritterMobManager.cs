using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CritterType { sml, med, lrg, leg }


class CritterParams{
    public TrackSkins Skin;
    public float SpawnWidth {get;set;}
    public float Odds {get;set;}
    public int Density {get;set;}
    public List<GameObject> Selection{get; set;}
    public List<GameObject> Pool {get;set;}

    CritterParams(TrackSkins skin,List<GameObject> selection, float odds, int density, float spawnWidth){
        this.Skin = skin;
        this.Selection = new List<GameObject> (selection);
        this.Pool = new List<GameObject>();
        this.Odds = odds;
        this.Density = density;
        this.SpawnWidth = spawnWidth;
    }

}

public class CritterMobManager : MonoBehaviour {
    public GameObject CritterContainer;
    [Tooltip ("How many points ahead of players current closest point on track does the mob spawn")]
    public int SpawnPointsLookahead = 2;

    private CritterParams FarmCrits, MtnCrits, DesertCrits, SnowCrits; 

    public float SmlCritterSpawnAreaWidth = 0.5f;
    public float MedCritterSpawnAreaWidth = 0.7f;
    public float LgCritterSpawnAreaWidth = 1f;
    public float LegendarySpawnAreaWidth = 0.5f;

    [SerializeField]
    User user;
    [SerializeField]
    MapRenderer mapRenderer;

    [Tooltip ("Odds must add up to 1")]
    public float SmlCritterOdds = 0.5f;
    public int SmlCritterDensity = 20;

    [Tooltip ("Odds must add up to 1")]
    public float MedCritterOdds = 0.3f;
    public int MedCritterDensity = 10;

    [Tooltip ("Odds must add up to 1")]
    public float LgCritterOdds = 0.18f;
    public int LgCritterDensity = 5;

    [Tooltip ("Odds must add up to 1")]
    public float LegendaryCritterOdds = 0.02f;
    public int LegendaryCritterDensity = 3;

    [Tooltip ("List index must correspond with TrackSkin Enum in Data class")]
    public List<GameObject> smlCritterSelection = new List<GameObject> ();
    [Tooltip ("List index must correspond with TrackSkin Enum in Data class")]
    public List<GameObject> medCritters = new List<GameObject> ();
    [Tooltip ("List index must correspond with TrackSkin Enum in Data class")]
    public List<GameObject> lgCritters = new List<GameObject> ();

    public List<GameObject> LegendaryCritters = new List<GameObject> ();

    public float SecsBetweenSpawns = 10f;

    public GameObject Player;
    Camera cam;
    private List<GameObject> SmlCritterPool = new List<GameObject> ();
    private List<GameObject> MedCritterPool = new List<GameObject> ();
    private List<GameObject> LgCritterPool = new List<GameObject> ();
    private List<GameObject> LegendaryCritterPool = new List<GameObject> ();
    private List<Vector2> thinnedTrackPoints = new List<Vector2> ();
    private int LastNearestPointToPlayer;

    //this delegate is called from any animal controller and is also the signature for broadcasting the event from one central location;
    public delegate void critterHit (CritterType type);
    void _critterHit (CritterType type) {
        Debug.Log ("critter hit via delegate in AnimalController, type: " + type.ToString ());
        if (OnCritterHit != null) {
            OnCritterHit (type);
        }
    }
    public critterHit CritterHit;

    public event critterHit OnCritterHit;

    void OnEnable () {
        

        CritterHit = new critterHit (_critterHit);
        cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
        Spawned = false;
        for (int i = 0; i < user.ActiveTrack.TrackPoints.Count; i += Mathf.RoundToInt ((int) mapRenderer.MeshTrackPointFreq / 4)) {
            thinnedTrackPoints.Add (user.ActiveTrack.TrackPoints[i]);
        }
    }

    private bool Spawned = false;
    void Update () {
        if (!Spawned) {
            StartCoroutine ("SpawnMob");
            Spawned = true;
        }

    }
    
    


    IEnumerator SpawnMob () {
        yield return new WaitForSeconds (Random.Range (SecsBetweenSpawns - 1f, SecsBetweenSpawns + 1f));
        //grab nearest track point to player
        int nearestTrackIndex = ExtensionMethods.GetNearestInList ((Vector2) Player.transform.position, thinnedTrackPoints);
        Vector2 NearestPointToPlayer = user.ActiveTrack.TrackPoints[nearestTrackIndex];
        if (nearestTrackIndex == LastNearestPointToPlayer) {
            Spawned = false;
            yield break;
        }
        LastNearestPointToPlayer = nearestTrackIndex;
        int mobSpawnIndex;
        //makes sure mob spawn points loop across zero index
        mobSpawnIndex = (nearestTrackIndex + SpawnPointsLookahead) % thinnedTrackPoints.Count;
        
        Vector2 SpawnCenterPoint = thinnedTrackPoints[mobSpawnIndex];
        SpawnCenterPoint = SpawnCenterPoint + Random.insideUnitCircle;

        List<GameObject> TargetCritterType = new List<GameObject> ();
        List<GameObject> TargetCritterPool = new List<GameObject> ();
        int TargetCritterDensity;
        float targetSpawnAreaWidth;
        CritterType critterType;

        // void setTargetParams(){
        //     Debug.Log("hi");
        // }

        //picks the type of critter and density for the current Mob.
        float RandomCritterSelector = Random.value;
        if (RandomCritterSelector < SmlCritterOdds) {
            TargetCritterType = SmlCritters;
            TargetCritterDensity = SmlCritterDensity;
            targetSpawnAreaWidth = SmlCritterSpawnAreaWidth;
            TargetCritterPool = SmlCritterPool;
            critterType = CritterType.sml;
        } else if (RandomCritterSelector > SmlCritterOdds && RandomCritterSelector < SmlCritterOdds + MedCritterOdds) {
            TargetCritterType = MedCritters;
            TargetCritterDensity = MedCritterDensity;
            targetSpawnAreaWidth = MedCritterSpawnAreaWidth;
            TargetCritterPool = MedCritterPool;
            critterType = CritterType.med;
        } else if (RandomCritterSelector > MedCritterOdds + SmlCritterOdds && RandomCritterSelector < LgCritterOdds + MedCritterOdds + SmlCritterOdds) {
            TargetCritterType = LgCritters;
            TargetCritterDensity = LgCritterDensity;
            targetSpawnAreaWidth = LgCritterSpawnAreaWidth;
            TargetCritterPool = LgCritterPool;
            critterType = CritterType.lrg;
        } else {
            TargetCritterType = LegendaryCritters;
            TargetCritterDensity = LegendaryCritterDensity;
            targetSpawnAreaWidth = LegendarySpawnAreaWidth;
            TargetCritterPool = LegendaryCritterPool;
            critterType = CritterType.leg;
        }

        for (int i = 0; i < TargetCritterPool.Count; i++) {
            Vector2 AnimalInScreenCoords = cam.WorldToScreenPoint (TargetCritterPool[i].transform.position);
            if ((AnimalInScreenCoords.x > Screen.width + 50 || AnimalInScreenCoords.y > Screen.height + 50) && (AnimalInScreenCoords.x < -50 || AnimalInScreenCoords.y < -50)) {
                GameObject CritterChild = TargetCritterPool[i].transform.GetChild (0).gameObject;
                CritterChild.SetActive (false);
            }
        }
        

        //trigger
        for (int i = 0; i < TargetCritterDensity; i++) {
            GameObject newCritter;
            Vector2 SpawnPosition = Random.insideUnitCircle * targetSpawnAreaWidth;

            if (TargetCritterPool.Count < TargetCritterDensity) {
                newCritter = Instantiate (TargetCritterType[(int) user.CurrentSkin], CritterContainer.transform);
                AnimalController critterCtrl = newCritter.GetComponentInChildren<AnimalController> ();
                critterCtrl.MyType = critterType;
                critterCtrl.critterMobManager = gameObject.GetComponent<CritterMobManager> ();
                TargetCritterPool.Add (newCritter);
            } else {
                newCritter = TargetCritterPool.Find (x => x.transform.GetChild (0).gameObject.activeInHierarchy == false);
                //if there's not enough disabled critters after the check, add some more to the pool
                if (newCritter == null) {
                    newCritter = Instantiate (TargetCritterType[(int) user.CurrentSkin], CritterContainer.transform);
                    AnimalController critterCtrl = newCritter.GetComponentInChildren<AnimalController> ();
                    critterCtrl.MyType = critterType;
                    critterCtrl.critterMobManager = gameObject.GetComponent<CritterMobManager> ();
                    TargetCritterPool.Add (newCritter);
                }
            }

            newCritter.transform.GetChild (0).gameObject.SetActive (true);
            newCritter.transform.position = SpawnPosition + thinnedTrackPoints[mobSpawnIndex];
            newCritter.transform.rotation = Quaternion.Euler (0, 0, Random.Range (0, 300));
            yield return new WaitForSeconds (0.1f);
        }
        Spawned = false;

    }

}