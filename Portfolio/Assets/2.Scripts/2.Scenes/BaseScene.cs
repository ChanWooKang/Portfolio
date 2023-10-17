using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class BaseScene : MonoBehaviour
{
    public eScene PrevScene { get; protected set; } = eScene.Unknown;
    public eScene CurrScene { get; protected set; } = eScene.Unknown;

    public List<CursorUnit> list_Cursor;

    void Awake() { Init(); }

    protected virtual void Init()
    {
        PoolingManager._pool.LoadObjectPool();
        Managers._scene.AddCursor();
    }

    public void SceneLoad(eScene scene)
    {
        StartCoroutine(Managers._scene.LoadCoroutine(scene));
    }

    public virtual void Clear() 
    {
        PoolingManager._pool.Clear();
        PrevScene = CurrScene; 
    }
}
