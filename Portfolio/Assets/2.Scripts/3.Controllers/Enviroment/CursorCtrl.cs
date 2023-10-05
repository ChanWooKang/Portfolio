using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class CursorCtrl : MonoBehaviour
{
    Dictionary<eCursor, Texture2D> dict;
    eCursor nowCursor = eCursor.Unknwon;
    int lMask;

    void Start()
    {
        
        Init();    
    }

    void Update()
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) 
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit rhit, 100, lMask))
        {
            if(rhit.collider.gameObject.layer == (int)eLayer.Ground)
            {
                if (nowCursor != eCursor.Default)
                {
                    Texture2D tex = LoadCursor(eCursor.Default);
                    Cursor.SetCursor(tex, new Vector2(tex.width / 3, 0), CursorMode.Auto);
                    nowCursor = eCursor.Default;
                }
            }
            else
            {
                if (nowCursor != eCursor.Attack)
                {
                    Texture2D tex = LoadCursor(eCursor.Attack);
                    Cursor.SetCursor(tex, new Vector2(tex.width / 3, 0), CursorMode.Auto);
                    nowCursor = eCursor.Attack;
                }
            }
        }
        else
        {
            if(nowCursor != eCursor.Default)
            {
                Texture2D tex = LoadCursor(eCursor.Default);
                Cursor.SetCursor(tex, new Vector2(tex.width / 3, 0), CursorMode.Auto);
                nowCursor = eCursor.Default;
            }
        }
    }

    void Init()
    {
        dict = Managers._scene.dict_Cursor;
        nowCursor = eCursor.Unknwon;
        lMask = (1 << (int)eLayer.Ground) | (1 << (int)eLayer.Monster);
    }

    Texture2D LoadCursor(eCursor type)
    {
        
        if (dict.TryGetValue(type, out Texture2D cursor) == false)
        {
            Debug.Log($"CursorCtrl : Failed To Load Cursor TYPE({type})");
            return null;
        }
        else
            return cursor;
    }

}
