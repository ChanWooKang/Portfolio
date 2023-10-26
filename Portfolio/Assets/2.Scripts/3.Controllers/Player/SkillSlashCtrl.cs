using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSlashCtrl : MonoBehaviour
{
    float _offSetPosY;
    Rigidbody _rb;
    BoxCollider _colider;
    ParticleSystem _ps;
    public float Damage;

    Vector3 direction;
    [SerializeField] float power;
    bool isShoot = false;

    Coroutine SlashCoroutine = null;

    
    void FixedUpdate()
    {
        if (isShoot && direction != Vector3.zero)
        {
            _rb.velocity = Vector3.zero;            
            _rb.AddForce(transform.forward * power, ForceMode.Impulse);           
            isShoot = false;
        }
    }

    void Init()
    {
        _offSetPosY = 0.65f;
        _rb = GetComponent<Rigidbody>();
        _colider = GetComponent<BoxCollider>();
        _ps = GetComponentInChildren<ParticleSystem>();
        Damage = 0;
        SetEnable(false);
    }

    void SetEnable(bool check)
    {
        _colider.enabled = check;
    }

    void SetPosition(Transform pos)
    {
        Vector3 position = pos.position;
        position.y += _offSetPosY;
        transform.position = position + pos.forward;
        transform.rotation = Quaternion.LookRotation(pos.forward);
    }

    public void SlashEvent(SOSkill skill, Transform player,Vector3 destination)
    {
        Init();
        SetPosition(player);
        direction = destination;
        direction.y = 0;
        Damage = skill.effectValue * PlayerCtrl._inst._stat.Damage;
        if(SlashCoroutine != null)
            StopCoroutine(OnSlashEvent());
        SlashCoroutine = StartCoroutine(OnSlashEvent());
    }

    IEnumerator OnSlashEvent()
    {
        
        isShoot = true;
        yield return new WaitForSeconds(0.1f);
        _ps.Play();
        SetEnable(true);
        yield return new WaitForSeconds(1.8f);
        SetEnable(false);             
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Block")|| other.CompareTag("Ground"))
        {
           
            if (SlashCoroutine != null)
            {
                StopCoroutine(OnSlashEvent());
                SetEnable(false);
                PlayerCtrl._inst.CancelSlash(this.gameObject);
            }
        }
    }

}
