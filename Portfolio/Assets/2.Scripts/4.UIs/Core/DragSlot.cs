using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : UI_Base
{
    static DragSlot _uniqueInstance;
    public static DragSlot _inst { get { return _uniqueInstance; } }
    public bool isFormInven = true;
    public UI_Slot Slot_Inven;
    public UI_EquipSlot Slot_Equip;
    //public Äü½½·Ô 1°³

    public RectTransform rect;
    CanvasGroup groupCanvas;
    Image ImageItem;

    void Start()
    {
        Init(); 
    }

    public override void Init()
    {
        _uniqueInstance = this;
        rect = GetComponent<RectTransform>();
        ImageItem = GetComponent<Image>();
        groupCanvas = GetComponent<CanvasGroup>();
    }

    public void DragSetImage(Image icon)
    {
        ImageItem.sprite = icon.sprite;
        SetAlpha(1);
    }

    public void SetAlpha(float alpha)
    {
        Color color = ImageItem.color;
        color.a = alpha;
        ImageItem.color = color;
    }

    public void SetCanvas(bool isRaycast)
    {
        if (isRaycast == false)
        {
            groupCanvas.alpha = 0.6f;
        }
        else
        {
            groupCanvas.alpha = 1;
        }
        groupCanvas.blocksRaycasts = isRaycast;
    }
}
