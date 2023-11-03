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
    [SerializeField]
    GameObject ShopPage;
    UI_BuySlot[] BuySlots;
    UI_SellSlot[] SellSlots;
    Text PlayerMoney;
    int beforeMoney;
    const string _format = "{0:N0}";

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
        BuySlots = GetComponentsInChildren<UI_BuySlot>();
        SellSlots = GetComponentsInChildren<UI_SellSlot>();
        for (int i = 0; i < BuySlots.Length; i++)
            BuySlots[i].Init();

        for(int i = 0; i < SellSlots.Length; i++)
        {
            SellSlots[i].Init();
            SellSlots[i].gameObject.SetActive(false);
        }

        ActivatedShop = false;
        ShopPage.SetActive(false);
    }

    public void OpenUI()
    {
        ActivatedShop = true;
        UpdateBuyItem();
        UpdateInventoryItemByOpen();
        if (MoneyCoroutine != null)
        {
            StopCoroutine(MoneyCoroutine);
        }
        MoneyCoroutine = StartCoroutine(UpdateMoneyStat());
        ShopPage.SetActive(true);
        SoundManager._inst.Play(eSoundList.UI_Open);
    }

    public void CloseUI()
    {
        ActivatedShop = false;
        ShopPage.SetActive(false);
        UpdateInventoryItemByClose();
        if (MoneyCoroutine != null)
            StopCoroutine(MoneyCoroutine);

        UI_ItemInfo._inst.OffInforMation();
    }

    #region [ Buy ]

    public void UpdateBuyItem()
    {
        for (int i = 0; i < BuySlots.Length; i++)
            BuySlots[i].AddItem(PickItem());
    }

    SOItem PickItem()
    {
        int max = InventoryManager._inst.items.Length;
        int value = Random.Range(1, max);
        return InventoryManager._inst.items[value];
    }

    

    #endregion [ Buy ]

    #region [ Sell ]

    public void UpdateInventoryItemByOpen()
    {
        UI_Slot[] InvenData = InventoryManager._inst.Inven_Slots;
        int index = 0;
        for (int i = 0;  i <SellSlots.Length; i++)
        {
            if(SellSlots[i].matchSlot != null)
            {
                index++;
            }
        }
        
        for(int i = index; i < InvenData.Length; i++)
        {
            if(InvenData[i].item != null)
            {
                SellSlots[index].AddItem(InvenData[i], InvenData[i].itemCount);
                SellSlots[index].gameObject.SetActive(true);
                index++;
            }
        }
    }

    public void UpdateInventoryItemByClose()
    {
        for(int i = 0; i < SellSlots.Length; i++)
        {
            if(SellSlots[i].matchSlot == null)
                SellSlots[i].gameObject.SetActive(false);
        }
    }
    #endregion [ Sell ]

    void SetMoneyStat()
    {
        PlayerMoney.text = string.Format(_format, PlayerCtrl._inst._stat.Gold);
        beforeMoney = PlayerCtrl._inst._stat.Gold;
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
