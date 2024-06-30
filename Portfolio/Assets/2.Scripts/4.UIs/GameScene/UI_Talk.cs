using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Define;

public class UI_Talk : UI_Base
{    
   enum GameObjects
    {
        BackGround,
        TalkName,
        Dialogue,
        ETC
    }

    GameObject main;
    Text speakerName;
    Text dialogue;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        main = GetObject((int)GameObjects.BackGround);
        speakerName = GetObject((int)GameObjects.TalkName).GetComponent<Text>();
        dialogue = GetObject((int)GameObjects.Dialogue).GetComponent<Text>();

    }

    void Start()
    {
        Init();
        SetOnOff(false);
    }

    public void SetText(string speaker, string value)
    {
        speakerName.text = speaker;
        dialogue.text = value;
    }

    public void SetOnOff(bool isActive)
    {
        if(main.activeSelf != isActive)
            main.SetActive(isActive);
    }
}
