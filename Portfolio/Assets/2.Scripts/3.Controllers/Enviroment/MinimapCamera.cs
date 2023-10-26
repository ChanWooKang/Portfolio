using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class MinimapCamera : MonoBehaviour
{
    static MinimapCamera _uniqueInstance;
    public static MinimapCamera _inst { get { return _uniqueInstance; } }

    List<GameObject> _markers = new List<GameObject>();

    [SerializeField]
    public GameObject Mark_Player;
    [SerializeField]
    public GameObject Mark_Monster;

    [SerializeField]
    bool x, y, z;
    [SerializeField]
    Transform _target;
    Camera thisCam;

    [SerializeField]
    RenderTexture MiniMapTexture;
    [SerializeField]
    RenderTexture WorldMapTexture;

    public static bool IsWorldMap = false;

    float miniSize = 15f;
    [SerializeField]
    float worldSize;
    [SerializeField]
    Vector3 worldPos;

    void Awake()
    {
        _uniqueInstance = this;
        thisCam = GetComponent<Camera>();
        ChangeMiniMap();
    }

    void Update()
    {

        if (!IsWorldMap)
        {
            if (_target == null)
                return;

            transform.position = new Vector3(
            (x ? _target.position.x : transform.position.x),
            (y ? _target.position.y : transform.position.y),
            (z ? _target.position.z : transform.position.z));
        }
    }

    public void ChangeWorldMap()
    {
        if (IsWorldMap == false)
        {
            SettingObjectSize();
        }

        if (thisCam.orthographicSize != worldSize)
            thisCam.orthographicSize = worldSize;

        thisCam.targetTexture = WorldMapTexture;
        transform.position = worldPos;
        IsWorldMap = true;
    }

    public void ChangeMiniMap()
    {
        if(IsWorldMap == true)
        {
            SettingObjectSize();
        }

        if (thisCam.orthographicSize != miniSize)
            thisCam.orthographicSize = miniSize;

        thisCam.targetTexture = MiniMapTexture;
        IsWorldMap = false;
    }

    public void InstiatieMarker(bool isPlayer, Transform tr, eMonster type = eMonster.Unknown)
    {
        GameObject go = null;
        if (isPlayer)
        {
            go = Instantiate(Mark_Player, tr);

            if (IsWorldMap)
            {
                go.transform.localScale = Vector3.one * 4;
            }
            else
            {
                go.transform.localScale = Vector3.one * 2;
            }
            AddMarkSetting(go, true);
        }
        else
        {
            if(type != eMonster.Boss)
                go = Instantiate(Mark_Monster, tr);
            else
            {
                go = Instantiate(Mark_Monster, tr);
            }

            if (IsWorldMap)
            {
                go.transform.localScale = Vector3.one * 3;
            }
            else
            {
                go.transform.localScale = Vector3.one * 1;
            }
            AddMarkSetting(go, false);
        }

        
        
    }

    public void AddMarkSetting(GameObject go ,bool isPlayer = false)
    {
        if(go != null)
            _markers.Add(go);
    }

    void SettingObjectSize()
    {
        if (_markers != null)
        {
            if (IsWorldMap)
            {
                for (int i = 0; i < _markers.Count; i++)
                {
                    if(i > 0)
                        _markers[i].transform.localScale = Vector3.one * 2;
                    else
                        _markers[i].transform.localScale = Vector3.one * 3;
                }
                    
            }
            else
            {
                for (int i = 0; i < _markers.Count; i++)
                {
                    if (i > 0)
                        _markers[i].transform.localScale = Vector3.one * 4;
                    else
                        _markers[i].transform.localScale = Vector3.one * 5;
                }
            }
        }
    }
}

//µ¥Ä®