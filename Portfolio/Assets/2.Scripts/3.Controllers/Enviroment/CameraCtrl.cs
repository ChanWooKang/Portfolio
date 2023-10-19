using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class CameraCtrl : MonoBehaviour
{
    [SerializeField] CameraMode _mode;
    [SerializeField] Vector3 _offSet;
    [SerializeField] Transform _target;

    void LateUpdate()
    {
        Vector3 dir = (_target.position - transform.position).normalized;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, dir, Mathf.Infinity, 1 << (int)eLayer.TransBlock);

        for (int i = 0; i < hits.Length; i++)
        {
            TransparentObject[] obj = hits[i].transform.GetComponentsInChildren<TransparentObject>();

            for (int j = 0; j < obj.Length; j++)
            {
                obj[j]?.BecomeTransparent();
            }
        }


        if(_mode == CameraMode.Quater)
        {
            transform.position = _target.position + _offSet;
            transform.LookAt(_target);
        }
    }

    public void SetQuaterView(Vector3 offSet)
    {
        _mode = CameraMode.Quater;
        _offSet = offSet;
    }
}
