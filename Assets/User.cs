using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    //currency update delegate
    public delegate void CurrencyAdded(int newValue);
    public event CurrencyAdded OnCurrencyAdded;
    
    //player currency
    private int _userCurrency = 1;
    public int UserCurrency
    {
        get
        {
            return _userCurrency;
        }
        set
        {
            _userCurrency = value;
            if (OnCurrencyAdded != null)
            {
                OnCurrencyAdded(value);
            }
        }
    }
}
