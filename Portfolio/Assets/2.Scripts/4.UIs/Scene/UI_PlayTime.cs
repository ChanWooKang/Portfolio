using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Define;

public class UI_PlayTime : UI_Base
{
    enum Texts
    {
        TimeText
    }

    const string format = "{0:D2} : {1:D2}";
    Text Ttext;

    void Start()
    {
        Init();
    }

    void Update()
    {
        SettingUI();
    }


    public override void Init()
    {
        Bind<GameObject>(typeof(Texts));
        Ttext = GetObject((int)Texts.TimeText).GetComponent<Text>();
    }

    void SettingUI()
    {
        float time = GameManagerEX._inst.GameTime;
        int min = (Mathf.FloorToInt(time) / 60 % 60);
        int sec = (Mathf.FloorToInt(time) % 60);

        Ttext.text = string.Format(format, min, sec);
    }
}
