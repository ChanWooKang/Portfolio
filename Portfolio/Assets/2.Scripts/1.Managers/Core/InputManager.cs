using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Define;


public class InputManager
{
    public Action KeyAction = null;
    public Action<MouseEvent> RightMouseAction = null;
    bool isRPress = false;
    float RPressTime = 0;

    public void OnUpdate()
    {

        if (EventSystem.current.IsPointerOverGameObject()) 
            return;

        if (Input.anyKey && KeyAction != null)
            KeyAction.Invoke();

        if(RightMouseAction != null)
        {
            if (Input.GetMouseButton(1))
            {
                if(isRPress == false)
                {
                    RightMouseAction.Invoke(MouseEvent.PointerDown);
                    RPressTime = Time.time;
                }
                RightMouseAction.Invoke(MouseEvent.Press);
                isRPress = true;
            }
            else
            {
                if (isRPress)
                {
                    if (Time.time > RPressTime + 0.25f)
                        RightMouseAction.Invoke(MouseEvent.Click);
                    RightMouseAction.Invoke(MouseEvent.PointerUp);
                }
                isRPress = false;
                RPressTime = 0;
            }
        }
    }

    public void Clear()
    {
        RightMouseAction = null;
        KeyAction = null;
    }
}
