using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentObject : MonoBehaviour
{
    public bool IsTransParent { get; private set; } = false;

    MeshRenderer[] _renders;
    WaitForSeconds delay = new WaitForSeconds(0.001f);
    WaitForSeconds resetDelay = new WaitForSeconds(0.005f);
    const float Threshold_Alpha = 0.25f;
    const float Threshold_Time = 0.5f;

    bool isReseting = false;
    float timer = 0f;
    Coroutine timeCheckCoroutine;
    Coroutine resetCoroutine;
    Coroutine becomTransparentCoroutine;

    void Awake()
    {
        _renders = GetComponentsInChildren<MeshRenderer>();
    }

    public void BecomeTransparent()
    {
        if (IsTransParent)
        {
            timer = 0;
            return;
        }

        if (resetCoroutine != null && isReseting)
        {
            isReseting = false;
            IsTransParent = false;
            StopCoroutine(resetCoroutine);
        }

        SetMaterialTransparent();
        IsTransParent = true;
        becomTransparentCoroutine = StartCoroutine(BecomeTransparentCoroutine());

    }

    void SetMaterialRenderingMode(Material material, float mode, int renderQueue)
    {
        material.SetFloat("_Mode", mode);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = renderQueue;
    }

    void SetMaterialTransparent()
    {
        for(int i = 0; i < _renders.Length; i++)
        {
            foreach(Material mat in _renders[i].materials)
            {
                SetMaterialRenderingMode(mat, 3f, 3000);
            }
        }
    }

    void SetMaterialOpaque()
    {
        for (int i = 0; i < _renders.Length; i++)
        {
            foreach (Material mat in _renders[i].materials)
            {
                SetMaterialRenderingMode(mat, 0, -1);
            }
        }
    }

    public void ResetOriginalTransparent()
    {
        SetMaterialOpaque();
        resetCoroutine = StartCoroutine(ResetOriginalTransparentCoroutine());
    }

    IEnumerator BecomeTransparentCoroutine()
    {
        while (true)
        {
            bool isComplete = true;

            for(int i = 0; i < _renders.Length; i++)
            {
                if (_renders[i].material.color.a > Threshold_Alpha)
                    isComplete = false;

                Color color = _renders[i].material.color;
                color.a -= Time.deltaTime;
                _renders[i].material.color = color;
            }

            if (isComplete)
            {
                CheckTimer();
                break;
            }

            yield return delay;
        }
    }

    IEnumerator ResetOriginalTransparentCoroutine()
    {
        IsTransParent = false;

        while (true)
        {
            bool isComplete = true;
            for(int i = 0; i < _renders.Length; i++)
            {
                if (_renders[i].material.color.a < 1f)
                    isComplete = false;

                Color color = _renders[i].material.color;
                color.a += Time.deltaTime;
                _renders[i].material.color = color;
            }

            if (isComplete)
            {
                isReseting = false;
                break;
            }

            yield return resetDelay;
        }
    }

    public void CheckTimer()
    {
        if (timeCheckCoroutine != null)
            StopCoroutine(timeCheckCoroutine);
        timeCheckCoroutine = StartCoroutine(CheckTimerCoroutine());
    }

    IEnumerator CheckTimerCoroutine()
    {
        timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if(timer > Threshold_Time)
            {
                isReseting = true;
                ResetOriginalTransparent();
                break;
            }

            yield return null;
        }
    }
}
