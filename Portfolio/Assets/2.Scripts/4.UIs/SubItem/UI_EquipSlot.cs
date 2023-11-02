using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Define;

public class UI_EquipSlot : UI_Base, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public eEquipment slotType;
    public SOItem item = null;
    public Image Image_Item;

    enum GameObjects
    {
        Item_Image
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Image_Item = GetObject((int)GameObjects.Item_Image).GetComponent<Image>();
    }

    void SetAlpha(float alpha)
    {
        Color color = Image_Item.color;
        color.a = alpha;
        Image_Item.color = color;
    }

    public void SetItem(SOItem _item)
    {
        if (_item != null)
        {
            item = _item;
            Image_Item.sprite = item.icon;
            SetAlpha(1);
        }
        else
            ClearSlot();
    }

    public void ClearSlot()
    {
        item = null;
        Image_Item.sprite = null;
        SetAlpha(0);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {

        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item != null)
            {
                UI_ItemInfo._inst.OffInforMation();

                if (InventoryManager.ActiveChangeEquip == false)
                {
                    if (InventoryManager._inst.CheckSlotFull(item) == false)
                    {
                        InventoryManager._inst.OnChangeEvent?.Invoke(slotType, item, false);
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


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            if (InventoryManager.ActiveChangeEquip == false)
            {
                if (InventoryManager._inst.CheckSlotFull(item) == false)
                {
                    UI_ItemInfo._inst.OffInforMation();

                    DragSlot._inst.isFormInven = false;
                    DragSlot._inst.Slot_Equip = this;
                    DragSlot._inst.DragSetImage(Image_Item);
                    DragSlot._inst.SetCanvas(false);
                }
            }
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot._inst.rect.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot._inst.SetAlpha(0);
        DragSlot._inst.SetCanvas(true);
        DragSlot._inst.Slot_Inven = null;
        DragSlot._inst.Slot_Equip = null;
        DragSlot._inst.rect.position = Vector2.zero;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot._inst.Slot_Inven != null && DragSlot._inst.Slot_Inven.item.iType == eItem.Equipment)
        {
            if (DragSlot._inst.Slot_Inven.item.eType == slotType)
            {
                if (InventoryManager.ActiveChangeEquip == false)
                {
                    InventoryManager._inst.OnChangeEvent?.Invoke(DragSlot._inst.Slot_Inven.item.eType, DragSlot._inst.Slot_Inven.item, true);
                    DragSlot._inst.Slot_Inven.ClearSlot();
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            UI_ItemInfo._inst.SetInforMation(item, transform.position, true);
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UI_ItemInfo._inst.OffInforMation();
    }
}
