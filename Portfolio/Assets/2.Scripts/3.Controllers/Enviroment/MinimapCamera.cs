using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
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
    float worldSize = 130f;
    Vector3 worldPos = new Vector3(-60, 240, 10);

    void Awake()
    {
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
        if (thisCam.orthographicSize != worldSize)
            thisCam.orthographicSize = worldSize;

        thisCam.targetTexture = WorldMapTexture;
        transform.position = worldPos;
        IsWorldMap = true;
    }

    public void ChangeMiniMap()
    {
        if (thisCam.orthographicSize != miniSize)
            thisCam.orthographicSize = miniSize;

        thisCam.targetTexture = MiniMapTexture;
        IsWorldMap = false;
        
    }
}
