using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour {

    public class MapSettings {
        public float MapWidth { get; } = 80;
        public float MapHeight { get; } = 80;
        public float CornerLerpStep { get; } = 0.1f;
        public int PtsPerQuad { get; } = 500;
        public float PointSpacing { get; } = 3f;
        public

        public int CornerWidth { get; set; } = 1;

    }

    //currency update delegate
    public delegate void CurrencyAdded (int newValue);
    public event CurrencyAdded OnCurrencyAdded;

    //player currency
    private int userCurrency = 1;
    public int UserCurrency {
        get {
            return userCurrency;
        }
        set {
            userCurrency = value;
            OnCurrencyAdded (value);
        }
    }

}