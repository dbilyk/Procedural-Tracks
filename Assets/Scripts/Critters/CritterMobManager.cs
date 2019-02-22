using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CritterType { sml, med, lrg, leg }


public class CritterParams{
    public TrackSkins Skin;
    public float[] SpawnWidths {get;set;}
    //from small to legendary, must add up to 1
    public float[] Odds {get;set;}
    public int[] Densities {get;set;}
    public List<GameObject> Critters{get; set;}
    public List<GameObject>[] Pools {get;set;}

    public CritterParams(TrackSkins skin,List<GameObject> critters, float[] odds, int[] densities, float[] spawnWidths){
        this.Skin = skin;
        this.Critters = new List<GameObject> (critters);
        this.Pools = new List<GameObject>[] {new List<GameObject>(),new List<GameObject>(),new List<GameObject>(),new List<GameObject>()};
        this.Odds = odds;
        this.Densities = densities;
        this.SpawnWidths = spawnWidths;
    }

}

public class CritterMobManager : MonoBehaviour {
    //these contain all the params for critters per skin, arrays are sorted from small criter to legendary critter.
    private CritterParams FarmCrits, MtnCrits, DesertCrits, SnowCrits, CurrentCrits; 
    
    [Tooltip ("How many points ahead of players current closest point on track does the mob spawn")]
    public int SpawnPointsLookahead = 2;
    public float SecsBetweenSpawns = 10f;

    private int LastNearestPointToPlayer;

    [SerializeField]
    User user;

    [SerializeField]
    MapRenderer mapRenderer;

    public GameObject CritterContainer;


    public GameObject Player;
    Camera cam;
    //critter object lists for different skins.
    public List<GameObject> 
    FarmCritters = new List<GameObject> (), 
    MtnCritters = new List<GameObject> (),
    DesertCritters = new List<GameObject> (),
    SnowCritters = new List<GameObject> ();
    
    private List<Vector2> thinnedTrackPoints = new List<Vector2> ();

    //this delegate is called from any animal controller and is also the signature for broadcasting the event from one central location;
    public delegate void critterHit (CritterType type);
    void _critterHit (CritterType type) {
        if (OnCritterHit != null) {
            OnCritterHit (type);
        }
    }
    public critterHit CritterHit;

    public event critterHit OnCritterHit;

    //TO DO: AFTER A RACE IS DONE, THIS G.O. MUST BE DISABLED
    void OnEnable () {
        //these are the params for each skin, adjust here if necessary.
        FarmCrits   = new CritterParams(TrackSkins.Farm,FarmCritters,  new float[]{0.5f,0.25f,0.2f,0.05f},new int[]{10,7,4,1},new float[]{1f,0.7f,0.5f,0.4f});
        MtnCrits    = new CritterParams(TrackSkins.Farm,MtnCritters,   new float[]{0.5f,0.25f,0.2f,0.05f},new int[]{10,7,4,1},new float[]{1f,0.7f,0.5f,0.4f});
        DesertCrits = new CritterParams(TrackSkins.Farm,DesertCritters,new float[]{0.5f,0.25f,0.2f,0.05f},new int[]{10,7,4,1},new float[]{1f,0.7f,0.5f,0.4f});
        SnowCrits   = new CritterParams(TrackSkins.Farm,SnowCritters,  new float[]{0.5f,0.25f,0.2f,0.05f},new int[]{10,7,4,1},new float[]{1f,0.7f,0.5f,0.4f});
        
        //now we can just deal with the current crits in the rest of the logic.
        switch(user.CurrentSkin){
            case TrackSkins.Farm:
                CurrentCrits = FarmCrits;
                break;
            case TrackSkins.Mountains:
                CurrentCrits = MtnCrits;
                break;
            case TrackSkins.Desert:
                CurrentCrits = DesertCrits;
                break;
            case TrackSkins.Snow:
                CurrentCrits = SnowCrits;
                break;
        }

        CritterHit = new critterHit (_critterHit);
        cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
        Spawned = false;
        for (int i = 0; i < user.ActiveTrack.TrackPoints.Count; i += Mathf.RoundToInt ((int) mapRenderer.MeshTrackPointFreq / 4)) {
            thinnedTrackPoints.Add (user.ActiveTrack.TrackPoints[i]);
        }
        StartCoroutine(InstantiateCrittersInPool());

    }

