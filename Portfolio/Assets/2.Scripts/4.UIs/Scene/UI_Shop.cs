using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Define;
using System.Linq;

public class UI_Shop : UI_Base
{
    public static bool ActivatedShop = false;
    List<SOItem> list_Sells = new List<SOItem>();
    [SerializeField]
    GameObject ShopPage;
    UI_SellSlot[] Slots;

    void Start()
    {
        Init();
    }

    public override void Init()
    {

        Slots = GetComponentsInChildren<UI_SellSlot>();
        for (int i = 0; i < Slots.Length; i++)
            Slots[i].Init();
        CloseUI();
    }

    public void OpenUI()
    {
        ActivatedShop = true;
        UpdateItem();
        ShopPage.SetActive(true);
    }

    public void CloseUI()
    {
        ActivatedShop = false;
        ShopPage.SetActive(false);
    }

    public void UpdateItem()
    {
        list_Sells.Clear();
        for (int i = 0; i < Slots.Length; i++)
                Slots[i].AddItem(PickItem());
    }

    SOItem PickItem()
    {
        SOItem temp = null;
        int max = InventoryManager._inst.items.Length;
        int value = Random.Range(1, max);
        temp = InventoryManager._inst.items[value];
        return temp;
    }
}
