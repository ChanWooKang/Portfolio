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
                bc.RecognizePlayer(other.transform);
            }
            if (PlayerCtrl._inst != null)
            {
                if (!PlayerCtrl._inst.Bools[Define.PlayerBools.Dead])
                {
                    PlayerCtrl._inst.SetInBossField(bc.gameObject, true);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerCtrl._inst != null)
            {
                if (!PlayerCtrl._inst.Bools[Define.PlayerBools.Dead])
                {
                    PlayerCtrl._inst.SetInBossField();
                }
            }
        }

        if (other.CompareTag("Boss"))
        {
            if(bc != null)
            {
                bc.IsOutField();
            }
        }
    }
}
