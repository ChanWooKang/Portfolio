using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Define;


public class InputManager
{
    public Action KeyAction = null;
    public Action<MouseEvent> MouseAction = null;
    bool isRPress = false;
    float RPressTime = 0;

    public void OnUpdate()
    {

        if (EventSystem.current.IsPointerOverGameObject()) 
            return;

        if (Input.anyKey && KeyAction != null)
            KeyAction.Invoke();

        if(MouseAction != null)
        {
            if (Input.GetMouseButton(0))
            {
                if(isRPress == false)
                {
                    MouseAction.Invoke(MouseEvent.PointerDown);
                    RPressTime = Time.time;
                }
                MouseAction.Invoke(MouseEvent.Press);
                isRPress = true;
            }
            else
            {
                if (isRPress)
                {
                    if (Time.time > RPressTime + 0.25f)
                        MouseAction.Invoke(MouseEvent.Click);
                    MouseAction.Invoke(MouseEvent.PointerUp);
                }
                isRPress = false;
                RPressTime = 0;
            }
        }

    }

    public void Clear()
    {
        MouseAction = null;
        KeyAction = null;
    }
}
