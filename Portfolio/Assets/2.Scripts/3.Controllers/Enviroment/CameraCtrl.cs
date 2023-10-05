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
