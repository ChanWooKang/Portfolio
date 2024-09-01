using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

[CreateAssetMenu(fileName = "Skill",menuName = "Scriptable/Skill")]
public class SOSkill : ScriptableObject
{
    //��ų Ÿ��
    public eSkill type;
    //��ų��
    public string skillName;
    //�Է� Ű
    public KeyCode key;
    //��� MP
    public float useMp;
    //��Ÿ��
    public float cool;
    //���� �ð�
    public float duration;
    //ȿ�� ����
    public float effectValue;
    //ȿ�� ���� ����
    public List<STAT> sList = new List<STAT>();

    //������
    public Sprite icon;
    //��ų �ѱ���
    public string krName;
    //��ų ����
    [Multiline]
    public string description;
    //ȿ�� ���� ���ڿ� ��ȯ
    [Multiline]
    public string effect;

    //�ر� ����
    public int RequiredLevel;
}
