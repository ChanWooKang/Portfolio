using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Flame : MonoBehaviour
{
    [SerializeField] ParticleSystem _flame;
    [SerializeField] BoxCollider _collider;

    public void OnEffect()
    {
        if (_flame.isPlaying)
            _flame.Stop(true);

        _flame.Play(true);
        _collider.enabled = true;
    }

    public void OffEffect()
    {
        if (_flame.isPlaying)
            _flame.Stop(true);
        _collider.enabled = false;
    }
}
