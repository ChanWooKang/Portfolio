using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class Shop : MonoBehaviour
{
    [SerializeField] UI_Shop _shopUI;

    public void OpenShop()
    {
        if (UI_Shop.ActivatedShop)
            return;

        _shopUI.OpenUI();
    }

    public void CloseShop()
    {
        if (UI_Shop.ActivatedShop == false)
            return;

        _shopUI.CloseUI();
    }
}
