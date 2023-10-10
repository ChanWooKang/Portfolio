using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager
{
    int _order = 10;
    public Action OnSetUIEvent = null;
    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UIs");
            if (root == null)
                root = new GameObject { name = "@UIs" };
            return root;
        }
    }

    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = sort;

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public T MakeWorldSpace<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        //GameObject go = Managers._resource.Instantiate($"UI/WorldSpace/{name}");
        GameObject go = PoolingManager._pool.InstantiateAPS(name);

        if (parent != null)
            go.transform.SetParent(parent);
        else
            go.transform.SetParent(Managers._ui.Root.transform);

        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return Util.GetOrAddComponent<T>(go);
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers._resource.Instantiate($"UI/SubItem/{name}");

        if (parent != null)
            go.transform.SetParent(parent);

        return Util.GetOrAddComponent<T>(go);
    }

    public void Clear()
    {
        
    }
}
