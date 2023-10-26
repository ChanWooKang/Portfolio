using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossField : MonoBehaviour
{
    BossCtrl bc = null;

    public void SettingBoss(BossCtrl bCtrl)
    {
        Debug.Log("보스 세팅");
        bc = bCtrl;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (bc != null)
            {
                
                bc.RecognizePlayer(other.transform);
                Debug.Log("플레이어 진입");
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
                Debug.Log("플레이어 퇴장");
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
