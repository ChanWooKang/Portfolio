using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Flame : MonoBehaviour
{
    [SerializeField] ParticleSystem _flame;
    [SerializeField] BoxCollider _collider;

    public float Damage;
    Transform parentTransform = null;
    bool isOn = false;

    void FixedUpdate()
    {
        if (isOn)
        {
            transform.position = parentTransform.position;
            transform.rotation = parentTransform.rotation;
        }
    }




    public void OnEffect(Transform head,float dmg, float rate = 0.5f)
    {
        parentTransform = head;
        
        isOn = true;

        Damage = dmg * rate;
        if (_flame.isPlaying)
            _flame.Stop(true);

        _flame.Play(true);
        _collider.enabled = true;
    }

    public void OffEffect()
    {
        isOn = false;
        parentTransform = null;

        if (_flame.isPlaying)
            _flame.Stop(true);
        _collider.enabled = false;
    }
}
