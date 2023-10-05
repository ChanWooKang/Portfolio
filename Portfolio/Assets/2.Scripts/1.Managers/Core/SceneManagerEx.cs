using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Define;


public class SceneManagerEx
{ 
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }
    public Dictionary<eCursor, Texture2D> dict_Cursor;

    public void Init()
    {
        dict_Cursor = new Dictionary<eCursor, Texture2D>();
    }

    public IEnumerator LoadCoroutine(eScene scene)
    {
        Managers.Clear();
        string sceneName = Util.ConvertEnum(scene);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            yield return null;
        }
    }

    public void Clear()
    {
        CurrentScene.Clear();
        dict_Cursor.Clear();
    }

    public void AddCursor()
    {
        if (CurrentScene.list_Cursor.Count > 0)
        {
            for (int i = 0; i < CurrentScene.list_Cursor.Count; i++)
            {
                CursorUnit unit = CurrentScene.list_Cursor[i];

                if(dict_Cursor.ContainsKey(unit.type) == false)
                {
                    dict_Cursor.Add(unit.type, unit.tex);
                }
                else
                {
                    dict_Cursor[unit.type] = unit.tex;
                }
            }
        }
    }
    
    
}
