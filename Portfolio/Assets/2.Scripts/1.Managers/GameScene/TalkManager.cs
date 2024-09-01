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
        talkData.Add(1000, new string[] { "�ȳ� �ϽŰ�.", "���� ������ ������ ��󺸰�.." });
        talkData.Add(2000, new string[] { "�ȳ� !" });
        talkData.Add(2000 + 1, new string[] { "���� óġ�ϰ� ��!" });
        talkData.Add(2000 + 2, new string[] { "���� ���� �����̾�!" });
        talkData.Add(2000 + 3, new string[] { "���� ��Ź�Ұ� ����!" });        
    }

    public string GetTalk(int id, int talkIndex)
    {        
        if (talkIndex >= talkData[id].Length)
            return null;

        return talkData[id][talkIndex];
    }
}


