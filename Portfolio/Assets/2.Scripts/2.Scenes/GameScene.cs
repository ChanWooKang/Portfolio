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

        //�κ��丮 ���̺�
        Managers._data.SaveGameData();
        
    }

    void OnApplicationQuit()
    {
        Clear();
    }
}
