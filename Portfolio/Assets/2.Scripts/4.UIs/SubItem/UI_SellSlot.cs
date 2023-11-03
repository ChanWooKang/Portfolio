using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Define;
using UnityEngine.EventSystems;

public class UI_SellSlot : UI_Base
{
    enum GameObjects
    {
        ItemIcon,
        ItemName,
        ItemCount,
        ItemPrice
    }

    public UI_Slot matchSlot;
    public SOItem item;
    Image Item_Image;
    Text Item_Name;
    Text Item_Count;
    Text Item_Price;

    int itemAmount;
    int price;

    public bool SoldOut { get; set; }
    const string _priceFormat = "{0:#,###}";
    const string _countFormat = "보유 개수 : {0}";
    const string _soldOut = "판매 완료";

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Item_Image = GetObject((int)GameObjects.ItemIcon).GetComponent<Image>();
        Item_Name = GetObject((int)GameObjects.ItemName).GetComponent<Text>();
        Item_Count = GetObject((int)GameObjects.ItemCount).GetComponent<Text>();
        Item_Price = GetObject((int)GameObjects.ItemPrice).GetComponent<Text>();
        ClearItem();
    }

    public void AddItem(UI_Slot slot, int cnt = 1)
    {
        matchSlot = slot;
        item = slot.item;
        Item_Image.sprite = item.icon;
        Item_Name.text = item.krName;
        itemAmount = cnt;
        price = Mathf.FloorToInt(item.price * 0.5f);
        Item_Price.text = string.Format(_priceFormat, price);
        SoldOut = false;
        SetCountText();
        if (item.iType == eItem.Equipment)        
            Item_Count.gameObject.SetActive(false);
        else 
            Item_Count.gameObject.SetActive(true);
    }

    void SetCountText()
    {
        if(item != null)
        {
            if (item.iType != eItem.Equipment)
                Item_Count.text = string.Format(_countFormat, itemAmount);
        }
    }

    public void ClearItem()
    {
        matchSlot = null;
        item = null;
        SoldOut = true;
        itemAmount = 0;
        price = 0;
        SetCountText();
        Item_Price.text = _soldOut;
    }

    public void SellItem()
    {
        if (item == null || SoldOut || matchSlot == null)
            return;

        if(PlayerCtrl._inst != null)
        {
            if (itemAmount < 1)
                return;

            PlayerCtrl._inst.EarnMoney(price);
            SoundManager._inst.Play(eSoundList.Shop_BuySell);
            matchSlot.SetSlotCount(-1);
            itemAmount--;
            SetCountText();
            if (itemAmount == 0)
            {
                ClearItem();
            }
        }
    }
}
