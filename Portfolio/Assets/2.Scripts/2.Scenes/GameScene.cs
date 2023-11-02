using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        CurrScene = eScene.GameScene;
        Managers._data.LoadGameData();
    }

    public override void Clear()
    {
        base.Clear();
       
    }



    void OnApplicationQuit()
    {
        Managers._data.SaveGameData();
        Clear();
    }

    public override void NewGame()
    {
        
    }

    public override void ContinueGame()
    {
        
    }
}
