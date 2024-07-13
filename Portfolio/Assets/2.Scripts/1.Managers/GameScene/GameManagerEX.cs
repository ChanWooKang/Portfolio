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
    public int TotalKill { get { SetTotalKill(); return totalKill; } }
    PlayerCtrl player;
    UI_Inventory inven;
    UI_WorldMap world;
    UI_Alert alt;
    UI_GameEnd end;
    UI_Shop shop;
    public UI_Information information;
    public UI_Talk talk;
    public UI_Quest questUI;

    public QuestManager questManager;
    public QuestProvider contactProvider;

    int totalKill;
    float InGameTime;
    public bool isGameEnd = false;
    Dictionary<eMonster, int> _killDict = new Dictionary<eMonster, int>();

    public TalkManager talkManager;    
    public int talkIndex;
    public bool StopMove = false;
    public GameObject ScanObject;
    public eInteract ScanType;
    public bool endTalk = false;

    void Awake()
    {
        _uniqueInstance = this;
    }

    void Start()
    {
        DictionarySetting();

        if (Managers.IsNew)
        {
            ResetData();
            questManager.Init();
        }
        else
        {
            questManager.LoadQuestData();
        }
            

        player = PlayerCtrl._inst;
        //Sounds
        SoundManager._inst.Play(eSoundList.BGM_GameScene, eSound.BGM);
        //스킬 체크
        Debug.Log(player._stat.Level);
        player.InitData();
        information.CheckSkillAble();
        //장비 체크
        player.pec.SettingEquipment();
    }

    void Update()
    {
        if (isGameEnd == false)
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
      
        if (UI_WorldMap.ActivatedWorldMap || UI_Shop.ActivatedShop)
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

        if (UI_Inventory.ActivatedInventory || UI_Shop.ActivatedShop)
            return;

        if (world == null)
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

        if (shop == null)
            shop = FindObjectOfType<UI_Shop>();

        SoundManager._inst.StopAudio();

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

        if (UI_Shop.ActivatedShop)
        {
            shop.CloseUI();
            return;
        }

        if (!UI_WorldMap.ActivatedWorldMap && !UI_Inventory.ActivatedInventory && !UI_Shop.ActivatedShop)
        {
            if (alt == null)
                alt = FindObjectOfType<UI_Alert>();

            if(player.State != PlayerState.Idle)
                player.Stop();
            OpenUISoundEvent();
            alt.OpenAlert();
        }
    }

    #endregion

    public void ShopAlert(string desc)
    {
        if (alt == null)
            alt = FindObjectOfType<UI_Alert>();

        alt.SettingShopAlert(desc);
    }


    void UIOpen(bool isOver)
    {
        if (end == null)
            end = FindObjectOfType<UI_GameEnd>();

        end.OpenUI(isOver);
    }

    public void SetTotalKill()
    {
        int sum = 0;
        foreach (var data in _killDict)
        {
            sum += data.Value;
        }
        totalKill = sum;
    }



    public void GameOver()
    {
        SoundManager._inst.Play(eSoundList.GameOver);
        player.gameObject.SetActive(false);
        player.ChangeColor(Color.white);
        isGameEnd = true;
        UIOpen(true);
        
    }



    public void GameClear(BossCtrl bc)
    {
        SoundManager._inst.Play(eSoundList.GameClear);
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
    }

    public void ResetCountTime()
    {
        InGameTime = 0;
        isGameEnd = false;
        _killDict = new Dictionary<eMonster, int>();
    }

    public void KillCount(eMonster type, int cnt = 1)
    {
        //Test
        if (questManager.isProgress)
        {
            questManager.AddCount(type, cnt);
            
        }

        if (_killDict.ContainsKey(type))
        {
            _killDict[type] += cnt;
        }
        else
        {
            _killDict.Add(type, cnt);
        }
    }

    #region [ Talk ]
    public void ShowText(GameObject scanObj, eInteract type)
    {
        ScanObject = scanObj;
        ScanType = type;
        
        ObjectData objData = scanObj.GetComponent<ObjectData>();
        if (player.State != PlayerState.Idle) 
            player.Stop();        
        if (OnTalk(objData.id, objData.krName) == false)
        {
            ScanObject = null;
            ScanType = eInteract.Unknown;

            // 진행할거 , 퀘스트 Or 상점 열기
            switch (type)
            {
                case eInteract.Shop:
                    if (scanObj.TryGetComponent<Shop>(out Shop shop))
                    {
                        talk.SetOnOff(false);
                        shop.OpenShop();                        
                    }
                    break;
                case eInteract.NPC:
                    if (scanObj.TryGetComponent<QuestProvider>(out QuestProvider quest))
                    {
                        if (endTalk)
                        {
                            talk.SetOnOff(false);
                            StopMove = false;
                            endTalk = false;
                            return;
                        }

                        if (questManager.isProgress)
                        {
                            if (questManager.ProgerssQuest.isSucess)
                            {                                
                                OnTalk(objData.id + 2, objData.krName);
                                AddReward(questManager.ProgerssQuest.reward);
                                questManager.isProgress = false;
                                questManager.DisableUI();

                                QuestData data = questManager.ProgerssQuest;
                                contactProvider.SetNowQuest(data.questID, false, true);
                                endTalk = true;
                            }
                            else
                            {
                                OnTalk(objData.id + 1, objData.krName);
                                endTalk = true;
                            }
                        }
                        else
                        {
                            
                            if (quest.OpenQuest())
                            {
                                OnTalk(objData.id + 3, objData.krName);
                                endTalk = true;
                            }
                            else
                            {
                                talk.SetOnOff(false);
                            }
                        }                        
                    }
                    break;
            }
        }
        else
        {
            talk.SetOnOff(true);
        }
    }

    bool OnTalk(int id , string speaker = null)
    {
        string talkData = talkManager.GetTalk(id, talkIndex);
        if (string.IsNullOrEmpty(talkData))
        {            
            StopMove = false;
            talkIndex = 0;
            return false;
        }
        talk.SetText(speaker, talkData);

        StopMove = true;
        talkIndex++;
        return true;
    }
    #endregion [ Talk ]

    public void ShowQuest(QuestProvider provider,QuestData data)
    {
        contactProvider = provider;
        questUI.OpenUI(data);
    }

    public void AcceptQuest(QuestData data)
    {
        if (questManager.isProgress)
        {
            //진행중인 퀘스트가 있습니다 알림
        }
        else
        {
            contactProvider.SetNowQuest(data.questID, true, false);
            questManager.AcceptQuest(data);
        }        
    }

    public void RejectQuest()
    {
        contactProvider = null;
    }

    public void AddReward(List<RewardItem> items)
    {
        for(int i = 0; i < items.Count; i++)
        {
            InventoryManager._inst.AddInvenItem(items[i].item, items[i].rewardCount);
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
        if (Managers._data.killData != null)
        {
            KillData saveData = Managers._data.killData;

            int i = 0;
            for (; i < saveData.MonsterType.Count; i++)
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