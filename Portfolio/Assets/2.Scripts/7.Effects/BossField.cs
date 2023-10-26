using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossField : MonoBehaviour
{
    BossCtrl bc = null;

    public void SettingBoss(BossCtrl bCtrl)
    {
        Debug.Log("���� ����");
        bc = bCtrl;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (bc != null)
            {
                
                bc.RecognizePlayer(other.transform);
                Debug.Log("�÷��̾� ����");
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
                Debug.Log("�÷��̾� ����");
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
