using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;
using DataContents;

public class DataManager
{
    public Dictionary<int, DataByLevel> Dict_Stat { get; private set; } = new Dictionary<int, DataByLevel>();
    public Dictionary<eMonster, DataByMonster> Dict_Monster { get; private set; } = new Dictionary<eMonster, DataByMonster>();
    public PlayerData playerData = new PlayerData();
    public Inventorydata invenData = new Inventorydata();

    public void Init()
    {
        LoadData();
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        string text = Managers._file.LoadJsonFile(path);
        if (string.IsNullOrEmpty(text) == false)
            return JsonUtility.FromJson<Loader>(text);
        else
        {
            TextAsset data = Managers._resource.Load<TextAsset>($"Json/{path}");
            Loader datas = JsonUtility.FromJson<Loader>(data.ToString());
            Managers._file.SaveJsonFile(datas, path);
            return datas;
        }

    }

    void LoadData()
    {

        Dict_Stat = LoadJson<StatData, int, DataByLevel>("DataByLevel").Make();
        Dict_Monster = LoadJson<MonsterData, eMonster, DataByMonster>("DataByMonster").Make();
    }

    public void LoadGameData()
    {
        string player = Managers._file.LoadJsonFile("PlayerData");
        if (string.IsNullOrEmpty(player) == false)
            playerData = JsonUtility.FromJson<PlayerData>(player);
        else
            playerData = new PlayerData();

        string inven = Managers._file.LoadJsonFile("Inventorydata");
        if (string.IsNullOrEmpty(inven) == false)
            invenData = JsonUtility.FromJson<Inventorydata>(inven);
        else
            invenData = new Inventorydata();
    }

    public void SaveGameData()
    {
        //인벤토리 매니저 && 플레이어 스탯에 저장 콜 후 저장
        //플레이어 컨트롤 -> 플레이어 스텟 - > 세이브 플레이어 데이터 추출
        playerData = PlayerCtrl._inst._stat.SavePlayer();
        invenData = InventoryManager._inst.InventorySave();
        Managers._file.SaveJsonFile(playerData, "PlayerData");
        Managers._file.SaveJsonFile(invenData, "Inventorydata");
    }

}
