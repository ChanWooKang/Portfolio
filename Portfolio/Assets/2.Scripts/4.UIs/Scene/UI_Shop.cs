using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Define;

public class UI_Shop : UI_Base
{
    enum GameObjects
    {
        PlayerMoney,
        Close
    }

    public static bool ActivatedShop = false;
    List<SOItem> list_Sells = new List<SOItem>();
    [SerializeField]
    GameObject ShopPage;
    UI_BuySlot[] SellSlots;

    Text PlayerMoney;
    int beforeMoney;
    const string _format = "{0:#,###}";

    Coroutine MoneyCoroutine = null;
    void Start()
    {
        Init();
    }

    public override void Init()
    {
        beforeMoney = 0;
        Bind<GameObject>(typeof(GameObjects));
        BindEvent(GetObject((int)GameObjects.Close), (PointerEventData data) => { if (data.button == PointerEventData.InputButton.Left) CloseUI(); });
        PlayerMoney =GetObject((int)GameObjects.PlayerMoney).GetComponent<Text>();
        SellSlots = GetComponentsInChildren<UI_BuySlot>();
        for (int i = 0; i < SellSlots.Length; i++)
            SellSlots[i].Init();
        CloseUI();
    }

    public void OpenUI()
    {
        ActivatedShop = true;
        UpdateItem();
        if(MoneyCoroutine != null)
        {
            StopCoroutine(MoneyCoroutine);
        }
        MoneyCoroutine = StartCoroutine(UpdateMoneyStat());
        ShopPage.SetActive(true);
        GameManagerEX._inst.OpenUISoundEvent();
    }

    public void CloseUI()
    {
        ActivatedShop = false;
        if (MoneyCoroutine != null)
        {
            StopCoroutine(MoneyCoroutine);
        }
        ShopPage.SetActive(false);
        GameManagerEX._inst.OpenUISoundEvent();
    }

    void SetMoneyStat()
    {
        PlayerMoney.text = string.Format(_format, PlayerCtrl._inst._stat.Gold);
        beforeMoney = PlayerCtrl._inst._stat.Gold;
    }

    public void UpdateItem()
    {
        list_Sells.Clear();
        for (int i = 0; i < SellSlots.Length; i++)
            SellSlots[i].AddItem(PickItem());
    }

    SOItem PickItem()
    {
        SOItem temp = null;
        int max = InventoryManager._inst.items.Length;
        int value = Random.Range(1, max);
        temp = InventoryManager._inst.items[value];
        return temp;
    }

    IEnumerator UpdateMoneyStat()
    {
        while(ActivatedShop)
        {
            if(beforeMoney != PlayerCtrl._inst._stat.Gold)
                SetMoneyStat();
            yield return null;
        }
    }
}
