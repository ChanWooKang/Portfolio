using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossField : MonoBehaviour
{
    BossCtrl bc = null;
    public bool isInPlayer = false;
    public void SettingBoss(BossCtrl bCtrl)
    {
        bc = bCtrl;
        if(bc != null)
        {
            if (isInPlayer)
                bc.gameObject.SetActive(false);
        }        
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
                if (PlayerCtrl._inst.Bools[Define.PlayerBools.Dead] == false)
                {
                    PlayerCtrl._inst.SetInBossField(bc.gameObject, true);
                    isInPlayer = true;
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
                if (PlayerCtrl._inst.Bools[Define.PlayerBools.Dead] == false) 
                {
                    PlayerCtrl._inst.SetInBossField();
                    isInPlayer = false;

                    if(bc != null)
                    {
                        if (bc.isDead == true)
                        {
                            bc.PlayerOut();
                        }                            
                    }
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
