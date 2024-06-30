using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestProvider : MonoBehaviour
{
    
    public List<QuestData> quests;
    ObjectData objData;
    public int npcID;
    public int nowQuest;
    public int nowQuestID;

    void Start()
    {
        Init();
    }

    void Init()
    {
        objData = GetComponent<ObjectData>();        
    }

    public void SetQuest(int index, int cnt)
    {
        //로드 해서 클리어 번호 알아내기
        nowQuestID = index;
        nowQuest = index % 10;
        bool isCheck;
        for(int i = 0; i < quests.Count; i++)
        {
            isCheck = false;
            quests[i].Progress = false;
            quests[i].isSucess = false;
            if ( i == nowQuest)
            {
                if(cnt != 0)
                {
                    quests[i].quest.nowCount = cnt;
                    if(quests[i].quest.nowCount == quests[i].quest.goalCount)
                    {
                        quests[i].Progress = false;
                        quests[i].isSucess = true;
                    }
                    else
                    {
                        quests[i].Progress = true;
                    }
                    
                    isCheck = true;
                }
            }
            else if(i < nowQuest)
            {
                quests[i].isSucess = true;
            }            
            
            if(isCheck)
            {
                GameManagerEX._inst.questManager.ProgerssQuest = quests[i];
                GameManagerEX._inst.questManager.isProgress = quests[i].Progress;
            }            
        }
        
    }   

    public void SetNowQuest(int value, bool isProgress,bool success)
    {
        nowQuestID = value;
        nowQuest = value % 10;
        quests[nowQuest].Progress = isProgress;
        quests[nowQuest].isSucess = success;
    }

    public bool OpenQuest()
    {                
        if(quests[nowQuest].isSucess == false)
            GameManagerEX._inst.ShowQuest(this, quests[nowQuest]);
        else
        {
            if(quests.Count > nowQuest + 1)
            {
                nowQuest++;
                GameManagerEX._inst.ShowQuest(this, quests[nowQuest]);
                return false;
            }
            else
            {
                Debug.Log("퀘스트 전부 클리어");
                return true;
            }
        }
        return false;
    }
}
