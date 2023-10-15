using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCryCtrl : MonoBehaviour
{
    [SerializeField] ParticleSystem _ps;
    SphereCollider _colider;
    public float Damage;

    void Awake()
    {
        _colider = GetComponent<SphereCollider>();
        Damage = 0;
        SetEnable(false);
        _ps.Stop(true);
    }

    void SetEnable(bool check)
    {
        _colider.enabled = check;
    }

    public void CryActive(float damage, float time)
    {
        Damage = damage;
        StopCoroutine(CryEvent(time));
        StartCoroutine(CryEvent(time));
    }

    IEnumerator CryEvent(float time)
    {
        _ps.Play(true);        
        yield return new WaitForSeconds(0.1f);
        SetEnable(true);
        yield return new WaitForSeconds(0.1f);
        SetEnable(false);
    }
}
