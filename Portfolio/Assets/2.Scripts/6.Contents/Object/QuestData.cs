using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

[System.Serializable]
public class QuestData
{
    public int questID;
    public int npcID;

    public bool isSucess;
    public bool Progress;
    
    public string title;
    public eMonster mType;
    public string desc;    

    public List<RewardItem> reward;
    public int rewardGold;

    public Quest quest;

}

[System.Serializable]
public class RewardItem
{
    public SOItem item;
    public int rewardCount;
}

[System.Serializable]
public class Quest
{
    public int nowCount;
    public int goalCount;
    public int clearNPCID;
}
