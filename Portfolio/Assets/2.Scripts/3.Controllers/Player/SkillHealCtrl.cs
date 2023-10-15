using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHealCtrl : MonoBehaviour
{
    [SerializeField] ParticleSystem _ps;

    public void HealEffect()
    {
        _ps.Play(true);
    }
}
