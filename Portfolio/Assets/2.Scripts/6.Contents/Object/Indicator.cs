using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{

    public GameObject IndicatorCanvas;
    GameObject _indicatorObj;
    float defaultAngle;
    public bool isQuestType = false;

    Vector2 myVec2 { get { return new Vector2(transform.position.x,transform.position.z); } }
    Vector2 playerVec2 { get { return new Vector2(PlayerCtrl._inst.transform.position.x, PlayerCtrl._inst.transform.position.z); } }
    public void MakeIndicator()
    {
        _indicatorObj = PoolingManager._pool.InstantiateAPS("Indicator", IndicatorCanvas.transform);
        _indicatorObj.transform.localScale = Vector3.one;

        Vector2 dir = new Vector2(Screen.width, Screen.height);
        defaultAngle = Vector2.Angle(Vector2.up, dir);
    }

    public void SetIndicator()
    {
        if (!isOffScreen())
            return;

        float angle = Vector2.Angle(Vector2.up, myVec2 - playerVec2);
        int sign = playerVec2.x > myVec2.x ? -1 : 1;
        angle *= sign;

        Vector3 targetVector = Camera.main.WorldToScreenPoint(transform.position);
        float x = targetVector.x - 0.5f;
        float y = targetVector.z - 0.5f;

        RectTransform indicatorRect = _indicatorObj.GetComponent<RectTransform>();
        if (-defaultAngle <= angle && angle <= defaultAngle)
        {
            float anchorMinMaxY = 0.96f;

            float anchorMinMaxX = x * (anchorMinMaxY - 0.5f) / y + 0.5f;

            if (anchorMinMaxX >= 0.94f) anchorMinMaxX = 0.94f;
            else if (anchorMinMaxX <= 0.06f) anchorMinMaxX = 0.06f;

            indicatorRect.anchorMin = new Vector2(anchorMinMaxX, anchorMinMaxY);
            indicatorRect.anchorMax = new Vector2(anchorMinMaxX, anchorMinMaxY);
        }
        else if (defaultAngle <= angle && angle <= 180 - defaultAngle)
        {
            float anchorMinMaxX = 0.94f;

            float anchorMinMaxY = y * (anchorMinMaxX - 0.5f) / x + 0.5f;

            if (anchorMinMaxY >= 0.96f) anchorMinMaxY = 0.96f;
            else if (anchorMinMaxY <= 0.04f) anchorMinMaxY = 0.04f;

            indicatorRect.anchorMin = new Vector2(anchorMinMaxX, anchorMinMaxY);
            indicatorRect.anchorMax = new Vector2(anchorMinMaxX, anchorMinMaxY);
        }
        else if (-180 + defaultAngle <= angle && angle <= -defaultAngle)
        {
            float anchorMinMaxX = 0.06f;

            float anchorMinMaxY = (y * (anchorMinMaxX - 0.5f) / x) + 0.5f;

            if (anchorMinMaxY >= 0.96f) anchorMinMaxY = 0.96f;
            else if (anchorMinMaxY <= 0.04f) anchorMinMaxY = 0.04f;

            indicatorRect.anchorMin = new Vector2(anchorMinMaxX, anchorMinMaxY);
            indicatorRect.anchorMax = new Vector2(anchorMinMaxX, anchorMinMaxY);
        }
        else if (-180 <= angle && angle <= -180 + defaultAngle || 180 - defaultAngle <= angle && angle <= 180)
        {
            float anchorMinMaxY = 0.04f;

            float anchorMinMaxX = x * (anchorMinMaxY - 0.5f) / y + 0.5f;

            if (anchorMinMaxX >= 0.94f) anchorMinMaxX = 0.94f;
            else if (anchorMinMaxX <= 0.06f) anchorMinMaxX = 0.06f;

            indicatorRect.anchorMin = new Vector2(anchorMinMaxX, anchorMinMaxY);
            indicatorRect.anchorMax = new Vector2(anchorMinMaxX, anchorMinMaxY);
        }
        indicatorRect.anchoredPosition = Vector3.zero;
    }

    public bool isOffScreen()
    {
        Vector2 vec = Camera.main.WorldToViewportPoint(transform.position);
        if (vec.x >= 0 && vec.x <= 1 && vec.y >= 0 && vec.y <= 1)
        {
            _indicatorObj.SetActive(false);
            return false;
        }
        else
        {
            _indicatorObj.SetActive(true);
            return true;
        }
    }
}
