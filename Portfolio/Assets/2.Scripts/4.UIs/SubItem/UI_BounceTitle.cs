using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BounceTitle : MonoBehaviour
{
    float cntTime = 0;

    [SerializeField]
    float updateTime;
    [SerializeField]
    float power;
    [SerializeField]
    float min = 0.5f;
    [SerializeField]
    float max = 1.2f;


    void Update()
    {
        if (cntTime <= updateTime)
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * min, power * Time.deltaTime);
        else if (cntTime <= updateTime * 2)
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * max, power * Time.deltaTime);
        else
            cntTime = 0;

        cntTime += Time.deltaTime;
    }

}
