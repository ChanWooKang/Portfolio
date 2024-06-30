using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Define;

public class UI_RewardSlot : UI_Base, IPointerEnterHandler, IPointerExitHandler
{

    enum GameObjects
    {
        Item_Image,
        Count_Text,
        Count_Parent,

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
    }

    void SetAlpah(float alpha)
    {
        Color color = Item_Image.color;
        color.a = alpha;
        Item_Image.color = color;
    }

    public void AddItem(SOItem _item, int cnt = 1)
    {
        item = _item;
        itemCount = cnt;
        Item_Image.sprite = _item.icon;
        if (item.iType != eItem.Equipment)
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            if (item.iType == eItem.Equipment)
            {
                UI_ItemInfo._inst.SetInforMation(item, transform.position, true);
            }
            else
            {
                UI_ItemInfo._inst.SetInforMation(item, transform.position, false, itemCount);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UI_ItemInfo._inst.OffInforMation();
    }
}
