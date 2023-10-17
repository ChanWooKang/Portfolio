using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Define;

public class UI_ItemInfo : UI_Base
{
    static UI_ItemInfo _uniqueInstance;
    public static UI_ItemInfo _inst { get { return _uniqueInstance; } }
    enum GameObjects
    {
        MainFrame,
        ItemName,
        Icon,
        EffectParent,
        Effect,
        Desc,
        AmountParent,
        Amount,
        Price,
    }

    GameObject _parent;
    GameObject _effectParent;
    GameObject _amountParent;
    Image _icon;
    Text _name;
    Text _effect;
    Text _desc;
    Text _amount;
    Text _price;
    string _format;

    void Awake()
    {
        _uniqueInstance = this;
    }

    void Start()
    {
        Init();
        OffInforMation();
    }

    public override void Init()
    {
        _format = "{0:#,###}";
        Bind<GameObject>(typeof(GameObjects));
        _parent = GetObject((int)GameObjects.MainFrame);
        _effectParent = GetObject((int)GameObjects.EffectParent);
        _amountParent = GetObject((int) GameObjects.AmountParent);
        _icon = GetObject((int)GameObjects.Icon).GetComponent<Image>();
        _name = GetObject((int)GameObjects.ItemName).GetComponent<Text>();
        _effect = GetObject((int)GameObjects.Effect).GetComponent<Text>();
        _desc = GetObject((int)GameObjects.Desc).GetComponent<Text>();
        _amount = GetObject((int)GameObjects.Amount).GetComponent<Text>();
        _price = GetObject((int)GameObjects.Price).GetComponent<Text>();
    }

    string SetTextEffect(List<STAT> sList)
    {
        string data = "";
        for(int i = 0; i < sList.Count; i++)
        {
            float value = sList[i].sValue;

            if ((value - Mathf.FloorToInt(value) != 0)) 
            {
                value *= 100;
                if (value > 0)
                {
                    
                    data += $"{sList[i].descName} + {value}%";
                }
                else
                {
                    data += $"{sList[i].descName} - {value}%";
                }
            }
            else
            {
                if (value > 0)
                {
                    data += $"{sList[i].descName} + {value}";
                }
                else
                {
                    data += $"{sList[i].descName} - {value}";
                }
            }
            

            if (sList.Count > 1 && i < sList.Count - 1) 
                data += "\n";
        }
        return data;
    }
    
    public void SetInforMation(SOItem item, Vector3 pos,bool isEquipment = false, int cnt = 1)
    {
        _icon.sprite = item.icon;
        _name.text = item.krName;
         
        string effectText = SetTextEffect(item.sList);
        if (string.IsNullOrEmpty(effectText))
        {
            // 효과가 없는 경우 
            _effectParent.SetActive(false);
            _effect.text = "";
        }
        else
        {
            //효과가 있는 경우
            _effectParent.SetActive(true);
            _effect.text = effectText;
        }

        _desc.text = item.description;
        _price.text = string.Format($"{_format}", item.price);

        if (isEquipment == false)
        {
            //장비 아이템이 아닐 경우
            _amountParent.SetActive(true);
            _amount.text = cnt.ToString();
        }
        else
        {
            //장비 아이템일 경우
            _amountParent.SetActive(false);
            _amount.text = "";

        }

        transform.position = pos;
        _parent.SetActive(true);
    }

    public void OffInforMation()
    {
        _parent.SetActive(false);
        transform.position = Vector3.zero;   
    }
}
