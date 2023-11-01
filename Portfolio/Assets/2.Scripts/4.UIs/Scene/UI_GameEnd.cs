using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Define;

public class UI_GameEnd : UI_Base
{
    enum GameObjects
    {
        Pannel,
        Title,
        PlayTime,
        KillCount,
        ReGame,
        Main,
        Quit
    }

    GameObject Pannel;
    Text title;
    Text playTime;
    Text killCount;

    const string GameOver = "게임 오버";
    const string GameClear = "게임 클리어";
    const string timeformat = "{0:D2} : {1:D2}";
    const string killformat = "{0} 마리 처치";
    void Start()
    {
        Init();
        CloseUI();
    }


    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Pannel = GetObject((int)GameObjects.Pannel);
        title = GetObject((int)GameObjects.Title).GetComponent<Text>();
        playTime = GetObject((int)GameObjects.PlayTime).GetComponent<Text>();
        killCount = GetObject((int)GameObjects.KillCount).GetComponent<Text>();
        GameObject go = GetObject((int)GameObjects.Main);
        BindEvent(go, (PointerEventData data) => { if (data.button == PointerEventData.InputButton.Left) GoToMainScene(); }, UIEvent.Click);
        GameObject go2 = GetObject((int)GameObjects.Quit);
        BindEvent(go2, (PointerEventData data) => { if (data.button == PointerEventData.InputButton.Left) Quit(); }, UIEvent.Click);
        GameObject go3 = GetObject((int)GameObjects.ReGame);
        BindEvent(go3, (PointerEventData data) => { if (data.button == PointerEventData.InputButton.Left) ReGame(); }, UIEvent.Click);
    }
    void SetTimeScale(float time = 1)
    {
        Time.timeScale = time;
    }

    public void OpenUI(bool isOver)
    {
        SetTimeScale(0);
        if (isOver)
        {
            title.text = GameOver;
        }
        else
        {
            title.text = GameClear;
        }
        TextSetting();
        Pannel.SetActive(true);
    }

    public void CloseUI()
    {
        Pannel.SetActive(false);
    }

    void TextSetting()
    {
        float time = GameManagerEX._inst.GameTime;
        int min = (Mathf.FloorToInt(time)/ 60 % 60);
        int sec = (Mathf.FloorToInt(time) % 60);

        playTime.text = string.Format(timeformat, min, sec);
        killCount.text = string.Format(killformat, GameManagerEX._inst.TotalKill.ToString());
    }


    public void ReGame()
    {
        SetTimeScale(1);
        GameManagerEX._inst.ReGame();
        CloseUI();
    }

    public void GoToMainScene()
    {
        SetTimeScale(1);
        GameManagerEX._inst.ResetData();
        Managers._scene.CurrentScene.SceneLoad(eScene.MainScene);
    }

    public void Quit()
    {
        SetTimeScale(1);
        GameManagerEX._inst.ResetData();
        Managers._scene.CurrentScene.Quit();
    }
}
