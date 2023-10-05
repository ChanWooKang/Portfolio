using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FileManager
{
    const string JsonSuffix = ".json";
    string JsonPath;

    public void Init()
    {
#if UNITY_EDITOR
        JsonPath = Application.dataPath + "/8.Datas/Json";
#else
        JsonPath = Application.persistentDataPath + "/Json";
#endif
       
    }

    public void SaveJsonFile<T>(T data, string name)
    {
        if (Directory.Exists(JsonPath) == false)
        {
            Directory.CreateDirectory(JsonPath);
        }

        string path = Path.Combine(JsonPath, name) + JsonSuffix;
        if (File.Exists(path))
            File.Delete(path);
        FileStream fs = new FileStream(path, FileMode.Create);
        try
        {
            string Json = JsonUtility.ToJson(data,true);
            byte[] datas = Encoding.UTF8.GetBytes(Json);
            fs.Write(datas, 0, datas.Length);
            fs.Close();
        }
        catch
        {
            Debug.Log($"FileManager : Failed To Save Json ({name})");
            fs.Close();
        }
    }

    public string LoadJsonFile(string name)
    {
        if (Directory.Exists(JsonPath) == false)
        {
            Directory.CreateDirectory(JsonPath);
        }
        string path = Path.Combine(JsonPath, name) + JsonSuffix;
        if (File.Exists(path) == false)
            return null;

        FileStream fs = new FileStream(path, FileMode.Open);
        try
        {
            byte[] datas = new byte[fs.Length];
            fs.Read(datas, 0, datas.Length);
            fs.Close();
            string JsonData = Encoding.UTF8.GetString(datas);
            return JsonData;
        }
        catch
        {
            Debug.Log($"FileManager : Failed To Load Json ({name})");
            fs.Close();
            return null;
        }
    }

}
