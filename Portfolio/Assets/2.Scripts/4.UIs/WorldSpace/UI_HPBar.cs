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

    public override void Init()
    {
        Bind<Image>(typeof(Images));
        
        HP = GetImage((int)Images.HP);                
    }

    IEnumerator Setting()
    {
        while(mc.isDead == false)
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

    public void CoroutineStart(MonsterCtrl _mc)
    {
        Init();
        mc = _mc;
        SetHPBar(1);
        StopCoroutine(Setting());
        StartCoroutine(Setting());
    }

    public void CoroutineStop()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }
}
