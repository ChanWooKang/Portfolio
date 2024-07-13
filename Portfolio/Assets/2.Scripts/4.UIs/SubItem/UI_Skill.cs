using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Define;



public class UI_Skill : UI_Base, IPointerEnterHandler, IPointerExitHandler
{
    enum GameObjects
    {
        Skill_Img,
        Cool_Img
    }

    PlayerCtrl player;
    
    public SOSkill _skill;
    Image Skill_Img;
    Image Cool_Img;
    bool isAble = false;

    void Awake()
    {
        Bind<GameObject>(typeof(GameObjects));
        Skill_Img = GetObject((int)GameObjects.Skill_Img).GetComponent<Image>();
        Cool_Img = GetObject((int)GameObjects.Cool_Img).GetComponent<Image>();
    }
    void Start()
    {
        Init();
    }

    public override void Init()
    {        
        Managers._input.KeyAction -= OnKeyBoardEvent;
        Managers._input.KeyAction += OnKeyBoardEvent;
        ClearCool();
        
    }

    public void TryCheckActive()
    {
        if(player == null)
        {
            player = PlayerCtrl._inst;
        }

        if (player._stat.Level >= _skill.RequiredLevel)
        {
            if (isAble == false)
            {
                isAble = true;
                Skill_Img.gameObject.SetActive(true);
                ClearCool();
            }
        }
        else
        {
            isAble = false;
            Skill_Img.gameObject.SetActive(false);
            Cool_Img.gameObject.SetActive(false);
        }
    }

    void OnKeyBoardEvent()
    {
        if (player.Bools[PlayerBools.Dead] || GameManagerEX._inst.StopMove)
            return;

        if (!isAble)
            return;

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

        if (_skill.type == eSkill.Spin && PlayerCtrl._inst.State == PlayerState.Attack)
            return;

        switch (_skill.type) 
        {
            case eSkill.Dodge:
                {
                    if (player.Bools[PlayerBools.ActSkill])
                        return;
                }
                break;
            default:
                if (player.Bools[PlayerBools.ActDodge])
                    return;

                if (player.UseMP(_skill.useMp) == false)
                    return;
                break;
        }

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        
        UI_SkillInfo._inst.SetInformation(_skill, transform.position);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UI_SkillInfo._inst.OffInforMation();
    }
}
