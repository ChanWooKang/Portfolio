using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class Item : MonoBehaviour
{
    public SOItem itemSO;
    Rigidbody _rb;
    SphereCollider _colider;

    bool isShoot;
    bool isCall;
    bool isStop;
    public int Gold { get; set; }

    private void OnCollisionStay(Collision collision)
    {
        if(isShoot)
        {
            if (collision.gameObject.CompareTag(Util.ConvertEnum(eTag.Ground)))
            {
                _rb.isKinematic = true;
                _colider.enabled = false;
                if(!isCall)
                {
                    //ÄÚ·çÆ¾
                    StartCoroutine(Turnnig());
                    isCall = true;
                }
            }
        }
    }

    void Init()
    {
        _rb = GetComponent<Rigidbody>();
        _colider = GetComponent<SphereCollider>();
        isShoot = false;
        isCall = false;
        isStop = false;
    }

    Vector3 GetRandomPoint()
    {
        Vector3 pos = Random.onUnitSphere;
        pos.y = 1;
        return pos;
    }

    public void Spawn(float power = 5)
    {
        Init();
        Vector3 dir = GetRandomPoint();
        _rb.AddForce(dir * power, ForceMode.Impulse);
        isShoot = true;
    }

    IEnumerator Turnnig()
    {
        while(isStop == false)
        {
            transform.Rotate(30.0f * Vector3.up * Time.deltaTime);
            yield return null;
        }
    }

    public bool Root()
    {
        if(itemSO.iType == eItem.Gold)
        {
            PlayerCtrl._inst.EarnMoney(Gold);
            Despawn();
            return true;
        }
        else
        {
            if(InventoryManager._inst.CheckSlotFull(itemSO) == false)
            {
                InventoryManager._inst.AddInvenItem(itemSO);
                Despawn();
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    void Despawn()
    {
        gameObject.DestroyAPS();
        _rb.isKinematic = false;
        _colider.enabled = true;
        isStop = false;
        isCall = false;
        isShoot = false;
    }


}
