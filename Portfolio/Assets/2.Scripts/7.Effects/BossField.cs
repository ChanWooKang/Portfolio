using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossField : MonoBehaviour
{
    BossCtrl bc = null;

    public void SettingBoss(BossCtrl bCtrl)
    {
        bc = bCtrl;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (bc != null)
            {
                bc.SetTarget(other.transform);
                bc.RecognizePlayer();
            }
        }

        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (bc != null)
            {
                bc.SetTarget(null);
            }
        }

        if (other.CompareTag("Monster"))
        {
            if(bc != null)
            {
                bc.IsOutField();
            }
        }
    }
}
