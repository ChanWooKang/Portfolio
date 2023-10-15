using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class SpawnManager : MonoBehaviour
{
    static SpawnManager _uniqueInstance;
    public static SpawnManager _inst { get { return _uniqueInstance; } }
    public Action<eMonster, int> OnSpawnEvent;
    public List<SpawnPoint> points;

    [SerializeField] SOItem testItem;
    PoolingManager pool;

    void Awake()
    {
        _uniqueInstance = this;
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        pool = PoolingManager._pool;
    }

    public void Clear()
    {
       
    }

    eMonster GetMonsterType(GameObject go)
    {
        eMonster type = eMonster.Unknown;
        if(go.TryGetComponent<MonsterCtrl>(out MonsterCtrl mc))
        {
            type = mc.mType;
        }
        return type;
    }
    
    public GameObject Spawn(eMonster type)
    {
        Transform tr = null;
        for(int i = 0; i < points.Count; i++)
        {
            if(type == points[i].targetType)
            {
                tr = points[i].target;
                break;
            }
        }

        if(tr == null)
        {
            Debug.Log($"해당 스폰 위치 존재 X : {type}");
            return null;
        }

        GameObject go = pool.InstantiateAPS(Util.ConvertEnum(type), tr.position, tr.rotation, Vector3.one);
        if(go.TryGetComponent<MonsterCtrl>(out MonsterCtrl mc) == false)
        {
            Destroy(go);
            return null;
        }

        mc._defPos = tr.position;
        OnSpawnEvent?.Invoke(type, 1);
        return go;
    }

    public void Spawn(SOItem item, Transform parent)
    {
        GameObject go = SpawnObject(item, parent);
        if(go != null)
        {
            if(go.TryGetComponent<Item>(out Item _item) == false)
            {
                _item = go.AddComponent<Item>();
                _item.itemSO = item;
            }
            _item.Spawn();
        }
        else
        {
            Debug.Log($"SpawnManager : Failed to Load Prefab ({item.Name})");
        }
    }

    public void Spawn(SOItem item , Transform parent, int gold)
    {
        GameObject go = SpawnObject(item, parent);
        if (go != null)
        {
            if (go.TryGetComponent<Item>(out Item _item) == false)
            {
                _item = go.AddComponent<Item>();
                _item.itemSO = item;
            }
            _item.Gold = gold;
            _item.Spawn();
        }
        else
        {
            Debug.Log($"SpawnManager : Failed to Load Prefab ({item.Name})");
        }
    }

    GameObject SpawnObject(SOItem item, Transform parent)
    {
        Vector3 pos = parent.position;
        pos.y = 0.5f;
        Quaternion rot = Quaternion.Euler(0, 0, 0);
        GameObject go = pool.InstantiateAPS(item.Name, pos, rot, Vector3.one);
        return go;
    }

    public void MonsterDespawn(GameObject go)
    {
        eMonster type = GetMonsterType(go);
        if (type == eMonster.Unknown || type == eMonster.Max_Cnt)
            return;
        OnSpawnEvent?.Invoke(type, -1);
        go.DestroyAPS();
    }
}
