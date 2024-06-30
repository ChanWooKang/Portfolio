using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Quest : UI_Base
{
    enum GameObjects
    {
        BackGround,
        Close,
        Title,
        Desc,
    }

    GameObject main;
    Text title;
    Text description;
    UI_RewardSlot[] slots;

    QuestData openQuestData;

    public static bool ActivatedQuestWindow = false;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        
        Bind<GameObject>(typeof(GameObjects));
        main = GetObject((int)GameObjects.BackGround);
        title = GetObject((int)GameObjects.Title).GetComponent<Text>();
        description = GetObject((int)GameObjects.Desc).GetComponent<Text>();
        BindEvent(GetObject((int)GameObjects.Close), (PointerEventData data) => { if (data.button == PointerEventData.InputButton.Left) CloseUI(); });
        slots = GetComponentsInChildren<UI_RewardSlot>();
        for(int i = 0; i < slots.Length; i++)
        {
            slots[i].Init();
            slots[i].gameObject.SetActive(false);
        }

        ActivatedQuestWindow = false;
        main.SetActive(false);
    }

    
    public void OpenUI(QuestData data)
    {
        ActivatedQuestWindow = true;
        GameManagerEX._inst.StopMove = true;
        openQuestData = data;
        SetQuestUI(data);
        main.SetActive(true);
    }

    public void CloseUI()
    {
        ActivatedQuestWindow = false;
        GameManagerEX._inst.StopMove = false;
        ClearRewardSlot();
        main.SetActive(false);
    }


    void SetQuestUI(QuestData data)
    {
        title.text = data.title;
        description.text = data.desc;

        for (int i = 0; i < data.reward.Count; i++)
        {
            slots[i].AddItem(data.reward[i].item, data.reward[i].rewardCount);
            slots[i].gameObject.SetActive(true);
        }
    }

    void ClearRewardSlot()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if(slots[i].gameObject.activeSelf)
            {
                slots[i].ClearSlot();
                slots[i].gameObject.SetActive(false);
            }
        }
    }

    public void AcceptQuest()
    {
        GameManagerEX._inst.AcceptQuest(openQuestData);        
        CloseUI();
    }

    public void RejectQuest()
    {
        GameManagerEX._inst.RejectQuest();
        CloseUI();
        openQuestData = null;
    }
}