    IEnumerator InstantiateCrittersInPool(){
        for(int i =0; i<CurrentCrits.Pools.Length;i++){
            for(int j=0; j<CurrentCrits.Densities[i];j++){
                yield return new WaitForEndOfFrame();
                GameObject newCritter = Instantiate (CurrentCrits.Critters[i], CritterContainer.transform);
                AnimalController critterCtrl = newCritter.GetComponentInChildren<AnimalController> ();
                critterCtrl.critterMobManager = gameObject.GetComponent<CritterMobManager> ();
                newCritter.transform.GetChild(0).gameObject.SetActive(false);
                CurrentCrits.Pools[i].Add (newCritter);

                
            }
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

        //if its the same point and before, do nothing.
        if (nearestTrackIndex == LastNearestPointToPlayer) {
            Spawned = false;
            yield break;
        }
        LastNearestPointToPlayer = nearestTrackIndex;

        //makes sure mob spawn points loop across zero index
        int mobSpawnIndex = (nearestTrackIndex + SpawnPointsLookahead) % thinnedTrackPoints.Count;
        
        Vector2 SpawnCenterPoint = thinnedTrackPoints[mobSpawnIndex];
        SpawnCenterPoint = SpawnCenterPoint + Random.insideUnitCircle;

        //this nubmer determines if were dealing with sml,med,lrg, or leg critters.
        int targetType;

        //picks the type of critter and density for the current Mob.
        float random = Random.value;
        if (random < CurrentCrits.Odds[0]) {
            targetType = (int)CritterType.sml;
            
        } else if (random > CurrentCrits.Odds[0] && random < CurrentCrits.Odds[0] + CurrentCrits.Odds[1]) {
            targetType = (int)CritterType.med;

        } else if (random > CurrentCrits.Odds[0] + CurrentCrits.Odds[1] && random < CurrentCrits.Odds[0] + CurrentCrits.Odds[1] + CurrentCrits.Odds[1]) {
            targetType = (int)CritterType.lrg;

        } else {
            targetType = (int)CritterType.leg;
            
        }

        ViewportRect viewport = new ViewportRect(new Vector2(0,0),new Vector2(cam.pixelWidth,0),new Vector2(0,cam.pixelHeight),new Vector2(cam.pixelWidth,cam.pixelHeight));
        
        //go through the relevant pool and check which critters are out of view and disable.
        for (int i = 0; i < CurrentCrits.Pools[targetType].Count; i++) {
            Vector2 thisCritter = cam.WorldToScreenPoint (CurrentCrits.Pools[targetType][i].transform.position);
            
            
            if (!ExtensionMethods.PointInRectangle(thisCritter,viewport.A,viewport.B,viewport.C,viewport.D)) {
                GameObject outOfViewCrit = CurrentCrits.Pools[targetType][i].transform.GetChild (0).gameObject;
                outOfViewCrit.SetActive (false);
            }
        }
        
        

        //trigger
        for (int i = 0; i < CurrentCrits.Densities[targetType]; i++) {
            GameObject newCritter;
            Vector2 SpawnPosition = Random.insideUnitCircle * CurrentCrits.SpawnWidths[targetType];

            if (CurrentCrits.Pools[targetType].Count < CurrentCrits.Densities[targetType]) {
                newCritter = Instantiate (CurrentCrits.Critters[targetType], CritterContainer.transform);
                AnimalController critterCtrl = newCritter.GetComponentInChildren<AnimalController> ();
                critterCtrl.critterMobManager = gameObject.GetComponent<CritterMobManager> ();
                CurrentCrits.Pools[targetType].Add (newCritter);

            } else {
                newCritter = CurrentCrits.Pools[targetType].Find (x => x.transform.GetChild (0).gameObject.activeInHierarchy == false);
                //if there's not enough disabled critters after the check, add some more to the pool
                if (newCritter == null) {
                    newCritter = Instantiate (CurrentCrits.Critters[targetType], CritterContainer.transform);
                    AnimalController critterCtrl = newCritter.GetComponentInChildren<AnimalController> ();
                    critterCtrl.critterMobManager = gameObject.GetComponent<CritterMobManager> ();
                    CurrentCrits.Pools[targetType].Add (newCritter);
                }
            }

            newCritter.transform.GetChild (0).gameObject.SetActive (true);
            newCritter.transform.position = SpawnPosition + thinnedTrackPoints[mobSpawnIndex];
            newCritter.transform.rotation = Quaternion.Euler (0, 0, Random.Range (0, 300));
            yield return new WaitForSeconds (0.1f);
        }
        Spawned = false;

    }
    public struct ViewportRect{
        public Vector2 A,B,C,D;
        public ViewportRect(Vector2 a, Vector2 b, Vector2 c, Vector2  d){
            this.A = a;
            this.B= b;
            this.C = c;
            this.D = d;

        }
    }

}