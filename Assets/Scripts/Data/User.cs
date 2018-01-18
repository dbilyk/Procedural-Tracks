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

    //player currency
    private int _userCurrency = 10000;
    private TrackSkins _currentSkin = TrackSkins.Farm;
    private Track _currentTrack;

    private static List<Track> _savedTracks = new List<Track> ();
    private static List<sTrack> _serializedTracks = new List<sTrack> ();

    void OnEnable () {

        ui_skins.OnClickSkin += setSkin;
        critterMobManager.OnCritterHit += userHitCritter;
        mapSelector.OnClickStartRace += addSavedTrack;
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

    public Track CurrentTrack {
        get {
            return _currentTrack;
        }
        private set {
            _currentTrack = value;
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

    //private skin setter based on UI input;
    void setSkin (TrackSkins skin) {
        CurrentSkin = skin;
    }
    void setCurrentTrack (Track track) {
        CurrentTrack = track;
    }
    void addSavedTrack (Track track) {
        Track newTrack = new Track (track);
        if (SavedTracks.Count == 0 || SavedTracks[0].ControlPoints != track.ControlPoints) {
            User.SavedTracks.Insert (0, newTrack);

            //could be slow depending on the number of tracks that are being serialized
            savedTracks serialClass = new savedTracks ();
            for (int i = 0; i < SavedTracks.Count; i++) {
                serialClass.svdTracks.Add (SavedTracks[i].toSerializedTrack ());
            }
            Save<savedTracks> (serialClass, savedTracksPath);
        }

    }

    public void Save<T> (T obj, string path) {
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

    public T Open<T> (string path) {
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