using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    #region [ Component ]
    Rigidbody _rigid;
    Collider _collider;
    ParticleSystem _particle;
    #endregion [ Component]

    float _offSetPosY;
    public float shootPower;
    public float Damage;
    bool isShoot = false;
    Vector3 direction;

    Coroutine ShootCoroutine = null;

    void FixedUpdate()
    {
        if (isShoot && direction != Vector3.zero)
        {
            _rigid.velocity = Vector3.zero;
            _rigid.AddForce(transform.forward * shootPower, ForceMode.Impulse);
            isShoot = false;
        }
    }

    void Init()
    {
        _offSetPosY = 0.65f;
        _rigid = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _particle = GetComponentInChildren<ParticleSystem>();
        Damage = 0;

    }

    void SetEnable(bool isEnable)
    {
        _collider.enabled = isEnable;
    }

    void SetPosition(Transform pos)
    {
        Vector3 position = pos.position;
        position.y += _offSetPosY;
        transform.position = position + pos.forward;
        transform.rotation = Quaternion.LookRotation(pos.forward);
    }

    public void ShootEvent(Transform caster, Vector3 destination, float damage)
    {
        Init();
        SetPosition(caster);
        direction = destination;
        direction.y = 0;
        Damage = damage;
        if (ShootCoroutine != null)
            StopCoroutine(OnShootEvent());
        ShootCoroutine = StartCoroutine(OnShootEvent());
    }

    IEnumerator OnShootEvent()
    {
        isShoot = true;
        _particle.Play();
        SetEnable(true);
        yield return new WaitForSeconds(1.8f);
        _particle.Stop();
        SetEnable(false);
        gameObject.DestroyAPS();
    }

    public void Disable()
    {
        StopCoroutine(OnShootEvent());
        _particle.Stop();
        SetEnable(false);
        gameObject.DestroyAPS();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block") || other.CompareTag("Ground"))
        {
            if(ShootCoroutine != null)
            {
                Disable();
            }
        }
    }
}
