using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Define;

public class UI_HotKey : UI_Base, IPointerClickHandler,IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    enum GameObjects
    {
        Item_Image,
        Count_Parent,
        Count_Text
    }

    public SOItem item;
    public int itemCount;
    Image Item_Image;
    Text Count_Text;
    GameObject Count_Parent;
    
    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Item_Image = GetObject((int)GameObjects.Item_Image).GetComponent<Image>();
        Count_Text = GetObject((int)GameObjects.Count_Text).GetComponent<Text>();
        Count_Parent = GetObject((int)GameObjects.Count_Parent);
        Managers._input.KeyAction -= OnKeyBoardEvent;
        Managers._input.KeyAction += OnKeyBoardEvent;
        
    }

    void OnKeyBoardEvent()
    {
        if (PlayerCtrl._inst.Bools[PlayerBools.Dead])
            return;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnUsePotion();
        }
    }

    void OnUsePotion()
    {
        if (item == null)
            return;

        InventoryManager._inst.OnUsePotion(item);
        SetSlotCount(-1);
    }

    void SetAlpah(float alpha)
    {
        Color color = Item_Image.color;
        color.a = alpha;
        Item_Image.color = color;
    }

    public bool CheckSlotRest(int cnt)
    {
        if (item != null)
        {
            int value = itemCount + cnt;
            if (item.maxStack >= value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }
    public int AccquireItem(int cnt)
    {
        if(item != null)
        {
            if (CheckSlotRest(cnt))
            {
                SetSlotCount(cnt);
                return 0;
            }
            else
            {
                int value = item.maxStack - itemCount;
                SetSlotCount(value);
                return cnt - value;
            }
        }
        else
        {
            return -1;
        }
    }

    public void LoadItem(string iName, int iCount)
    {
        for (int i = 0; i < InventoryManager._inst.items.Length; i++)
        {
            if (InventoryManager._inst.items[i].Name == iName)
            {
               AddItem(InventoryManager._inst.items[i], iCount);
                break;
            }
        }

    }

    public void SetSlotCount(int cnt)
    {
        itemCount += cnt;
        Count_Text.text = itemCount.ToString();

        if (itemCount <= 0)
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        item = null;
        itemCount = 0;
        Item_Image.sprite = null;
        SetAlpah(0);
        Count_Text.text = "0";
        Count_Parent.SetActive(false);
    }

    public void AddItem(SOItem _item, int cnt = 1)
    {
        item = _item;
        itemCount = cnt;
        Item_Image.sprite = _item.icon;
        Count_Text.text = itemCount.ToString();
        Count_Parent.SetActive(true);
        SetAlpah(1);
    }

    void ChangeSlot()
    {
        SOItem tempItem = item;
        int tempCount = itemCount;
        AddItem(DragSlot._inst.Slot_Inven.item, DragSlot._inst.Slot_Inven.itemCount);
        if (tempItem != null)
        {
            DragSlot._inst.Slot_Inven.AddItem(tempItem, tempCount);
        }
        else
        {
            DragSlot._inst.Slot_Inven.ClearSlot();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(item != null)
            {
                UI_ItemInfo._inst.OffInforMation();
                if (InventoryManager.ActiveChangeEquip == false)
                {
                    if (InventoryManager._inst.CheckSlotFull(item) == false)
                    {
                        InventoryManager._inst.AddHotKeyToInven(item, itemCount);
                        ClearSlot();
                    }
                    else
                    {
                        //Debug.Log("해제 불가 /인벤토리가 꽉찼습니다....");
                    }
                }
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot._inst.Slot_Inven != null && DragSlot._inst.Slot_Inven.item.iType == eItem.Potion)
        {
            if(item != null)
            {
                if(InventoryManager._inst.CheckSlotFull(item))
                {
                    return;
                }
            }
            ChangeSlot();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            UI_ItemInfo._inst.SetInforMation(item, transform.position, false, itemCount);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UI_ItemInfo._inst.OffInforMation();
    }
}
