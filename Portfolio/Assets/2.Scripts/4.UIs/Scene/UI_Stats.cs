using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Stats : UI_Base
{
    enum Texts
    {
        Level,
        NowEXP,
        MaxEXP,
        NowHP,
        MaxHP,
        PlusHP,
        NowMP,
        MaxMP,
        PlusMP,
        Damage,
        PlusDamage,
        Defense,
        PlusDefense,
        Gold
    }

    Text Level;
    Text NowExp;
    Text MaxExp;
    Text NowHp;
    Text MaxHp;
    Text PlusHp;
    Text NowMp;
    Text MaxMp;
    Text PlusMp;
    Text Damage;
    Text PlusDamage;
    Text Defense;
    Text PlusDefense;
    Text Gold;

    //[SerializeField] Text Level;
    //[SerializeField] Text NowExp;
    //[SerializeField] Text MaxExp;
    //[SerializeField] Text NowHp;
    //[SerializeField] Text MaxHp;
    //[SerializeField] Text PlusHp;
    //[SerializeField] Text NowMp;
    //[SerializeField] Text MaxMp;
    //[SerializeField] Text PlusMp;
    //[SerializeField] Text Damage;
    //[SerializeField] Text PlusDamage;
    //[SerializeField] Text Defense;
    //[SerializeField] Text PlusDefense;
    //[SerializeField] Text Gold;

    PlayerStat ps;
    string _format;
    public bool _isRunning = true;

    public override void Init()
    {
        _format = "{0:N0}";
        Bind<Text>(typeof(Texts));
        Level = GetText((int)Texts.Level);
        NowExp = GetText((int)Texts.NowEXP);
        MaxExp = GetText((int)Texts.MaxEXP);
        NowHp = GetText((int)Texts.NowHP);
        MaxHp = GetText((int)Texts.MaxHP);
        PlusHp = GetText((int)Texts.PlusHP);
        NowMp = GetText((int)Texts.NowMP);
        MaxMp = GetText((int)Texts.MaxMP);
        PlusMp = GetText((int)Texts.PlusMP);
        Damage = GetText((int)Texts.Damage);
        PlusDamage = GetText((int)Texts.PlusDamage);
        Defense = GetText((int)Texts.Defense);
        PlusDefense = GetText((int)Texts.PlusDefense);
        Gold = GetText((int)Texts.Gold);

    }

    void Update()
    {
        if(UI_Inventory.ActivatedInventory && gameObject.activeSelf)
        {
            SetUI();
        }
    }

    void UIEvent()
    {
        if(_isRunning == false)
        {
            StopCoroutine(OnSetUI());
            StartCoroutine(OnSetUI());
        }
        
    }

    void SetUI()
    {
        ps = PlayerCtrl._inst._stat;
        float exp = ps.ConvertEXP();
        float totalexp = ps.ConvertTotalEXP();
        Level.text = string.Format(_format, ps.Level);
        if (exp > 0)
            NowExp.text = string.Format(_format, exp);
        else if (exp == 0)
            NowExp.text = "0";
        else
            NowExp.text = "Error";
        if (totalexp > 0)
            MaxExp.text = string.Format(_format, totalexp);
        else if (exp == 0)
            MaxExp.text = "0";
        else
            MaxExp.text = "Error";
        if (ps.HP > 0)
            NowHp.text = string.Format(_format, Mathf.Min(ps.HP, ps.MaxHP));
        else
            NowHp.text = "0";
        MaxHp.text = string.Format(_format, ps.MaxHP);
        if (ps.MP > 0)
            NowMp.text = string.Format(_format, Mathf.Min(ps.MP, ps.MaxMP));
        else
            NowMp.text = "0";
        MaxMp.text = string.Format(_format, ps.MaxMP);
        Damage.text = string.Format(_format, ps.Damage);
        Defense.text = string.Format(_format, ps.Defense);

        int value = 0;
        if (ps.PlusHP > 0)
        {
            value = Mathf.RoundToInt(ps.PlusHP);
            PlusHp.text = string.Format($"+{_format}", value);
            PlusHp.gameObject.SetActive(true);
        }
        else
        {
            PlusHp.text = "0";
            PlusHp.gameObject.SetActive(false);
        }
        if (ps.PlusMP > 0)
        {
            value = Mathf.RoundToInt(ps.PlusMP);
            PlusMp.text = string.Format($"+{_format}", value);
            PlusMp.gameObject.SetActive(true);
        }
        else
        {
            PlusMp.text = "0";
            PlusMp.gameObject.SetActive(false);
        }
        if (ps.PlusDamage > 0)
        {
            value = Mathf.RoundToInt(ps.PlusDamage);
            PlusDamage.text = string.Format($"+{_format}", value);
            PlusDamage.gameObject.SetActive(true);
        }
        else
        {
            PlusDamage.text = "0";
            PlusDamage.gameObject.SetActive(false);
        }
        if (ps.PlusDefense > 0)
        {
            value = Mathf.RoundToInt(ps.PlusDefense);
            PlusDefense.text = string.Format($"+{_format}", value);
            PlusDefense.gameObject.SetActive(true);
        }
        else
        {
            PlusDefense.text = "0";
            PlusDefense.gameObject.SetActive(false);
        }

        if (ps.Gold > 0)
            Gold.text = string.Format(_format, ps.Gold);
        else
            Gold.text = "0";


    }

    IEnumerator OnSetUI()
    {
        ps = PlayerCtrl._inst._stat;
        _isRunning = true;
        while (UI_Inventory.ActivatedInventory)
        {
            float exp = ps.ConvertEXP();
            float totalexp = ps.ConvertTotalEXP();
            Level.text = string.Format(_format, ps.Level);
            if (exp > 0)
                NowExp.text = string.Format(_format, exp);
            else if (exp == 0)
                NowExp.text = "0";
            else
                NowExp.text = "Error";
            if (totalexp > 0)
                MaxExp.text = string.Format(_format, totalexp);
            else if (exp == 0)
                MaxExp.text = "0";
            else
                MaxExp.text = "Error";
            if (ps.HP > 0)
                NowHp.text = string.Format(_format, Mathf.Min(ps.HP, ps.MaxHP));
            else
                NowHp.text = "0";
            MaxHp.text = string.Format(_format, ps.MaxHP);
            if (ps.MP > 0)
                NowMp.text = string.Format(_format, Mathf.Min(ps.MP, ps.MaxMP));
            else
                NowMp.text = "0";
            MaxMp.text = string.Format(_format, ps.MaxMP);
            Damage.text = string.Format(_format, ps.Damage);
            Defense.text = string.Format(_format, ps.Defense);

            int value = 0;
            if (ps.PlusHP > 0)
            {
                value = Mathf.RoundToInt(ps.PlusHP);
                PlusHp.text = string.Format($"+{_format}", value);
                PlusHp.gameObject.SetActive(true);
            }
            else
            {
                PlusHp.text = "0";
                PlusHp.gameObject.SetActive(false);
            }
            if (ps.PlusMP > 0)
            {
                value = Mathf.RoundToInt(ps.PlusMP);
                PlusMp.text = string.Format($"+{_format}", value);
                PlusMp.gameObject.SetActive(true);
            }
            else
            {
                PlusMp.text = "0";
                PlusMp.gameObject.SetActive(false);
            }
            if (ps.PlusDamage > 0)
            {
                value = Mathf.RoundToInt(ps.PlusDamage);
                PlusDamage.text = string.Format($"+{_format}", value);
                PlusDamage.gameObject.SetActive(true);
            }
            else
            {
                PlusDamage.text = "0";
                PlusDamage.gameObject.SetActive(false);
            }
            if (ps.PlusDefense > 0)
            {
                value = Mathf.RoundToInt(ps.PlusDefense);
                PlusDefense.text = string.Format($"+{_format}", value);
                PlusDefense.gameObject.SetActive(true);
            }
            else
            {
                PlusDefense.text = "0";
                PlusDefense.gameObject.SetActive(false);
            }

            if (ps.Gold > 0)
                Gold.text = string.Format(_format, ps.Gold);
            else
                Gold.text= "0";
            yield return null;
        }
        _isRunning = false;
    }
}
