using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class MainScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        CurrScene = eScene.MainScene;
    }

    public override void Clear()
    {
        base.Clear();
    }

    public override void NewGame()
    {
        Managers.IsNew = true;
        Managers._data.ResetData();
        SceneLoad(eScene.TestScene);
    }

    public override void ContinueGame()
    {
        Managers.IsNew = false;
        SceneLoad(eScene.TestScene);
    }

    public override void Quit()
    {
        base.Quit();
    }
}
