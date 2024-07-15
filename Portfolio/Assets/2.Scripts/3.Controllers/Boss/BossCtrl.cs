using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Define;
using UnityEngine.SocialPlatforms;

public class BossCtrl : FSM<BossCtrl>
{
    public SODropTable _dropTable;
    public MonsterStat _stat;
    Animator _ani;
    Rigidbody _rb;
    SkinnedMeshRenderer _mesh;
    NavMeshAgent _agent;

    //[SerializeField] Transform _bodyTransform;
    BoxCollider _bodyCollider;
    AudioSource _source;


    [SerializeField] Transform headTR;
    [SerializeField] Boss_Flame _flameEffect;

    [SerializeField] float _rSpeed = 10;
    BossState _nowState = BossState.FlameAttack;
    public eMonster mType = eMonster.Boss;

    [HideInInspector] public Vector3 _offSet = Vector3.zero;
    [HideInInspector] public Vector3 _defPos = Vector3.zero;
    [HideInInspector] public Transform target = null;
    [HideInInspector] public float lastCallTime;
    [HideInInspector] public float delayTime;
    [HideInInspector] public float cntTime;
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isAttack;

    float hitDamage;

    public float HandDamage { get; set; }

    [SerializeField]
    int[] _patternWeight;

    bool isImotal = false;
    
    Coroutine DamageCoroutine = null;
    Coroutine RegenerateCoroutine = null;
    public NavMeshAgent Agent { get { if(_agent == null) _agent = GetComponent<NavMeshAgent>(); return _agent; } }
    public BossState State
    {
        get { return _nowState; }
        set
        {
            _nowState = value;
            ChangeAnim(_nowState);
        }
    }

    public PlayerCtrl player { get { if(PlayerCtrl._inst != null) { return PlayerCtrl._inst; } return null; } }

    void Start()
    {
        InitComponent();
        InitState(this, BossStateInitial._inst);
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
        _bodyCollider = GetComponent<BoxCollider>();
        _source = gameObject.GetOrAddComponent<AudioSource>();
        _ani = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _stat = GetComponent<MonsterStat>();
        
    }

    public void Init()
    {
        _stat.HP = _stat.MaxHP;
        delayTime = 2.0f;
        isDead = false;
        isAttack = false;
        SetCollider(true);
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

    //보스가 필드 외 지역 으로 갔을때 전환 용
    public void IsOutField()
    {
        ChangeState(BossStateReturnHome._inst);
    }

    public void SetTarget(Transform targets)
    {
        if (player != null && player.State != PlayerState.Die)
            target = targets;
        else
            target = null;

    }

    void FreezeRotation()
    {
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    void SetCollider(bool isOn)
    {
        isImotal = !isOn;
        _bodyCollider.enabled = isOn;
    }

    public void ChangeColor(Color color)
    {
        _mesh.material.color = color;
    }

    public void ChangeLayer(eLayer layer)
    {
        int i = (int)layer;
        if(gameObject.layer != i)
            gameObject.layer = i;
    }

    public bool IsCloseTarget(Vector3 pos, float range)
    {
        float dist = Vector3.SqrMagnitude(transform.position - pos);
        if (dist < range * range)
            return true;
        return false;
    }


    public BossPattern PickPattern()
    {
        BossPattern pattern = BossPattern.Basic;

        int sum = 0;
        int i = 0;
        for(; i < _patternWeight.Length; i++)
        {
            sum += _patternWeight[i];
        }
        int randValue = Random.Range(0, sum);

        for(i = 0; i < _patternWeight.Length; i++)
        {
            if (_patternWeight[i] > randValue)
            {
                pattern = (BossPattern)i;
                break;
            }
            else
                randValue -= _patternWeight[i];
        }

        return pattern;
    }

    public void ChangeAnim(BossState type)
    {
        switch (type)
        {
            case BossState.Die:
                _ani.CrossFade("Die", 0.1f);
                break;
            case BossState.Sleep:
                _ani.CrossFade("Sleep", 0.1f);
                break;
            case BossState.Idle:
                _ani.CrossFade("Idle", 0.1f);
                break;
            case BossState.Scream:
                _ani.CrossFade("Scream", 0.1f, -1, 0);
                break;
            case BossState.Trace:
            case BossState.Return:
                _ani.CrossFade("Walk", 0.1f);
                break;
            case BossState.Attack:
                _ani.CrossFade("Attack", 0.1f, -1, 0);
                break;
            case BossState.HandAttack:
                isImotal = true;
                _ani.CrossFade("Hand", 0.1f, -1, 0);
                break;
            case BossState.FlameAttack:
                _ani.CrossFade("Flame", 0.1f, -1, 0);
                break;
        }
    }

    public void MoveFunc(Vector3 pos)
    {
        Vector3 dir = pos - transform.position;
        _agent.SetDestination(pos);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), _rSpeed * Time.deltaTime);
        transform.Rotate(dir * 20.0f * Time.deltaTime);
    }

