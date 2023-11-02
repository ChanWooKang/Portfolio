using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class Shop : MonoBehaviour
{
    UI_Shop _shopUI;
    

    public void TryOpenShop()
    {
        if (UI_Shop.ActivatedShop)
            return;

        if(_shopUI == null)
            _shopUI = FindObjectOfType<UI_Shop>();

        _shopUI.TryOpenUI();
    }
}
