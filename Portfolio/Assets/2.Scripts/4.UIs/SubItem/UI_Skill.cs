using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_Skill : UI_Base
{
    enum GameObjects
    {
        Cool_Img
    }

    public SOSkill _skill;
   Image Cool_Img;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Cool_Img = GetObject((int)GameObjects.Cool_Img).GetComponent<Image>();
        Managers._input.KeyAction -= OnKeyBoardEvent;
        Managers._input.KeyAction += OnKeyBoardEvent;
        ClearCool();
    }

    void OnKeyBoardEvent()
    {
        if (Input.GetKeyDown(_skill.key))
        {
            OnSkill();
        }
    }

    void ClearCool()
    {
        Cool_Img.fillAmount = 0;
        Cool_Img.gameObject.SetActive(false);
    }

    void SetCool()
    {
        if (Cool_Img.gameObject.activeSelf == false)
            Cool_Img.gameObject.SetActive(true);
        Cool_Img.fillAmount = 1;
    }

    public void OnSkill()
    {
        if (Cool_Img.fillAmount > 0)
            return;

        //스킬 처리
        PlayerCtrl._inst.SkillEvent(_skill.type, _skill);

        //스킬 쿨타임 처리
        StopCoroutine(Skill_Cool());
        StartCoroutine(Skill_Cool());
    }

    IEnumerator Skill_Cool()
    {
        float tick = 1.0f / _skill.cool;
        float t = 0;

        SetCool();

        while (Cool_Img.fillAmount > 0)
        {
            Cool_Img.fillAmount = Mathf.Lerp(1, 0, t);
            t += (Time.deltaTime * tick);

            yield return null;
        }
        ClearCool();
    }
}
