using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Define;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    }
    public static void DestroyAPS(this GameObject go)
    {
        PoolingManager.DestroyAPS(go);
    }
    public static void BindEvent(this GameObject go, Action<PointerEventData> action, UIEvent type = UIEvent.Click) { UI_Base.BindEvent(go, action, type); }
}
