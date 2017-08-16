using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    public InputManager Data;
    public Slider SteeringSlider; 
    private float _steering;
    private bool _accel;
    private bool _brake;
    public void AccelButtonDown()
    {
        _accel = true;
    }
    public void AccelButtonUp()
    {
        _accel = false;
    }
    public void BrakeButtonDown()
    {
        _brake = true;
        Debug.Log("BRAKE");
    }
    public void BrakeButtonUp()
    {
        _brake = false;
    }
    public void SetSteering() 
    {
        _steering = SteeringSlider.value;
    }
    public void ReleaseSteering()
    {

    }


    public float GetSteering()
    {
        float val = 0;
        if (Input.touchCount > 0)
        {
            //val = -(Input.touches[0].position.x - (Screen.width / 2)) / (Screen.width / 2);
            val = -((SteeringSlider.value - 0.5f) *2);
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
        //if (Input.touchCount == 1 || Input.GetKey(KeyCode.Space))
        //    return true;
        //else
        //    return false;
        return _accel;

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