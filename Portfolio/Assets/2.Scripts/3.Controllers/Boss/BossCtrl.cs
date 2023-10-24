using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Define;

public class BossCtrl : FSM<BossCtrl>
{
    public SODropTable _dropTable;
    public MonsterStat _stat = new MonsterStat();
    Animator _ani;
    Rigidbody _rb;
    Collider[] _colliders;
    Renderer[] _meshs;
    NavMeshAgent _agent;

    BossState _nowState;
    public eMonster mType = eMonster.Boss;

    [HideInInspector] public Vector3 _offSet = Vector3.zero;
    [HideInInspector] public Vector3 _defPos = Vector3.zero;
    [HideInInspector] public Transform target = null;
    //[HideInInspector] public Vector3 targetPos;
    [HideInInspector] public float lastCallTime;
    [HideInInspector] public float delayTime;
    [HideInInspector] public float cntTime;
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isAttack;

    [SerializeField]
    int[] _patternWeight;
    [SerializeField]
    int _totalRate;

    public NavMeshAgent Agent { get { if(_agent == null) _agent = GetComponent<NavMeshAgent>(); return _agent; } }
    public BossState State
    {
        get { return _nowState; }
        set
        {
            _nowState = value;
        }
    }

    public PlayerCtrl player { get { if(PlayerCtrl._inst != null) { return PlayerCtrl._inst; } return null; } }

    void Start()
    {

        InitState(this, BossStateIdle._inst);
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
        _ani = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _colliders = GetComponentsInChildren<Collider>();
        _meshs = GetComponentsInChildren<Renderer>();
        _agent.updateRotation = false;
        _stat = new MonsterStat();
    }

    public void Init()
    {
        //targetPos = Vector3.zero;
        _stat.HP = _stat.MaxHP;
        isDead = false;
        isAttack = false;

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

    void ChangeColor(Color color)
    {
        if(_meshs.Length> 0)
        {
            foreach (Renderer mesh in _meshs)
                mesh.material.color = color;
        }
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
            case BossState.Idle:
                _ani.CrossFade("Idle", 0.1f);
                break;
            case BossState.Scream:
                _ani.CrossFade("Scream", 0.1f, -1, 0);
                break;
        }
    }

    public void MoveFunc(Vector3 pos)
    {
        Vector3 dir = pos - transform.position;
        _agent.SetDestination(pos);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
    }

    public void TurnTowardPlayer()
    {
        if(player != null)
        {
            Vector3 dir = player.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    //플레이어가 보스 영역에 들어왔을 경우
    public void RecognizePlayer()
    {
        //아이들 상태에서만 비명 지르고 추격
        if(State == BossState.Idle)
        {
            State = BossState.Scream;
        }
    }

    public void EndScreamEvent()
    {
        ChangeState(BossStateTrace._inst);
    }

    public void AttackEvent()
    {
        BossPattern nowPattern = PickPattern();
    }
}
