using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class PotionEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem hp;
    [SerializeField] ParticleSystem mp;
    [SerializeField] ParticleSystem both;

    public void EffectPlay(ePotion type)
    {
        switch (type)
        {
            case ePotion.HP:
                if(both.isStopped == false)
                    both.Stop(true);
                if(mp.isStopped == false)
                    mp.Stop(true);
                hp.Play(true);
                break;
            case ePotion.MP:
                if (both.isStopped == false)
                    both.Stop(true);
                if (hp.isStopped == false)
                    hp.Stop(true);
                mp.Play(true);
                break;
            case ePotion.Double:
                if (mp.isStopped == false)
                    mp.Stop(true);
                if (hp.isStopped == false)
                    hp.Stop(true);
                both.Play(true);
                break;
        }
    }
}
