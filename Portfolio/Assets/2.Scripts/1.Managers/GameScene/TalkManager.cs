using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData;

    private void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        GenerateData();
    }

    void GenerateData()
    {
        talkData.Add(1000, new string[] { "안녕 하신가.", "많은 물픔이 있으니 골라보게.." });
        talkData.Add(2000, new string[] { "안녕 !" });
        talkData.Add(2000 + 1, new string[] { "전부 처치하고 와!" });
        talkData.Add(2000 + 2, new string[] { "고마워 여기 보상이야!" });
        talkData.Add(2000 + 3, new string[] { "아직 부탁할게 없어!" });        
    }

    public string GetTalk(int id, int talkIndex)
    {        
        if (talkIndex >= talkData[id].Length)
            return null;

        return talkData[id][talkIndex];
    }
}


