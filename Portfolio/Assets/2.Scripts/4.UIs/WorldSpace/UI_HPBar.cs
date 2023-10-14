using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HPBar : UI_Base
{
    enum Images
    {
        HP
    }

    MonsterCtrl mc;
    Image HP;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<Image>(typeof(Images));
        HP = GetImage((int)Images.HP);
        mc = GetComponentInParent<MonsterCtrl>();
        StartCoroutine(Setting());
    }

    IEnumerator Setting()
    {
        while (mc.isDead == false)
        {
            Transform parent = transform.parent;
            transform.position = parent.position + Vector3.up * (mc.Agent.height);
            transform.rotation = Camera.main.transform.rotation;

            float ratio = mc._stat.HP / mc._stat.MaxHP;
            HP.fillAmount = ratio;
            yield return null;
        }        
    }

    public void SetHPBar(float ratio)
    {
        HP.fillAmount = ratio;
    }

    public void CoroutineStart()
    {
        if (mc == null)
            return;

        if(mc.isDead == false)
        {
            StopCoroutine(Setting());
            StartCoroutine(Setting());
        }
    }
}
