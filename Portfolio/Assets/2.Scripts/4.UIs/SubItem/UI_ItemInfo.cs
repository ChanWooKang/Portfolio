using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Define;

public class UI_ItemInfo : UI_Base
{
    enum GameObjects
    {
        MainFrame,
        ItemName,
        Icon,
        Effect,
        Desc,
        AmountParent,
        Amount,
        Price,
    }

    GameObject _parent;
    GameObject _amountParent;
    Image _icon;
    Text _name;
    Text _effect;
    Text _desc;
    Text _amount;
    Text _price;

    public override void Init()
    {
       Bind<GameObject>(typeof(GameObjects));
        _parent = GetObject((int)GameObjects.MainFrame);
        _amountParent = GetObject((int) GameObjects.AmountParent);
        _icon = GetObject((int)GameObjects.Icon).GetComponent<Image>();
        _name = GetObject((int)GameObjects.ItemName).GetComponent<Text>();
        _effect = GetObject((int)GameObjects.Effect).GetComponent<Text>();
        _desc = GetObject((int)GameObjects.Desc).GetComponent<Text>();
        _amount = GetObject((int)GameObjects.Amount).GetComponent<Text>();
        _price = GetObject((int)GameObjects.Price).GetComponent<Text>();
    }

    
}
