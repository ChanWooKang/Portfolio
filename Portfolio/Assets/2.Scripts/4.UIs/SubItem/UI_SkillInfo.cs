using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Define;

public class UI_SkillInfo : UI_Base
{
    static UI_SkillInfo _unique;
    public static UI_SkillInfo _inst { get { return _unique; } }
    enum GameObjects
    {
        MainFrame,
        SkillName,
        SkillIcon,
        SkillDesc,
        UseMP_Parent,
        UseMP,
        CoolTime,
        Duration_Parent,
        Duration,
        Effect
    }

    enum TextType
    {
        Name,
        MP,
        Desc,
        CoolTime,
        Duration,
        Effect
    }

    GameObject _main;
    GameObject _usempParent;
    GameObject _durationParent;
    Image _icon;
    Text _name;
    Text _useMp;
    Text _desc;
    Text _coolTime;
    Text _duration;
    Text _effect;

    void Awake()
    {
        _unique = this;
    }

    void Start()
    {
        Init();
        OffInforMation();
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        _main = GetObject((int)GameObjects.MainFrame);
        _usempParent = GetObject((int)GameObjects.UseMP_Parent);
        _durationParent = GetObject((int)GameObjects.Duration_Parent);
        _icon = GetObject((int)GameObjects.SkillIcon).GetComponent<Image>();
        _name = GetObject((int)GameObjects.SkillName).GetComponent<Text>();
        _useMp = GetObject((int)GameObjects.UseMP).GetComponent<Text>();
        _desc = GetObject((int)GameObjects.SkillDesc).GetComponent<Text>();
        _coolTime = GetObject((int)GameObjects.CoolTime).GetComponent<Text>();
        _duration = GetObject((int)GameObjects.Duration).GetComponent<Text>();
        _effect = GetObject((int)GameObjects.Effect).GetComponent<Text>();
    }

    string SetStringFormatByType(TextType type, SOSkill skill)
    {
        string value = null;
        switch (type)
        {
            case TextType.Name:
                value = skill.krName;
                break;
            case TextType.MP:
                {
                    if (skill.useMp > 0)
                    {
                        _usempParent.SetActive(true);
                        value = $"MP {skill.useMp}";
                    }
                    else
                        _usempParent.SetActive(false);
                }
                break;
            case TextType.Desc:
                value = skill.description;
                break;
            case TextType.CoolTime:
                value = $"쿨타임 : {skill.cool}";
                break;
            case TextType.Duration:
                {
                    if(skill.duration > 0)
                    {
                        _durationParent.SetActive(true);
                        value = $"지속시간 : {skill.duration}";
                    }
                    else
                    {
                        _durationParent.SetActive(false);
                    }
                }
                
                break;
            case TextType.Effect:
                value = skill.effect;
                break;
        }
        return value;
    }

    public void SetInformation(SOSkill skill, Vector3 pos)
    {
        _icon.sprite = skill.icon;
        _name.text = SetStringFormatByType(TextType.Name, skill);
        _useMp.text = SetStringFormatByType(TextType.MP, skill);
        _desc.text = SetStringFormatByType(TextType.Desc, skill);
        _coolTime.text = SetStringFormatByType(TextType.CoolTime, skill);
        _duration.text = SetStringFormatByType(TextType.Duration, skill);
        _effect.text = SetStringFormatByType(TextType.Effect, skill);

        transform.position = pos;
        _main.SetActive(true);
    }

    public void OffInforMation()
    {
        _main.SetActive(false);
        transform.position = Vector3.zero;
    }
}
