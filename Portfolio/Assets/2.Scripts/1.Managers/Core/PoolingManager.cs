using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

[AddComponentMenu("Custom/PoolingManager")]
public class PoolingManager : MonoBehaviour
{
    static PoolingManager _uniqueInstance;
    public static PoolingManager _pool { get { Init(); return _uniqueInstance; } }
    public PoolUnit[] _poolingUnits;
    public List<GameObject>[] _pooledUnitList;
    public int _defPoolAmount = 5;
    public bool _canPoolExpand = true;
    public Dictionary<string, PoolUnit> _poolingUnitDictionary;
    public Dictionary<string, Dictionary<int, GameObject>> _pooledUnits;


    static void Init()
    {
        if(_uniqueInstance == null)
        {
            GameObject go = GameObject.Find("@Pool");
            if(go == null)
            {
                go = new GameObject { name = "@Pool" };
                go.AddComponent<PoolingManager>();
            }
            else
            {
                _uniqueInstance = go.GetComponent<PoolingManager>();
            }
        }
    }    

    public void Clear()
    {
        _uniqueInstance = null;
    }

    public void LoadObjectPool()
    {
        _pooledUnits = new Dictionary<string, Dictionary<int, GameObject>>();
        _poolingUnitDictionary = new Dictionary<string, PoolUnit>();
        for (int i = 0; i < _poolingUnits.Length; i++)
        {
            Dictionary<int, GameObject> objDict = new Dictionary<int, GameObject>();
            if (_poolingUnits[i].amount > 0)
                _poolingUnits[i].CurAmount = _poolingUnits[i].amount;
            else
                _poolingUnits[i].CurAmount = _defPoolAmount;

            if (_poolingUnitDictionary.ContainsKey(_poolingUnits[i].name) == false)
            {
                _poolingUnitDictionary.Add(_poolingUnits[i].name, _poolingUnits[i]);
            }
            else
            {
                Debug.Log("복수로 풀링매니저에 저장되어있습니다.");
            }

            int index = 0;
            for (int j = 0; j < _poolingUnits[i].CurAmount; j++)
            {
                GameObject newItem = (GameObject)Instantiate(_poolingUnits[i].prefab);
                objDict.Add(j, newItem);
                AddToPooledUnits(newItem);
                ++index;
            }
            _pooledUnits.Add(_poolingUnits[i].name, objDict);
        }
    }

    void AddToPooledUnits(GameObject newItem, Transform parent = null)
    {
        newItem.SetActive(false);
        if (parent == null)
            newItem.transform.SetParent(transform);
        else
            newItem.transform.SetParent(parent);

    }

    GameObject GetPooledItem(string value)
    {
        if (_pooledUnits == null)
            return null;

        if (_pooledUnits.ContainsKey(value))
        {
            foreach (var Data in _pooledUnits[value])
            {
                if (Data.Value.activeInHierarchy == false)
                    return Data.Value;
            }

            if (_canPoolExpand)
            {
                if (_poolingUnitDictionary.ContainsKey(value))
                {
                    GameObject prefab = _poolingUnitDictionary[value].prefab;
                    GameObject tmpObj = (GameObject)Instantiate(prefab);

                    AddToPooledUnits(tmpObj);

                    //Dictionary 추가 해야함
                    int index = _pooledUnits[value].Count;
                    _pooledUnits[value].Add(index, tmpObj);

                    return tmpObj;
                }
            }
        }
        return null;
    }

    public GameObject InstantiateAPS(int idx, Transform parent = null)
    {
        string pooledUnitName = _poolingUnits[idx].name;
        Transform prefabTransform = _poolingUnits[idx].prefab.transform;
        GameObject go = InstantiateAPS(pooledUnitName, Vector3.zero, prefabTransform.rotation, Vector3.one, parent);
        return go;
    }

    public GameObject InstantiateAPS(
        int idx, Vector3 pos, Quaternion rot, Vector3 scale,Transform parent = null
        )
    {
        string pooledUnitName = _poolingUnits[idx].name;
        GameObject go = InstantiateAPS(pooledUnitName, pos, rot, scale, parent);
        return go;
    }

    public GameObject InstantiateAPS(string pooledUnitName, Transform parent = null)
    {
        GameObject go = GetPooledItem(pooledUnitName);
        if (parent != null)
            go.transform.SetParent(parent);
        go.SetActive(true);
        return go;
    }

    public GameObject InstantiateAPS(
        string pooledUnitName, Vector3 pos, Quaternion rot, Vector3 scale, Transform parent = null
        )
    {
        GameObject go = GetPooledItem(pooledUnitName);
        if(go != null)
        {
            if (parent != null)
                go.transform.SetParent(parent);
            go.transform.position = pos;
            go.transform.rotation = rot;
            go.transform.localScale = scale;
            go.SetActive(true);
        }
        return go;
    }

    public List<GameObject> GetActivePooledItem()
    {
        List<GameObject> items = new List<GameObject>();
        for(int unitIdx = 0; unitIdx < _poolingUnits.Length; unitIdx++)
        {
            for(int listIdx = 0; listIdx < _pooledUnitList[unitIdx].Count; listIdx++)
            {
                if (_pooledUnitList[unitIdx][listIdx].activeInHierarchy)
                    items.Add(_pooledUnitList[unitIdx][listIdx]);
            }
        }
        return items;
    }

    public static void DestroyAPS(GameObject go)
    {
        go.SetActive(false);
        if (go.transform.parent != _uniqueInstance.transform)
            go.transform.SetParent(_uniqueInstance.transform);
    }
}
