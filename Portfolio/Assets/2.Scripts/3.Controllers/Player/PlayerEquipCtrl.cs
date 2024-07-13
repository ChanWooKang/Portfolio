using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class PlayerEquipCtrl : MonoBehaviour
{
    public List<GameObject> Weapons;
    public List<GameObject> Shields;
    public WeaponCtrl nowWeapon;

    Dictionary<int, GameObject> weaponCollect;
    Dictionary<int, GameObject> shieldCollect;

    //인벤토리 로드 이후 실행
    public void SettingEquipment()
    {
        Dictionary<eEquipment, SOItem> nowEquip = InventoryManager._inst.dict_Equip;

        foreach(var Equipment in nowEquip)
        {
            if(Equipment.Value == null)
            {
                //장비를 장착 하지 않은 경우
                switch (Equipment.Key)
                {
                    case eEquipment.Weapon:
                        SetWeapon(0);
                        break;
                    case eEquipment.Shield:
                        SetShield(0);
                        break;
                }
            }
            else
            {
                //장비를 장착 한 경우
                switch (Equipment.Key)
                {
                    case eEquipment.Weapon:
                        for(int i = 0; i < Weapons.Count; i++)
                        {
                            if(Equipment.Value.Name == Weapons[i].name)
                            {
                                SetWeapon(i);
                                break;
                            }
                        }
                        if (nowWeapon == null)
                            SetWeapon(0);
                        break;
                    case eEquipment.Shield:
                        for (int i = 0; i < Shields.Count; i++)
                        {
                            if (Equipment.Value.Name == Shields[i].name)
                            {
                                SetShield(i);
                                break;
                            }
                        }                       
                        break;
                }
            }
        }        
    }

    public void ChangeEquipment(eEquipment type, string EquipName)
    {
        switch (type)
        {
            case eEquipment.Weapon:
                for (int i = 0; i < Weapons.Count; i++)
                {
                    if (EquipName == Weapons[i].name)
                    {
                        SetWeapon(i);
                        break;
                    }
                }
                if (nowWeapon == null)
                    SetWeapon(0);
                break;
            case eEquipment.Shield:
                for (int i = 0; i < Shields.Count; i++)
                {
                    if (EquipName == Shields[i].name)
                    {
                        SetShield(i);
                        break;
                    }
                }
                break;
        }
    }

    public void SetWeapon(int index)
    {
        nowWeapon = null;

        for(int i = 0; i < Weapons.Count; i++)
        {            
            if(i == index)
            {
                Weapons[i].SetActive(true);
            }
            else
            {
                Weapons[i].SetActive(false);
            }
        }
        

        if(!Weapons[index].TryGetComponent(out WeaponCtrl wc))
        {
            wc = Weapons[index].AddComponent<WeaponCtrl>();
        }

        nowWeapon = wc;
    }

    public void SetShield(int index)
    {
        for (int i = 0; i < Shields.Count; i++)
        {
            if (i == index)
            {
                Shields[i].SetActive(true);
            }
            else
            {
                Shields[i].SetActive(false);
            }
        }
    }
}
