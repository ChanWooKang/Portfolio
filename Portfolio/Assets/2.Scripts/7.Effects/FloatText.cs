using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class FloatText : MonoBehaviour
{
    public const float _lifeTime = 1.5f;
    const string _objectName = "FloatText";

    public string Text = string.Empty;
    public float FloatSpeed = 8f;
    Text textComponent;

    void Start()
    {
        
    }

    public static GameObject Create(
        string objName,
        bool byPlayer,
        Vector3 pos,
        object value,
        float lifetime = 0f,
        Transform parent = null)
    {
        GameObject go = PoolingManager._pool.InstantiateAPS(_objectName, pos, Quaternion.identity, Vector3.one * 0.8f, parent);
        go.name = objName;
        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        go.GetComponent<FloatText>().Configure(byPlayer,value, _lifeTime);
        if (parent != null)
            go.transform.SetParent(parent);
        return go;
    }

    public void Configure(bool byPlayer, object value, float lifeTime = 0)
    {
        textComponent = GetComponentInChildren<Text>();
        Text = $"{value}";
        if (byPlayer)
        {
            textComponent.color = Color.red;
        }
        else
        {
            textComponent.color = Color.blue;
        }
        StartCoroutine(Floatting());

        if (lifeTime > 0f)
        {
            Invoke("Despawn", lifeTime);
        }
    }

    IEnumerator Floatting()
    {
        while (true)
        {
            textComponent.text = Text;
            transform.rotation = Camera.main.transform.rotation;
            transform.Translate(new Vector3(0, FloatSpeed * Time.deltaTime, 0));
            yield return null;
        }
    }

    void Despawn()
    {
        StopCoroutine(Floatting());
        gameObject.DestroyAPS();
    }
}
