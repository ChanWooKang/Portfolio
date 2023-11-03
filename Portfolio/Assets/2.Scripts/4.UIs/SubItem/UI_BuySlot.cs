using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Define;
using UnityEngine.EventSystems;

public class UI_BuySlot : UI_Base, IPointerEnterHandler, IPointerExitHandler
{
    enum GameObjects
    {
        ItemIcon,
        ItemName,
        ItemPrice
    }

    public SOItem item;
    Image Item_Image;
    Text Item_Name;
    Text Item_Price;
    public bool SoldOut { get; set; }
    const string _priceFormat = "{0:#,###}";
    const string _soldOut = "구매 완료";
    const string _failedByMoney = "금화가 부족합니다.";
    const string _failedByInventory = "인벤토리가 꽉 찼습니다.";

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Item_Image = GetObject((int)GameObjects.ItemIcon).GetComponent<Image>();
        Item_Name = GetObject((int)GameObjects.ItemName).GetComponent<Text>();
        Item_Price = GetObject((int)GameObjects.ItemPrice).GetComponent<Text>();
        ClearItem();
    }

    public void AddItem(SOItem _item)
    {
        item = _item;
        Item_Image.sprite = _item.icon;
        Item_Name.text = _item.krName;
        Item_Price.text = string.Format(_priceFormat, _item.price);
        SoldOut = false;
    }

    public void ClearItem()
    {
        item = null;        
        SoldOut = true;
        Item_Price.text = _soldOut;
    }

    public void TryBuyItem()
    {
        if (item == null || SoldOut)
        {
            return;
        }

        if (InventoryManager.ActiveChangeEquip == false)
        {
            if (InventoryManager._inst.CheckSlotFull(item) == false)
            {
                if (PlayerCtrl._inst != null)
                {

                    if (PlayerCtrl._inst.TryUseMoney(item.price))
                    {
                        SoundManager._inst.Play(eSoundList.Shop_BuySell);
                        InventoryManager._inst.AddToInven(item);
                        ClearItem();
                    }
                    else
                    {
                        GameManagerEX._inst.ShopAlert(_failedByMoney);
                    }
                }

            }
            else
            {
                GameManagerEX._inst.ShopAlert(_failedByInventory);
            }

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            if (item.iType == eItem.Equipment)
            {
                UI_ItemInfo._inst.SetInforMation(item, Item_Image.transform.position, true);
            }
            else
            {
                UI_ItemInfo._inst.SetInforMation(item, Item_Image.transform.position, false, 1);
            }
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UI_ItemInfo._inst.OffInforMation();
    }
}