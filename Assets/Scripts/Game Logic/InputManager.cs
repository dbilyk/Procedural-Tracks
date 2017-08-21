using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    public InputManager Data;
    public Image SteeringStaticUI;
    public Image SteeringIndicatorUI;
    public float steeringTouchWidth;
    private int steeringFingerID = -1;
    private int steeringCenterPosition = 0;
    private bool reverseEngaged = false;
    

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
                SteeringStaticUI.gameObject.SetActive(true);
                //SteeringStaticUI.rectTransform.position = new Vector2(steeringCenterPosition, Input.touches[i].position.y+30);
                break;
            }

            if(Input.touches[i].fingerId == steeringFingerID)
            {
                val = -Mathf.Clamp((Input.touches[i].position.x - steeringCenterPosition) / (steeringTouchWidth / 2),-1,1);
                SteeringIndicatorUI.rectTransform.position = new Vector2(SteeringStaticUI.rectTransform.position.x - (val*(steeringTouchWidth/2)), SteeringIndicatorUI.rectTransform.position.y);
            }
            if (Input.touches[i].phase == TouchPhase.Ended && steeringFingerID == Input.touches[i].fingerId)
            {
                //SteeringIndicatorUI.rectTransform.position = Vector2.zero;
                SteeringStaticUI.gameObject.SetActive(false);
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

    public bool GetAccel()
    {
        bool engaged = false;
        for(int i = 0; i <Input.touchCount; i++)
        {
            if (Input.touches[i].fingerId != steeringFingerID && Input.touches[i].position.x > Screen.width/2 && !reverseEngaged)
            {
                engaged = true;
                
            }
        }
        if (Input.GetKey(KeyCode.Space) && Input.touchCount ==0)
        {
            engaged = true;
        }
        
        return engaged;
       

    }
    public bool GetReverse()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.touches[i].fingerId != steeringFingerID && Input.touches[i].position.x > Screen.width / 2)
            {
                if (Input.touches[i].tapCount  == 2)
                {
                    reverseEngaged = true;
                    return true;
                }
                else
                {
                    reverseEngaged = false;
                    return false;
                }
            }
        }
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
    

   
}