using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCtrl : MonoBehaviour
{
    TrailRenderer _render;
    BoxCollider _colider;

    public float Damage = 0;

    void Awake()
    {
        Init();
        SetEnable(false);
    }

    void Init()
    {
        _render = GetComponentInChildren<TrailRenderer>();
        _colider = GetComponent<BoxCollider>();
    }

    void SetEnable(bool check)
    {
        _render.enabled = check;
        _colider.enabled = check;
    }

    public void WeaponUse(float damage , float time)
    {
        Damage = damage;
        StopCoroutine(WeaponEvent(time));
        StartCoroutine(WeaponEvent(time));
    }

    IEnumerator WeaponEvent(float time)
    {
        SetEnable(true);
        yield return new WaitForSeconds(time);
        SetEnable(false);
    }
}
