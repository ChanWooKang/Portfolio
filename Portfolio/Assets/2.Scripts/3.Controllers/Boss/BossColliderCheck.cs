using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossColliderCheck : MonoBehaviour
{
    BossCtrl bc;

    void OnTriggerEnter(Collider other)
    {
        if (bc == null)
        {
            bc = GetComponentInParent<BossCtrl>();
        }

        if (other.CompareTag("Weapon") || other.CompareTag("Cry") || other.CompareTag("Slash"))
        {
            float damage = 0;
            if (other.CompareTag("Weapon"))
            {
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
}
