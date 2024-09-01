using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class PoolManager : TSingleton<PoolManager>
{
    public PoolUnit[] _poolingUnits;
    //Dictionary<오브젝트이름, Dictionart<index, GameObject>> 
    public Dictionary<string, PoolUnit> _poolingUnitDictionary;
    public Dictionary<string, Dictionary<int,GameObject>> _pooledUnits;
    public int _defPoolAmount = 5;
    public bool _canPoolExpand = true;

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

            if(_poolingUnitDictionary.ContainsKey(_poolingUnits[i].name) == false)
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
            foreach(var Data in _pooledUnits[value])
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
}
