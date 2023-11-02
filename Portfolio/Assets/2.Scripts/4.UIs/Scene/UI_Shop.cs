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
    [SerializeField] int slotCnt = 5;

    public override void Init()
    {

        UpdateItem();
        CloseUI();
    }


    public void TryOpenUI()
    {
        if (ActivatedShop)
        {
            CloseUI();
        }
        else
        {
            OpenUI();
        }
    }

    void OpenUI()
    {
        ActivatedShop = true;
    }

    void CloseUI()
    {
        ActivatedShop = false;
    }

    public void UpdateItem()
    {
        list_Sells.Clear();
        for(int i = 0; i <slotCnt; i++)
        {

        }
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
