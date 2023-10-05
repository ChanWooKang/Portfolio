using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

[CreateAssetMenu(fileName = "Skill",menuName = "Scriptable/Skill")]
public class SOSkill : ScriptableObject
{
    public eSkill type;
    public string skillName;
    public KeyCode key;
    public float cool;
    public float effectValue;
    public List<STAT> sList = new List<STAT>();

    //UIs
    public Sprite icon;
    public string krName;
}
