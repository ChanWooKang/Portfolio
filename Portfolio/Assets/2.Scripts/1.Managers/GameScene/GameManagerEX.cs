using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;
using DataContents;

public class GameManagerEX : MonoBehaviour
{
    static GameManagerEX _uniqueInstance;
    public static GameManagerEX _inst { get { return _uniqueInstance; } }
    public float GameTime { get { return InGameTime; } }
    public int TotalKill { get {  SetTotalKill(); return totalKill; }}
    PlayerCtrl player;
    UI_Inventory inven;
    UI_WorldMap world;
    UI_Alert alt;
    UI_GameEnd end;

    int totalKill;
    float InGameTime;
    public bool isGameEnd = false;
    Dictionary<eMonster, int> _killDict = new Dictionary<eMonster, int>();
    void Awake()
    {
        _uniqueInstance = this;
    }

    void Start()
    {
        DictionarySetting();

        if (Managers.IsNew)
            ResetData();

        player = PlayerCtrl._inst;
        //Sounds
        SoundManager._inst.Play(eSoundList.BGM_GameScene, eSound.BGM);
    }

    void Update()
    {
        if(isGameEnd == false)
        {
            InGameTime += Time.deltaTime;
            InventoryEvent(Input.GetKeyDown(KeyCode.I));
            WorldMapEvent(Input.GetKeyDown(KeyCode.M));
            ESCEvent(Input.GetKeyDown(KeyCode.Escape));
        }
    }

    void DictionarySetting()
    {
        if (Managers.IsNew)
        {
            for (int i = 1; i < (int)eMonster.Max_Cnt; i++)
            {
                if (_killDict.ContainsKey((eMonster)i))
                {
                    _killDict[(eMonster)i] = 0;
                }
                else
                {
                    KillCount((eMonster)i, 0);
                }
            }
        }
        else
        {
            CountLoad();
        }
        
    }

    public void OpenUISoundEvent()
    {
        SoundManager._inst.Play(eSoundList.UI_Open);
    }

    #region [ Key Event ]
    void InventoryEvent(bool btnDown)
    {        
        if (btnDown == false)
            return;

        if (UI_WorldMap.ActivatedWorldMap)
            return;
        
        if (inven == null)
            inven = FindObjectOfType<UI_Inventory>();

        OpenUISoundEvent();
        inven.TryOpenInventory();
    }

    void WorldMapEvent(bool btnDown)
    {
        if (btnDown == false)
            return;

        if (UI_Inventory.ActivatedInventory)
            return;

        if(world == null)
            world = FindObjectOfType<UI_WorldMap>();

        OpenUISoundEvent();
        world.TryOpenWorldMap();
    }

    public void ESCEvent(bool btnDown)
    {
        if (btnDown == false)
            return;

        if (inven == null)
            inven = FindObjectOfType<UI_Inventory>();

        if (world == null)
            world = FindObjectOfType<UI_WorldMap>();

        if (UI_WorldMap.ActivatedWorldMap)
        {
            world.CloseUI();
            return;
        }

        if (UI_Inventory.ActivatedInventory)
        {
            inven.CloseInventory();
            return;
        }

        if(!UI_WorldMap.ActivatedWorldMap && !UI_Inventory.ActivatedInventory)
        {
            if (alt == null)
                alt = FindObjectOfType<UI_Alert>();

            OpenUISoundEvent();
            alt.TryOpen();
        }

    }

    #endregion

    void UIOpen(bool isOver)
    {
        if (end == null)
            end = FindObjectOfType<UI_GameEnd>();

        end.OpenUI(isOver);
    }

    public void SetTotalKill()
    {
        int sum = 0;
        foreach(var data in _killDict)
        {
            sum += data.Value;
        }
        totalKill = sum;
    }

    

    public void GameOver()
    {
        player.gameObject.SetActive(false);
        player.ChangeColor(Color.white);
        isGameEnd = true;
        UIOpen(true);
    }

    

    public void GameClear(BossCtrl bc)
    {
        KillCount(eMonster.Boss);
        isGameEnd = true;
        UIOpen(false);
    }


    public void ReGame()
    {
        if (player != null)
        {
            //player.ResetStat();
            //player.OnResurrectEvent();
            //player.gameObject.SetActive(true);
            //player.OnStartRegenarte();
            //ResetCountTime();
            
        }

        ResetData();
        Managers.IsNew = true;
        Managers._scene.CurrentScene.SceneLoad(eScene.GameScene);

    }

    public void ResetData()
    {
        PlayerCtrl._inst.ResetStat();
        InventoryManager._inst.ResetInventory();
        InGameTime = 0;
        totalKill = 0;
        _killDict = new Dictionary<eMonster, int>();
        Managers._data.ResetData();
        ResetCountTime();
        Debug.Log("Reset ¿Ï·á");
    }

    public void ResetCountTime()
    {
        InGameTime = 0;
        isGameEnd = false;
        _killDict = new Dictionary<eMonster, int>();
    }

    public void KillCount(eMonster type, int cnt = 1)
    {
        if (_killDict.ContainsKey(type))
        {
            _killDict[type] += cnt;
        }
        else
        {
            _killDict.Add(type, cnt);
        }
    }

    #region [ Save & Load ]
    public KillData CountSave()
    {
        KillData saveData = new KillData();
        foreach (var data in _killDict)
        {
            saveData.MonsterType.Add((int)data.Key);
            saveData.KillCount.Add(data.Value);
        }           
        saveData.InGameTime = InGameTime;
        return saveData;
    }

    public void CountLoad()
    {
        if(Managers._data.killData != null)
        {
            KillData saveData = Managers._data.killData;

            int i = 0; 
            for(; i < saveData.MonsterType.Count; i++)
            {
                KillCount((eMonster)saveData.MonsterType[i], saveData.KillCount[i]);
            }
            InGameTime = saveData.InGameTime;
        }
        else
        {
            DictionarySetting();
            InGameTime = 0;
        }
            
    }
    #endregion [ Save & Load ]
}
