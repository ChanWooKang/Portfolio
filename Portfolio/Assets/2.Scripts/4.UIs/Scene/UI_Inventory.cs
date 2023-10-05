using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Define;

public class UI_Inventory : UI_Base
{
    enum GameObjects
    {
        Inventory_Base,
        Inventory_Slot_Parent,
        Close
    }

    public static bool ActivatedInventory = false;

    GameObject Inventory_Base;
    GameObject Slots_Parent;
    UI_Slot[] slots;
    UI_Stats stats;

    void Update()
    {
        TryOpenInventory();
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Inventory_Base = GetObject((int)GameObjects.Inventory_Base);
        Slots_Parent = GetObject((int)GameObjects.Inventory_Slot_Parent);
        slots = Slots_Parent.GetComponentsInChildren<UI_Slot>();
        GameObject go = GetObject((int)GameObjects.Close);
        BindEvent(go, (PointerEventData data) => { if (data.button == PointerEventData.InputButton.Left) CloseInventory(); }, UIEvent.Click);
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Init();
        }

        stats = FindObjectOfType<UI_Stats>();
        stats.Init();
        CloseInventory();
    }

    public UI_Slot[] GetInvenSlots()
    {
        return slots;
    }

    public void LoadToInven(int arrNum, string iName, int iCount)
    {
        for (int i = 0; i < InventoryManager._inst.items.Length; i++)
        {
            if (InventoryManager._inst.items[i].Name == iName)
            {
                slots[arrNum].AddItem(InventoryManager._inst.items[i], iCount);
            }
        }
    }

    public void ResetAllSlots()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
        }
    }

    void TryOpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (ActivatedInventory == false)
                OpenInventory();
            else
                CloseInventory();
        }
    }

    void OpenInventory()
    {
        ActivatedInventory = true;
        Inventory_Base.SetActive(true);
        Managers._ui.OnSetUIEvent?.Invoke();
    }

    public void CloseInventory()
    {
        ActivatedInventory = false;
        Inventory_Base.SetActive(false);
    }

    public void AcquireItem(SOItem _item, int _count = 1)
    {
        if (_item.iType == eItem.Gold)
            return;

        if (eItem.Equipment != _item.iType)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null)
                {
                    if (slots[i].item.Name == _item.Name)
                    {
                        if (slots[i].itemCount + _count <= slots[i].item.maxStack)
                        {
                            slots[i].SetSlotCount(_count);
                            return;
                        }
                        else
                        {
                            continue;
                        }
                            
                    }
                }
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                return;
            }
        }

    }

    public bool CheckSlotFull(SOItem _item, int _count = 1)
    {
        if (eItem.Equipment != _item.iType)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null)
                {
                    if (slots[i].item.Name == _item.Name)
                    {
                        if(slots[i].itemCount + _count <= slots[i].item.maxStack)
                        {
                            return false;
                        }
                        else
                        {
                            continue;
                        }
                        
                    }
                }
            }
        }
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return false;
            }
        }

        return true;
    }
}
