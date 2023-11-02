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
        SceneLoad(eScene.GameScene);
    }

    public override void ContinueGame()
    {
        Managers.IsNew = false;
        SceneLoad(eScene.GameScene);
    }

    public override void Quit()
    {
        base.Quit();
    }
}
