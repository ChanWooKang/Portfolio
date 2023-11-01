using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers _uniqueInstance;
    static Managers _inst { get { Init(); return _uniqueInstance; } }

    DataManager Data = new DataManager();
    FileManager File = new FileManager();
    InputManager Input = new InputManager();
    ResourceManager Resource = new ResourceManager();
    SceneManagerEx Scene = new SceneManagerEx();
    UIManager UI = new UIManager();

    public static DataManager _data { get { return _inst.Data; } }
    public static FileManager _file { get { return _inst.File; } }
    public static InputManager _input { get { return _inst.Input; } }
    public static ResourceManager _resource { get { return _inst.Resource; } }
    public static SceneManagerEx _scene { get { return _inst.Scene; } }
    public static UIManager _ui { get { return _inst.UI; } }

    public static bool IsNew { get; set; } = true;
    static void Init()
    {
        if(_uniqueInstance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if(go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go);
            _uniqueInstance = go.GetComponent<Managers>();

            //반드시 파일 -> 데이터
            _uniqueInstance.File.Init();
            _uniqueInstance.Data.Init();
            _uniqueInstance.Scene.Init();
        }
    }

    public static void Clear()
    {
        _input.Clear();
        _scene.Clear();
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        _input.OnUpdate();
    }
}
