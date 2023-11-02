using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class UI_Equipment : UI_Base
{
    enum GameObjects
    {
        Slots_Parent
    }

    Dictionary<eEquipment, UI_EquipSlot> Equip_Slots = new Dictionary<eEquipment, UI_EquipSlot>();
    GameObject Parent_Slots;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Parent_Slots = GetObject((int)GameObjects.Slots_Parent);
    }

    public void SettingSlot()
    {
        UI_EquipSlot[] slots = Parent_Slots.GetComponentsInChildren<UI_EquipSlot>();
        foreach (UI_EquipSlot slot in slots)
        {
            if (Equip_Slots.ContainsKey(slot.slotType))
            {
                //Debug.Log($"존재하는 장비 슬롯 선택을 다시하세요 {slot.gameObject.name}");
                return;
            }
            Equip_Slots.Add(slot.slotType, slot);
        }
    }

    public Dictionary<eEquipment, SOItem> GetEquipSlot()
    {
        Dictionary<eEquipment, SOItem> dict = new Dictionary<eEquipment, SOItem>();

        foreach(var slot in Equip_Slots)
        {
            if (slot.Value.item != null)
                dict.Add(slot.Key, slot.Value.item);
            else
                dict.Add(slot.Key, null);
        }

        return dict;
    }

    public void AcquireItem(eEquipment type, SOItem _item)
    {
        Equip_Slots[type].SetItem(_item);
    }

    public void LoadToEquip(eEquipment type, string iName)
    {
        for(int i = 0; i < InventoryManager._inst.items.Length; i++)
        {
            if(InventoryManager._inst.items[i].Name == iName)
            {
                SOItem item = InventoryManager._inst.items[i];
                Equip_Slots[type].SetItem(item);
            }
        }
    }

    
    public void ClearEquip()
    {
        for(int i = 0; i < (int)eEquipment.Max_Cnt; i++)
        {
            AcquireItem((eEquipment)i, null);
        }
            
    }
}
