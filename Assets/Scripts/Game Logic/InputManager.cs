using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    public InputManager Data;
    public float steeringTouchWidth;
    private int steeringFingerID = -1;
    private int steeringCenterPosition = 0;
    private bool _accel;
    private bool _brake;

    public float GetSteering()
    {
        float val = 0;
        for (int i = 0; i < Input.touchCount; i ++)
        {
            if (Input.touches[i].phase == TouchPhase.Began && Input.touches[i].position.x < Screen.width/2)
            {
                val = 0;
                steeringFingerID = Input.touches[i].fingerId;
                steeringCenterPosition = Mathf.RoundToInt(Input.touches[i].position.x);
                break;
            }

            if(Input.touches[i].fingerId == steeringFingerID)
            {
                val = -Mathf.Clamp((Input.touches[i].position.x - steeringCenterPosition) / (steeringTouchWidth / 2),-1,1);
            }
            if (Input.touches[i].phase == TouchPhase.Ended && steeringFingerID == Input.touches[i].fingerId)
            {
                steeringFingerID = -1;
                steeringCenterPosition = 0;
            }
        }
        
        if (Input.anyKey)
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
        bool engaged = false;
        for(int i = 0; i <Input.touchCount; i++)
        {
            if (Input.touches[i].fingerId != steeringFingerID && Input.touches[i].position.x > Screen.width/2)
            {
                engaged = true;
                return true;
            }
        }
        if (Input.GetKey(KeyCode.Space))
        {
            engaged = true;
        }
        else
        {
            engaged = false;
        }
        return engaged;
       

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