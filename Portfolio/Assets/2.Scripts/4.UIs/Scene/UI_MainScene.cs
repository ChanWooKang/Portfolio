using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Define;


public class UI_MainScene : UI_Base
{
    enum GameObjects
    {
        NewGame,
        Continue,
        Quit
    }

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        GameObject go = GetObject((int)GameObjects.NewGame);
        BindEvent(go, (PointerEventData data) => { if (data.button == PointerEventData.InputButton.Left) StartGameButton(true); });
        GameObject go1 = GetObject((int)GameObjects.Continue);
        BindEvent(go1, (PointerEventData data) => { if (data.button == PointerEventData.InputButton.Left) StartGameButton(false); });
        GameObject go2 = GetObject((int)GameObjects.Quit);
        BindEvent(go2, (PointerEventData data) => { if (data.button == PointerEventData.InputButton.Left) QuitButton(); });
    }

    public void StartGameButton(bool isNew)
    {
        ClickUI();
        if (isNew)
        {
            Invoke("NewGame", 0.25f);
        }
        else
        {
            Invoke("Continue", 0.25f);
        }
    }
    public void QuitButton()
    {
        ClickUI();
        Invoke("Quit", 0.25f);
    }

    void NewGame()
    {
        Managers._scene.CurrentScene.NewGame();
    }

    void Continue()
    {
        Managers._scene.CurrentScene.ContinueGame();
    }

    void Quit()
    {
        Managers._scene.CurrentScene.Quit();
    }

    void ClickUI()
    {
        SoundManager._inst.Play(eSoundList.UI_Touch);
    }

    
}
