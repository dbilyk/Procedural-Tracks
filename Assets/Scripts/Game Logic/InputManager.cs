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
        if (Input.touchCount == 2 || Input.GetKey(KeyCode.B)) {
        return true;
    }
        else{
            return false;
    }
    }

    public bool GetAccel()
    {
        if (Input.touchCount == 1 || Input.GetKey(KeyCode.Space))
            return true;
        else
            return false;


    }

   void Awake()
    {
        if (_instance == null)
        {
           
            Data =this;
        }
        else
        {
            Data = _instance;
        }
    }
    

    void OnGUI()
    {
        GUI.TextField(new Rect(50,50,140,20),"Touch Count: " + Input.touchCount);
    }
}