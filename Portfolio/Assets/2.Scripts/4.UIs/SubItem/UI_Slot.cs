using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Define;

public class UI_Slot : UI_Base, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    enum GameObjects
    {
        Item_Image,
        Count_Text,
        Count_Parent
    }

    public SOItem item;
    public int itemCount;
    Image Item_Image;
    Text Count_Text;
    GameObject Count_Parent;
    //[SerializeField] Image Item_Image;
    //[SerializeField] Text Count_Text;
    //[SerializeField] GameObject Count_Parent;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Item_Image = GetObject((int)GameObjects.Item_Image).GetComponent<Image>();
        Count_Text = GetObject((int)GameObjects.Count_Text).GetComponent<Text>();
        Count_Parent = GetObject((int)GameObjects.Count_Parent);
        ClearSlot();
    }

    void SetAlpah(float alpha)
    {
        Color color = Item_Image.color;
        color.a = alpha;
        Item_Image.color = color;
    }

    public bool CheckSlotRest(SOItem _item,int cnt)
    {
        if(item != null)
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
        else
        {
            if(_item.maxStack >= cnt)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public void SetSlotCount(int cnt)
    {
        itemCount += cnt;
        Count_Text.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    public void AddItem(SOItem _item, int cnt = 1)
    {
        item = _item;
        itemCount = cnt;
        Item_Image.sprite = _item.icon;
        if(item.iType != eItem.Equipment)
        {
            Count_Text.text = itemCount.ToString();
            Count_Parent.SetActive(true);
        }
        else
        {
            Count_Text.text = "0";
            Count_Parent.SetActive(false);
        }
        SetAlpah(1);
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

    void ChangeSlot()
    {
        SOItem tempItem = item;
        int tempCount = itemCount;
        AddItem(DragSlot._inst.Slot_Inven.item, DragSlot._inst.Slot_Inven.itemCount);
        if(tempItem != null)
        {
            DragSlot._inst.Slot_Inven.AddItem(tempItem, tempCount);
        }
        else
        {
            DragSlot._inst.Slot_Inven.ClearSlot();
        }
    }

    //우클릭 아이템 사용 혹은 장착/해제
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(item != null)
            {                
                UI_ItemInfo._inst.OffInforMation();

                switch (item.iType)
                {
                    case eItem.Equipment:
                        {
                            if(InventoryManager.ActiveChangeEquip == false)
                            {
                                InventoryManager._inst.OnChangeEvent?.Invoke(item.eType, item, true);
                                ClearSlot();
                            }
                        }
                        break;
                    case eItem.Potion:
                        {
                            InventoryManager._inst.OnUsePotion(item);
                            SetSlotCount(-1);
                        }
                        break;
                }
            }
        }
    }
    //드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            //Test 
            UI_ItemInfo._inst.OffInforMation();

            DragSlot._inst.isFormInven = true;
            DragSlot._inst.SetCanvas(false);
            DragSlot._inst.Slot_Inven = this;
            DragSlot._inst.DragSetImage(Item_Image);

        }
    }
    //드래그 중 이동
    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot._inst.rect.position = eventData.position;
        }
    }
    // 드래그 완료
    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot._inst.SetAlpha(0);
        DragSlot._inst.SetCanvas(true);
        DragSlot._inst.Slot_Inven = null;
        DragSlot._inst.rect.position = Vector2.zero;
    }
    // 슬롯에 드래그 아이템 들어왔을 경우
    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot._inst.isFormInven)
        {
            if (DragSlot._inst.Slot_Inven != null)
            {
                ChangeSlot();
            }
        }
        else
        {
            if (InventoryManager.ActiveChangeEquip == false)
            {
                if (InventoryManager._inst.CheckSlotFull(DragSlot._inst.Slot_Equip.item) == false)
                {
                    InventoryManager._inst.OnChangeEvent?.Invoke(DragSlot._inst.Slot_Equip.item.eType, DragSlot._inst.Slot_Equip.item, false);
                    DragSlot._inst.Slot_Equip.ClearSlot();
                }
            }
        }
    }
    // 마우스 갖다 대면 아이템 정보 표시
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item != null)
        {
            if(item.iType == eItem.Equipment)
            {
                UI_ItemInfo._inst.SetInforMation(item, transform.position, true);
            }
            else
            {
                UI_ItemInfo._inst.SetInforMation(item, transform.position, false, itemCount);
            }
        }
        
    }
    // 아이템 정보 표시 X
    public void OnPointerExit(PointerEventData eventData)
    {
        UI_ItemInfo._inst.OffInforMation();
    }
}
