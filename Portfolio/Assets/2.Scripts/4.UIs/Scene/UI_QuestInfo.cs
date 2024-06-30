using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Define;

public class UI_QuestInfo : UI_Base
{
    [SerializeField] GameObject BackGround;
    [SerializeField] GameObject ProgressUI;
    [SerializeField] GameObject ClearUI;
    [SerializeField] Text Description;
    [SerializeField] Text Value;

    string _format = "{0} / {1}";
   
    public override void Init()
    {
        SetUI(false,false, false);
    }

    public void SetProgressUI(QuestData data)
    {
        Description.text = data.title;
        Value.text = string.Format(_format, data.quest.nowCount, data.quest.goalCount);
        SetUI(true,true, false);
    }

    public void SetUI(bool bg,bool progress,bool clear)
    {        
        ProgressUI.SetActive(progress);
        ClearUI.SetActive(clear);
        BackGround.SetActive(bg);
    }

    public void ActiveQuestUI(bool isActive)
    {
        BackGround.SetActive(isActive);
        
    }

   
}
