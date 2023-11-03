using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Define;

public class UI_WorldMap : UI_Base
{
    [SerializeField]
    MinimapCamera _cam;
    GameObject Base;

    public static bool ActivatedWorldMap = false;

    enum GameObjects
    {
        BackGround,
        Close
    }

    void Start()
    {
        Init();
    }

    public void TryOpenWorldMap()
    {
        if (ActivatedWorldMap == false)
            OpenUI();
        else
            CloseUI();
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Base = GetObject((int)GameObjects.BackGround);
        GameObject go = GetObject((int)GameObjects.Close);
        BindEvent(go, (PointerEventData data) => { if (data.button == PointerEventData.InputButton.Left) CloseUI(); }, UIEvent.Click);
        CloseUI();
    }

    public void CloseUI()
    {
        _cam.ChangeMiniMap();
        Base.SetActive(false);
        ActivatedWorldMap = false;
    }

    public void OpenUI()
    {
        _cam.ChangeWorldMap();
        Base.SetActive(true);
        ActivatedWorldMap = true;
    }
}
