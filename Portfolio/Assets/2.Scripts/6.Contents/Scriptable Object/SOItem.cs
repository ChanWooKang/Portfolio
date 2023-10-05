using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable/Item")]
public class SOItem : ScriptableObject
{
    public string Name;
    public eItem iType;
    public eEquipment eType;
    public int maxStack;
    public List<STAT> sList = new List<STAT>();
    //UIs
    public Sprite icon;
    public string krName;
    public string description;
    public int price;
}
