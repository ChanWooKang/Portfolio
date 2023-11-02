using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Define;

public class UI_Alert : UI_Base
{
    enum GameObjects
    {
        BackGround,
        Main,
        Quit,
        Close
    }

    GameObject BackGround;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        BackGround = GetObject((int)GameObjects.BackGround);

        GameObject go = GetObject((int)GameObjects.Main);
        BindEvent(go, (PointerEventData data) => { if (data.button == PointerEventData.InputButton.Left) GoToMainScene(); },UIEvent.Click);
        GameObject go2 = GetObject((int)GameObjects.Quit);
        BindEvent(go2, (PointerEventData data) => { if (data.button == PointerEventData.InputButton.Left) Quit(); }, UIEvent.Click);
        GameObject go3 = GetObject((int)GameObjects.Close);
        BindEvent(go3, (PointerEventData data) => { if (data.button == PointerEventData.InputButton.Left) CloseAlert(); }, UIEvent.Click);

        CloseAlert();
    }

    public void TryOpen()
    {
        if (BackGround.activeSelf)
            CloseAlert();
        else
            OpenAlert();
    }


    public void OpenAlert()
    {

        SetTimeScale(0);
        BackGround.SetActive(true);
    }

    public void CloseAlert()
    {
        SetTimeScale(1);
        BackGround.SetActive(false);
    }

    public void GoToMainScene()
    {
        GameManagerEX._inst.isGameEnd = true;
        SetTimeScale(1);
        Managers._data.SaveGameData();
        Managers._scene.CurrentScene.SceneLoad(eScene.MainScene);
    }

    public void Quit()
    {
        GameManagerEX._inst.isGameEnd = true;
        SetTimeScale(1);
        Managers._scene.CurrentScene.Quit();
    }

    void SetTimeScale(float time = 1)
    {
        Time.timeScale = time;
    }
}
