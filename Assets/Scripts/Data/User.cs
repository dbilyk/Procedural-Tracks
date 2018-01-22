using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class User : MonoBehaviour {

    void Awake () {
        Environment.SetEnvironmentVariable ("MONO_REFLECTION_SERIALIZER", "yes");
    }

    public readonly int newTrackCoinCost = 100;

    string savedTracksPath = "/playerTracks.dat";

    [SerializeField]
    UI_Skins ui_skins;

    [SerializeField]
    UI_MapSelector mapSelector;

    [SerializeField]
    UI_MapViewer mapViewer;
    [SerializeField]
    CritterMobManager critterMobManager;

    //currency update delegate
    public delegate void CurrencyAdded (int newValue);
    public event CurrencyAdded OnCurrencyAdded;

    public delegate void StartRace ();
    public event StartRace OnStartRace;

    //player currency
    private int _userCurrency = 10000;
    private TrackSkins _currentSkin = TrackSkins.Farm;
    private Track _activeTrack;
    private int _activeGameCurrency = 0;
    private int _opponentQty;

    private static List<Track> _savedTracks = new List<Track> ();
    private static List<sTrack> _serializedTracks = new List<sTrack> ();

    public int UserCurrency {
        get {
            return _userCurrency;
        }
        private set {
            _userCurrency = value;
            if (OnCurrencyAdded != null) {
                OnCurrencyAdded (value);
            }
        }
    }

    public TrackSkins CurrentSkin {
        get {
            return _currentSkin;
        }
        private set {
            _currentSkin = value;
        }
    }

    public Track ActiveTrack {
        get {
            return _activeTrack;
        }
        private set {
            _activeTrack = value;
        }
    }

    public static List<Track> SavedTracks {
        get {
            return _savedTracks;
        }
        private set {
            _savedTracks = value;
        }
    }

    public int OpponentQty { get; private set; }

    public int ActiveGameCoins {
        get {
            return _activeGameCurrency;
        }
        private set {
            _activeGameCurrency = value;
        }
    }

    //private skin setter based on UI input;
    void setSkin (TrackSkins skin) {
        CurrentSkin = skin;
    }
    void setOppQty (float desiredQty, int totalOptionsCt) {
        if (desiredQty != 1) {
            OpponentQty = 2 + Mathf.FloorToInt (desiredQty * (float) totalOptionsCt);

        } else {
            OpponentQty = totalOptionsCt + 1;
        }
        Debug.Log (OpponentQty);
    }

    void OnEnable () {

        //----------------------------------------------------REMOVE, this is just for testing
        OpponentQty = 2;

        ui_skins.OnClickSkin += setSkin;
        mapSelector.OnOpponentCtChanged += setOppQty;
        critterMobManager.OnCritterHit += userHitCritter;
        mapSelector.OnClickStartRace += targetTrack;
        mapSelector.OnClickNewTrackCoins += NewTrackPurchase;

        //if we have some persistent saved tracks, Open them and assign to savedTracks
        if (Open<savedTracks> (savedTracksPath) != null) {
            //populates SavedTracks with the serialized tracks retrieved by the Open() method
            SavedTracks = populateSavedTracks (Open<savedTracks> (savedTracksPath).svdTracks);
            Debug.Log ("Saved tracks populated with persistent data, count is now: " + SavedTracks.Count);
        }

    }

    void OnDisable () {
        ui_skins.OnClickSkin -= setSkin;
    }

    //listens for StartRace btn, and if the requested track is new, it adds it to SavedTracks and saves the new set.
    //if the requested track is a previously saved one, it simply assigns it to this object's CurrentTrack property.
    void targetTrack (Track track, bool isNew) {
        //if the track is not an existing saved track, but a newly rendered one...
        if (isNew) {
            ActiveTrack = track;
            //create a new track instance, add it to
            Track newTrack = new Track (track);
            User.SavedTracks.Insert (0, newTrack);
            savedTracks serialTracks = CreateSerialTracks (SavedTracks);
            //could be slow depending on the number of tracks that are being serialized
            Save<savedTracks> (serialTracks, savedTracksPath);

        } else {
            ActiveTrack = track;
        }
        if (OnStartRace != null) OnStartRace ();

    }
    //creates a serial class out of the passed in tracks list and returns it
    savedTracks CreateSerialTracks (List<Track> tracks) {
        savedTracks serialClass = new savedTracks ();
        for (int i = 0; i < SavedTracks.Count; i++) {
            serialClass.svdTracks.Add (SavedTracks[i].toSerializedTrack ());
        }
        return serialClass;
    }

    //general save method for any object to any path
    public static void Save<T> (T obj, string path) {
        BinaryFormatter bf = new BinaryFormatter ();
        FileStream file;
        if (!File.Exists (Application.persistentDataPath + path)) {
            File.Create (Application.persistentDataPath + path);
        }

        file = File.Open (Application.persistentDataPath + path, FileMode.Open);
        bf.Serialize (file, obj);
        file.Position = 0;
        file.Close ();
    }

    //general open method 
    public static T Open<T> (string path) {
        BinaryFormatter bf = new BinaryFormatter ();
        //check if file exists, if not, return default T
        if (File.Exists (Application.persistentDataPath + path)) {
            FileStream file = File.Open (Application.persistentDataPath + path, FileMode.Open);
            //check if file has contents, if not, return default T
            if (file.Length != 0) {
                file.Position = 0;
                T data = (T) bf.Deserialize (file);
                file.Close ();
                return data;

            } else {
                return default (T);
            }

        } else {
            return default (T);
        }
    }

    List<Track> populateSavedTracks (List<sTrack> serializedTracks) {
        List<Track> result = new List<Track> ();
        foreach (sTrack track in serializedTracks) {
            Track t = new Track ();
            t.DeserializeTrack (track);
            result.Add (t);
        }
        return result;
    }

    void userHitCritter (CritterType type) {
        switch (type) {
            case CritterType.sml:
                UserCurrency += 1;
                break;
            case CritterType.med:
                UserCurrency += 2;
                break;
            case CritterType.lrg:
                UserCurrency += 3;
                break;
            case CritterType.leg:
                UserCurrency += 10;
                break;
        }
    }

    void NewTrackPurchase () {
        UserCurrency -= newTrackCoinCost;
    }
}

[Serializable]
class savedTracks {
    public List<sTrack> svdTracks = new List<sTrack> ();
}