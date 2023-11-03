using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Define;

public class UI_SellSlot : UI_Base
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
        Item_Image.sprite = null;
        Item_Name.text = string.Empty;
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
                        
                        InventoryManager._inst.AddToInven(item);
                        ClearItem();
                    }
                    else
                    {
                        
                    }
                }

            }
            else
            {
                
            }

        }
    }
}