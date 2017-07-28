using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    public InputManager Data;


    public float GetSteering()
    {
        float val = 0;
        if (Input.touchCount > 0)
        {
            val = -(Input.touches[0].position.x - (Screen.width / 2)) / (Screen.width / 2);
        }
        else if (Input.anyKey)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
                val = 1f;
            if (Input.GetKey(KeyCode.RightArrow))
                val = -1f;
        }
        else
        {
            val = 0;
        }

        return val;
    }

    public bool GetBraking()
    {
        if (Input.touchCount > 1 || Input.GetKey(KeyCode.B))
            return true;
        else
            return false;
    }

    public bool GetAccel(DeviceType deviceType)
    {
        if (deviceType == DeviceType.Handheld || Input.GetKey(KeyCode.Space))
            return true;
        else
            return false;


    }

   void Awake()
    {
        if (_instance == null)
        {
            _instance = new InputManager();
            Data = _instance;
        }
        else
        {
            Data = _instance;
        }
    }
    
}