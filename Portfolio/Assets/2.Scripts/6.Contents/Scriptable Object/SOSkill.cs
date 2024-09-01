using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

[CreateAssetMenu(fileName = "Skill",menuName = "Scriptable/Skill")]
public class SOSkill : ScriptableObject
{
    //스킬 타입
    public eSkill type;
    //스킬명
    public string skillName;
    //입력 키
    public KeyCode key;
    //사용 MP
    public float useMp;
    //쿨타임
    public float cool;
    //지속 시간
    public float duration;
    //효과 배율
    public float effectValue;
    //효과 적용 스탯
    public List<STAT> sList = new List<STAT>();

    //아이콘
    public Sprite icon;
    //스킬 한국어
    public string krName;
    //스킬 설명
    [Multiline]
    public string description;
    //효과 배율 문자열 변환
    [Multiline]
    public string effect;

    //해금 레벨
    public int RequiredLevel;
}
