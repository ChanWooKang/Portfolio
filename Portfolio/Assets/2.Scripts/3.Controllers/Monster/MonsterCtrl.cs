using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Define;

public class MonsterCtrl : FSM<MonsterCtrl>
{
    public MonsterStat _stat;
    Animator _anim;
    Rigidbody _rb;
    CapsuleCollider _colider;
    Renderer[] _meshs;
    NavMeshAgent _agent;
    public UI_HPBar _hpBar;

    public eMonster mType = eMonster.Unknown;
    public SODropTable _dropTable;

    [SerializeField, Range(8, 15)] float _rSpeed;

    [HideInInspector] public Vector3 _offSet = Vector3.zero;
    [HideInInspector] public Vector3 _defPos = Vector3.zero;
    [HideInInspector] public Transform target = null;
    public Vector3 targetPos;
    [HideInInspector] public float lastCallTime;
    [HideInInspector] public float delayTime;
    [HideInInspector] public float cntTime;
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isAttack;
    [HideInInspector] public bool isReturnHome;

    MonsterState _nowState = MonsterState.Idle;
    eCombo _nowCombo = eCombo.Hit1;

   

    public NavMeshAgent Agent
    {
        get
        {
            if (_agent == null)
            {
                _agent = gameObject.GetOrAddComponent<NavMeshAgent>();
            }
            return _agent;
        }
    }

    public MonsterState State
    {
        get { return _nowState; }
        set
        {
            _nowState = value;
            ChangeAnim(_nowState);
        }
    }

    public eCombo nowCombo { get { return _nowCombo; } set { _nowCombo = value; } }

    public PlayerCtrl player { get { return PlayerCtrl._inst; } }

    void Start()
    {
        InitComponent();
        _hpBar = Managers._ui.MakeWorldSpace<UI_HPBar>(transform);
        InitState(this, MonsterStateInitial._inst);
        
    }

    void Update()
    {
        FSMUpdate();
    }

    void FixedUpdate()
    {
        FreezeRotation();
    }

    void InitComponent()
    {
        _stat = GetComponent<MonsterStat>();
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _colider = GetComponent<CapsuleCollider>();
        _meshs = GetComponentsInChildren<Renderer>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;        
    }

    public void InitData()
    {
        targetPos = Vector3.zero;
        _stat.HP = _stat.MaxHP;
        lastCallTime = 0;
        delayTime = 2.0f;
        isDead = false;
        isAttack = false;
        isReturnHome = false;
        _hpBar.CoroutineStart();
    }

    public void BaseNavSetting()
    {
        _agent.ResetPath();
        _agent.isStopped = false;
        _agent.updatePosition = true;
        _agent.updateRotation = false;
    }

    public void AttackNavSetting()
    {
        _agent.isStopped = true;
        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _agent.velocity = Vector3.zero;
    }

    public bool IsTooFar(float range = 20.0f)
    {
        Vector3 goalPos = new Vector3(_offSet.x, transform.position.y, _offSet.z);
        float dist = Vector3.SqrMagnitude(goalPos - transform.position);
        if (dist > range * range)
            return true;
        return false;
    }

    public void SetTarget()
    {
        if (player != null && player.State != PlayerState.Die) 
            target = player.transform;
        else
            target = null;
    }

