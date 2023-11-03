using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Define;

public class UI_Alert : UI_Base
{
    enum GameObjects
    {
        QuitBackGround,
        ShopAlert,
        Main,
        Quit,
        Close,
        ShopDesc,
        ShopClose,
    }

    GameObject QuitAlert;
    GameObject ShopAlert;
    Text ShopDesc;
    void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        QuitAlert = GetObject((int)GameObjects.QuitBackGround);
        ShopAlert = GetObject((int)GameObjects.ShopAlert);
        ShopDesc = GetObject((int)GameObjects.ShopDesc).GetComponent<Text>();
        GameObject go = GetObject((int)GameObjects.Main);
        BindEvent(go, (PointerEventData data) => { if (data.button == PointerEventData.InputButton.Left) GoToMainScene(); },UIEvent.Click);
        GameObject go2 = GetObject((int)GameObjects.Quit);
        BindEvent(go2, (PointerEventData data) => { if (data.button == PointerEventData.InputButton.Left) Quit(); }, UIEvent.Click);
        GameObject go3 = GetObject((int)GameObjects.Close);
        BindEvent(go3, (PointerEventData data) => { if (data.button == PointerEventData.InputButton.Left) CloseAlert(); }, UIEvent.Click);
        GameObject go4 = GetObject((int)GameObjects.ShopClose);
        BindEvent(go4, (PointerEventData data) => { if (data.button == PointerEventData.InputButton.Left) CloseAlert(); }, UIEvent.Click);

        CloseAlert();
    }

    //public void TryOpen(bool isQuit = true)
    //{
    //    if (isQuit)
    //    {
    //        if (QuitAlert.activeSelf)
    //            CloseAlert();
    //        else
    //            OpenAlert();
    //    }
    //    else
    //    {
    //        if (ShopAlert.activeSelf)
    //            CloseAlert(false);
    //        else
    //            OpenAlert(false);
    //    }
        
    //}


    public void OpenAlert(bool isQuit = true)
    {

        SetTimeScale(0);
        if (isQuit)
        {
            QuitAlert.SetActive(true);
            ShopAlert.SetActive(false);
        }
        else
        {
            QuitAlert.SetActive(false);
            ShopAlert.SetActive(true);
        }
    }

    public void CloseAlert(bool isQuit = true)
    {
        SetTimeScale(1);
        if (isQuit)
        {
            QuitAlert.SetActive(false);
            ShopAlert.SetActive(false);
        }
        else
        {
            QuitAlert.SetActive(false);
            ShopAlert.SetActive(false);
        }
    }

    public void SettingShopAlert(string desc)
    {
        ShopDesc.text = desc;
        OpenAlert(false);
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
