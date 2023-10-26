using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossColliderCheck : MonoBehaviour
{
    BossCtrl bc;
    float cntTime;
    float damage = 0;

    public float Damage;

    public void SetDamage(float dmg)
    {
        Damage = dmg;
    }

    void OnTriggerEnter(Collider other)
    {
        if (bc == null)
        {
            bc = GetComponentInParent<BossCtrl>();
        }

        if (other.CompareTag("Weapon") || other.CompareTag("Cry") || other.CompareTag("Slash"))
        {
            if (other.CompareTag("Weapon"))
            {
                cntTime = 0;
                damage = other.transform.GetComponent<WeaponCtrl>().Damage;
            }
            else if (other.CompareTag("Cry"))
            {
                
                damage = other.transform.GetComponent<SkillCryCtrl>().Damage;
            }
            else
            {
                damage = other.transform.GetComponent<SkillSlashCtrl>().Damage;
            }

            if (damage > 0)
                bc.OnDamage(damage);
            else
                return;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            cntTime += Time.deltaTime;
            if(cntTime > 0.5f)
            {
                if (damage > 0)
                    bc.OnDamage(damage);
                cntTime = 0;
            }
        }
    }
}