    public void TurnTowardPlayer()
    {
        if(player != null)
        {
            Vector3 dir = player.transform.position - transform.position;            
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, _rSpeed * Time.deltaTime);
            transform.Rotate(dir * 20.0f * Time.deltaTime);
        }
    }

    //플레이어가 보스 영역에 들어왔을 경우
    public void RecognizePlayer(Transform tr)
    {
        if(State == BossState.Sleep || State == BossState.Idle)
        {
            SetClip(eSoundList.Boss_Growl);
            State = BossState.Scream;
            SetTarget(tr);
        }
    }

    public void EndScreamEvent()
    {
        ChangeState(BossStateTrace._inst);
    }

    public void AttackEvent()
    {
        if(target == null || player.State == PlayerState.Die)
        {
            if (target != null)
                target = null;

            return;
        }

        BossPattern nowPattern = PickPattern();
        _agent.avoidancePriority = 51;
        isAttack = true;

        switch (nowPattern)
        {
            case BossPattern.Basic:
                State = BossState.Attack;
                SetClip(eSoundList.Boss_Bite);
                break;
            case BossPattern.Hand:
                State = BossState.HandAttack;
                break;
            case BossPattern.Flame:
                State = BossState.FlameAttack;
                break;
        }
    }

    // 기본 공격
    public void OnAttackEvent()
    {

        if(target != null && player.State != PlayerState.Die)
        {

            Vector3 dir = target.position - transform.position;
            dir = dir.normalized;

            Ray ray = new Ray(transform.position + Vector3.up, dir * _stat.AttackRange);

            if (Physics.Raycast(ray, _stat.AttackRange, (1 << (int)eLayer.Player)))
            {
                //Sound 삽입

                player.OnDamage(_stat);
            }
        }
    }


    //이동 공격
    public void OnHandAttackEvent()
    {
        SetClip(eSoundList.Boss_Hand);
        if (target != null && player.State != PlayerState.Die)
        {
            HandDamage = _stat.Damage;
            _bodyCollider.center = Vector3.forward * 8;
        }
    }

    //화염 발사 공격
    public void OnFlameAttackEvent()
    {
        SetClip(eSoundList.Boss_Flame, true);
        _flameEffect.OnEffect(headTR,_stat.Damage);
    }

    public void OffHandAttackEvent()
    {
        _bodyCollider.center = Vector3.zero;
        isImotal = false;
        HandDamage = 0;

        Invoke("OffAttackEvent", 0.25f);
    }

    public void OffFlameEvent()
    {
        StopAudio();
        _flameEffect.OffEffect();
        Invoke("OffAttackEvent", 0.25f);
    }

    public void OffAttackEvent()
    {
        
        _agent.avoidancePriority = 50;
        cntTime = 0;
        isAttack = false;
        
    }

    public void OnDamage(BaseStat stat)
    {
        if (isDead || isImotal)
            return;

        isDead = _stat.GetHit(stat);
        if (DamageCoroutine != null)
        {
            StopCoroutine(DamageCoroutine);
        }
        DamageCoroutine = StartCoroutine(OnDamageEvent());
    }

    public bool OnDamage(float damage)
    {
        if (isDead)
            return true;

        if (isImotal)
            return false;

        isDead = _stat.GetHit(damage);
        if (DamageCoroutine != null)
        {
            StopCoroutine(DamageCoroutine);
        }
        DamageCoroutine = StartCoroutine(OnDamageEvent());

        return isDead;
    }

    IEnumerator OnDamageEvent()
    {
        SetClip(eSoundList.GetHit);
        FloatText.Create("FloatText", true, transform.position + Vector3.up, (int)_stat.AttackedDamage);

        if (isDead)
        {
            //사운드 삽입
            _flameEffect.OffEffect();
            SetCollider(false);
            _agent.SetDestination(transform.position);
            _stat.DeadFunc(player._stat);
            ChangeColor(Color.gray);
            ChangeState(BossStateDie._inst);
            yield break;
        }
        ChangeColor(Color.red);
        isImotal = true;
        yield return new WaitForSeconds(0.3f);
        ChangeColor(Color.white);
        isImotal = false;
    }

    public void OnRegenerate()
    {
        if (RegenerateCoroutine != null)
            StopCoroutine(RegenerateCoroutine);
        RegenerateCoroutine = StartCoroutine(RegenerateHP());
    }

    public void OffRegenerate()
    {
        if (RegenerateCoroutine != null)
            StopCoroutine(RegenerateCoroutine);
    }

    IEnumerator RegenerateHP()
    {
        float hpRate = _stat.MaxHP * 0.15f;
        while(_stat.HP <= _stat.MaxHP)
        {
            _stat.HP += hpRate;
            yield return new WaitForSeconds(1);
        }

        _stat.HP = _stat.MaxHP;
    }

    public void OnDeadEvent()
    {        
        ChangeColor(Color.white);
        ChangeState(BossStateDisable._inst);
        _dropTable.ItemDrop(transform, _stat.Gold);
        _dropTable.ItemDrop(transform);

        //GameManager
        GameManagerEX._inst.BossClear();
        GameManagerEX._inst.KillCount(mType);
    }

    public void PlayerOut()
    {
        SpawnManager._inst.MonsterDespawn(gameObject);
    }

    public void OnRessurect()
    {
        SetCollider(true);
        ChangeLayer(eLayer.Monster);
        ChangeState(BossStateInitial._inst);
    }

    void SetClip(eSoundList sList, bool isLoop = false)
    {
        AudioClip clip = SoundManager._inst.GetOrAddAudioClip(sList);

        if (clip != null)
        {
            if (isLoop)
            {
                _source.clip = clip;
                _source.loop = true;
                _source.Play();
            }
            else
            {
                _source.loop = false;
                _source.PlayOneShot(clip);
            }
        }

    }

    void StopAudio()
    {
        if (_source.isPlaying)
            _source.Stop();
        _source.clip = null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon") || other.CompareTag("Cry") || other.CompareTag("Slash"))
        {
            if (other.CompareTag("Weapon"))
            {
                cntTime = 0;
                hitDamage = other.transform.GetComponent<WeaponCtrl>().Damage;
            }
            else if (other.CompareTag("Cry"))
            {
                hitDamage = other.transform.GetComponent<SkillCryCtrl>().Damage;
            }
            else
            {
                hitDamage = other.transform.GetComponent<SkillSlashCtrl>().Damage;
            }

            if (hitDamage > 0)
                OnDamage(hitDamage);
            else
                return;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            cntTime += Time.deltaTime;
            if (cntTime > 0.5f)
            {
                if (hitDamage > 0)
                    OnDamage(hitDamage);
                cntTime = 0;
            }
        }
    }
}
