using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    public InputManager Data;
    public float steeringTouchWidth = 200;
    private float _steering;
    private bool _accel;
    private bool _brake;

    public float GetSteering()
    {
        float val = 0;
        if (Input.touchCount >0 && Input.touches[0].position.x <= steeringTouchWidth)
        {
            val = -(Input.touches[0].position.x - (steeringTouchWidth / 2)) / (steeringTouchWidth/ 2);
            //val = -((SteeringSlider.value - 0.5f) *2);
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
        //if (Input.touchCount == 2 || Input.GetKey(KeyCode.B)) {
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
        return _brake;
    }

    public bool GetAccel()
    {
        if (Input.touchCount > 1 || Input.GetKey(KeyCode.Space))
            return true;
        else
            return false;
       

    }
    public bool GetReverse()
    {
        if (Input.touchCount > 1 && Input.touches[1].tapCount == 2  || Input.GetKey(KeyCode.R))
        {
            return true;
        }
        else
        {
            return false;
        }
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
    

   
}