    void FreezeRotation()
    {
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    public void ChangeAnim(MonsterState type)
    {
        switch (type)
        {
            case MonsterState.Die:
                _anim.CrossFade("Die", 0.1f);
                break;
            case MonsterState.Idle:
                _anim.CrossFade("Idle", 0.1f);
                break;
            case MonsterState.Sense:
                _anim.CrossFade("Sense", 0.1f, -1, 0);
                break;
            case MonsterState.Patrol:
                _anim.CrossFade("Patrol", 0.1f);
                break;
            case MonsterState.Trace:
                _anim.CrossFade("Trace", 0.1f);
                break;
            case MonsterState.Attack:
                _stat.Attack(_anim);
                break;
        }
    }

    void ChangeColor(Color color)
    {
        if (_meshs.Length > 0)
        {
            foreach (Renderer mesh in _meshs)
                mesh.material.color = color;
        }
    }

    public void ChangeLayer(eLayer layer)
    {
        int i = (int)layer;

        if (gameObject.layer != i)
            gameObject.layer = i;
    }

    public Vector3 GetRandomPos(float range = 5.0f)
    {
        Vector3 pos = Random.onUnitSphere;
        pos.y = 0;
        float r = Random.Range(1, range);
        pos = _defPos + (pos * r);

        NavMeshPath path = new NavMeshPath();
        if (_agent.CalculatePath(pos, path))
            return pos;
        else
            return GetRandomPos();
    }

    public bool IsCloseTarget(Vector3 pos, float range)
    {
        Vector3 goalPos = new Vector3(pos.x, transform.position.y, pos.z);        
        float dist = Vector3.SqrMagnitude(goalPos - transform.position);
        if (dist < range * range)
            return true;
        return false;
    }

    public void MoveFunc(Vector3 pos)
    {
        Vector3 dir = pos - transform.position;
        _agent.SetDestination(pos);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), _rSpeed * Time.deltaTime);
    }

    public void TurnTowardPlayer()
    {
        if (player != null)
        {
            Vector3 dir = player.transform.position - transform.position;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), _rSpeed * Time.deltaTime);
        }
    }

    public void AttackFunc()
    {
        if (target == null || player.State == PlayerState.Die)
        {
            if (target != null)
                target = null;
            
            return;
        }

        if (player.Bools[PlayerBools.ActDodge])
            return;

        _agent.avoidancePriority = 51;
        State = MonsterState.Attack;
        isAttack = true;
    }

    public void SpeacialAttack(Vector3 targetPos)
    {
        _stat.SpeacialAttack(target.position);
    }

    public void OnAttackEvent(int pattern)
    {
        if (target != null && player.State != PlayerState.Die)
        {
            if(pattern == 2)
            {
                SpeacialAttack(target.position);
                return;
            }

            if (IsCloseTarget(target.position, _stat.AttackRange))
            {                
                player.OnDamage(_stat);
            }
        }
    }

    public void OffAttackEvent()
    {
        _agent.avoidancePriority = 50;
        cntTime = 0;
        isAttack = false;
    }

    public void OnDamage(BaseStat stat)
    {
        if (isDead)
            return;

        isDead = _stat.GetHit(stat);
        StopCoroutine(OnDamageEvent());
        StartCoroutine(OnDamageEvent());
    }

    IEnumerator OnDamageEvent()
    {
        SoundManager._inst.Play(eSoundList.GetHit);
        FloatText.Create("FloatText", true, transform.position, (int)_stat.AttackedDamage);
        if (isDead)
        {
            _colider.enabled = false;
            _agent.SetDestination(transform.position);
            _stat.DeadFunc(player._stat);
            ChangeColor(Color.gray);
            ChangeState(MonsterStateDie._inst);
            yield break;
        }
        ChangeColor(Color.red);
        yield return new WaitForSeconds(0.3f);
        ChangeColor(Color.white);
    }

    public void OnDeadEvent()
    {      
        SpawnManager._inst.MonsterDespawn(gameObject);
        ChangeColor(Color.white);
        ChangeState(MonsterStateDisable._inst);
        _dropTable.ItemDrop(transform, _stat.Gold);
        _dropTable.ItemDrop(transform);

        //GameManager
        GameManagerEX._inst.KillCount(mType);
    }    

    public void OnResurrectEvent()
    {
        if (_colider.enabled == false)
            _colider.enabled = true;
        ChangeLayer(eLayer.Monster);
        ChangeState(MonsterStateInitial._inst);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon") || other.CompareTag("Cry") || other.CompareTag("Slash"))
        {
            float damage = 0;
            if (other.CompareTag("Weapon"))
            {
                damage = other.transform.GetComponent<WeaponCtrl>().Damage;
            }
            else if (other.CompareTag("Cry"))
            {
                damage = other.transform.GetComponent<SkillCryCtrl>().Damage;
            }
            else
            {                
                damage = other.transform.GetComponent<SkillSlashCtrl>().Damage;                
            }
                
            if (damage > 0)
                isDead = _stat.GetHit(damage);
            else
                return;

            StopCoroutine(OnDamageEvent());
            StartCoroutine(OnDamageEvent());
        }
    }
}
