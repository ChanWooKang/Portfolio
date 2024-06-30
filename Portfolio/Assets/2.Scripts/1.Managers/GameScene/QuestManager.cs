using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;
using DataContents;

public class QuestManager : MonoBehaviour
{
    public QuestProvider[] givers;
    public UI_QuestInfo quest;
    public QuestData ProgerssQuest;
    public bool isProgress = false;
    public bool isClear = false;    

    public void Init()
    {
        quest.SetUI(false, false, false);
    }


    public QuestSaveData SaveQuestData()
    {
        QuestSaveData saveData = new QuestSaveData();
        List<int> npcs = new List<int>();
        List<int> nowQuest = new List<int>();
        List<int> nowCount = new List<int>();

        for (int i = 0; i < givers.Length; i++)
        {
            npcs.Add(givers[i].npcID);
            nowQuest.Add(givers[i].nowQuestID);
            
            if(givers[i].nowQuestID == ProgerssQuest.questID)
            {
                Debug.Log(ProgerssQuest.quest.nowCount);
                nowCount.Add(ProgerssQuest.quest.nowCount);                
            }                
            else
            {
                nowCount.Add(0);
            }                
        }
        
        saveData.NPCDatas = npcs;
        saveData.QuestIndex = nowQuest;
        saveData.nowCount = nowCount;
        return saveData;
    }

    public void LoadQuestData()
    {
        if(Managers._data.questData != null)
        {
            QuestSaveData saveData = Managers._data.questData;
            for (int i = 0; i < saveData.NPCDatas.Count; i++) 
            {
                for (int j = 0; j < givers.Length; j++)
                {
                    if(saveData.NPCDatas[i] == givers[j].npcID)
                    {
                        givers[j].SetQuest(saveData.QuestIndex[i], saveData.nowCount[i]);                                                
                        break;
                    }
                }
            }
        }

        if(ProgerssQuest != null)
        {
            if (ProgerssQuest.isSucess)
            {
                quest.SetUI(true,false, true);
                return;
            }

            if (ProgerssQuest.Progress)
            {
                quest.SetProgressUI(ProgerssQuest);
                return;
            }            
        }

        quest.SetUI(false, false, false);
        
    }

    public void AddCount(eMonster type, int cnt = 1)
    {
        if(ProgerssQuest.mType == type)
        {
            ProgerssQuest.quest.nowCount += cnt;
            quest.SetProgressUI(ProgerssQuest);
            if(ProgerssQuest.quest.nowCount >= ProgerssQuest.quest.goalCount)
            {
                ProgerssQuest.isSucess = true;
                ProgerssQuest.Progress = true;
                ClearQuest();
            }
        }
    }
    
    public void AcceptQuest(QuestData data)
    {
        ProgerssQuest = data;
        isProgress = true;


        quest.SetProgressUI(ProgerssQuest);
    }

    void ClearQuest()
    {
        quest.SetUI(true,false, true);
        isClear = true;
    }

    public void DisableUI()
    {
        quest.ActiveQuestUI(false);
        quest.SetUI(false,false, false);

    }

   
    
}
