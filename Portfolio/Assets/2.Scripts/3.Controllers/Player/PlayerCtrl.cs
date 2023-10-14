using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Define;

public class PlayerCtrl : MonoBehaviour
{
    #region [ Data ]

    static PlayerCtrl _uniqueInstance;
    
    //Components
    public PlayerStat _stat;
    Animator _anim;
    Rigidbody _rb;
    NavMeshAgent _agent;
    Renderer[] _meshs;

    //���̾� ����ũ 
    int _clickMask;
    int _blockMask;

    // �̵� ��ǥ
    Vector3 _destPos;
    // ��ų �Է� �� ���콺 ��ġ ���� ��ǥ ����
    Vector3 _mouseWorldPoint;

    //���콺 Ŭ���� ������Ʈ ����
    GameObject _locktarget;

    //��ȣ�ۿ� ������Ʈ �ν�
    GameObject _nearObj;
    eInteract _nearType;

    PlayerState _state = PlayerState.Idle;
    eSkill _sType = eSkill.Unknown;

    Dictionary<PlayerBools, bool> dict_bool;

    #endregion [ Data ]

    #region [ Property ]

    public PlayerState State
    {
        get { return _state; }
        set
        {
            _state = value;
            if (_anim == null)
                _anim = GetComponent<Animator>();
            switch (_state)
            {
                case PlayerState.Die:
                    _anim.CrossFade("Die", 0.1f);
                    break;
                case PlayerState.Idle:
                    _anim.CrossFade("Idle", 0.1f);
                    break;
                case PlayerState.Move:
                    _anim.CrossFade("Move", 0.1f);
                    break;
                case PlayerState.Attack:
                    _anim.CrossFade("Attack", 0.1f, -1, 0);
                    break;
                case PlayerState.Skill:
                    {
                        switch (_sType)
                        {
                            case eSkill.Dodge:
                                _anim.CrossFade("Dodge", 0.1f, -1, 0);
                                break;
                            case eSkill.Slash:
                                _anim.CrossFade("Slash", 0.1f, -1, 0);
                                break;
                            case eSkill.Spin:
                                _anim.CrossFade("Spin", 0.1f);
                                break;
                        }
                    }
                    break;
            }
        }
    }
    public static PlayerCtrl _inst { get { return _uniqueInstance; } }
    public GameObject LockTarget { get { return _locktarget; } set { _locktarget = value; } }
    public Dictionary<PlayerBools,bool> Bools { get { return dict_bool; } }

    #endregion [ Property ]

    void Awake()
    {
        InitComponent();
    }

    void Start()
    {
        InitData();
        Managers._input.KeyAction -= OnKeyBoardEvent;
        Managers._input.KeyAction += OnKeyBoardEvent;
        Managers._input.RightMouseAction -= OnMouseEvent;
        Managers._input.RightMouseAction += OnMouseEvent;
    }

    void Update()
    {
        switch (_state)
        {
            case PlayerState.Move:
                UpdateMove();
                break;
            case PlayerState.Attack:
                UpdateAttack();
                break;
            case PlayerState.Skill:
                {
                    switch (_sType)
                    {
                        case eSkill.Dodge:
                        case eSkill.Slash:
                            UpdateSkill();
                            break;
                    }
                }
                break;
        }
    }

    void FixedUpdate()
    {
        FreezeRotate();
    }


    #region [ Sub Function ]
    void InitComponent()
    {
        _uniqueInstance = this;
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _meshs = GetComponentsInChildren<Renderer>();
        _stat = new PlayerStat();
    }

    void InitData()
    {
        _stat.LoadPlayer();
        _clickMask = (1 << (int)eLayer.Ground) | (1 << (int)eLayer.Monster);
        _blockMask = (1 << (int)eLayer.Block);

        _destPos = transform.position;
        _mouseWorldPoint = transform.position;

        _locktarget = null;
        _nearObj = null;
        _nearType = eInteract.Unknown;

        dict_bool = new Dictionary<PlayerBools, bool>();
        for (int i = 0; i < (int)PlayerBools.Max_Cnt; i++)
            dict_bool.Add((PlayerBools)i, false);
    }

    void FreezeRotate()
    {
        if (_sType == eSkill.Dodge)
            return;

        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    void ChangeColor(Color color)
    {
        foreach (Renderer mesh in _meshs)
            mesh.material.color = color;
    }

    void CheckAttackable(float range = 2.0f)
    {
        if (_locktarget == null)
            return;

        _destPos = _locktarget.transform.position;
        float dist = Vector3.SqrMagnitude(_destPos - transform.position);
        if (dist < range * range)
            State = PlayerState.Attack;

        return;
    }

    public void ClearNearObject()
    {
        _nearObj = null;
        _nearType = eInteract.Unknown;
    }

    bool CheckMousePoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray,out RaycastHit rhit, 100, (1<< (int)eLayer.Ground)))
        {
            _mouseWorldPoint = rhit.point - transform.position;
            _mouseWorldPoint.y = 0;
            _mouseWorldPoint = _mouseWorldPoint.normalized;
            return true;
        }

        _mouseWorldPoint = transform.position;
        return false;
    }

    #endregion [ Sub Function ]

    #region [ Key Action && Mouse Action ]

    void OnKeyBoardEvent()
    {
        if (dict_bool[PlayerBools.Dead])
            return;

        if (dict_bool[PlayerBools.ActDodge])
            return;

        OnInteractEvent(Input.GetKeyDown(KeyCode.F));
    }

    void OnMouseEvent(MouseEvent evt)
    {
        if (dict_bool[PlayerBools.Dead])
            return;

        if (dict_bool[PlayerBools.ActDodge])
            return;

        switch (_state)
        {
            case PlayerState.Idle:
            case PlayerState.Move:
                OnMouseEvent_IDLEMOVE(evt);
                break;
            case PlayerState.Attack:
                {
                    if (evt == MouseEvent.PointerUp)
                        dict_bool[PlayerBools.ContinueAttack] = false;
                }
                break;
            case PlayerState.Skill:
                {
                    if (_sType == eSkill.Spin)
                        OnMouseEvent_SPINMOVE(evt);
                }
                break;
        }
    }

    void OnMouseEvent_IDLEMOVE(MouseEvent evt)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hit = Physics.Raycast(ray, out RaycastHit rhit, 100, _clickMask);
        switch (evt)
        {
            case MouseEvent.PointerDown:
                {
                    if (hit)
                    {
                        _destPos = rhit.point;
                        if (State != PlayerState.Move)
                            State = PlayerState.Move;
                        dict_bool[PlayerBools.ContinueAttack] = true;
                        if (rhit.collider.gameObject.layer == (int)eLayer.Monster)
                            _locktarget = rhit.collider.gameObject;
                        else
                            _locktarget = null;
                    }
                }
                break;
            case MouseEvent.Press:
                {
                    if (_locktarget == null && hit)
                        _destPos = rhit.point;
                }
                break;
            case MouseEvent.PointerUp:
                dict_bool[PlayerBools.ContinueAttack] = false;
                break;
        }
    }

    void OnMouseEvent_SPINMOVE(MouseEvent evt)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hit = Physics.Raycast(ray, out RaycastHit rhit, 100, 1 << (int)eLayer.Ground);
        switch (evt)
        {
            case MouseEvent.PointerDown:
                {
                    if (hit)
                    {
                        _destPos = rhit.point;
                        if (State != PlayerState.Move)
                            State = PlayerState.Move;
                        dict_bool[PlayerBools.ContinueAttack] = true;
                    }
                }
                break;
            case MouseEvent.Press:
                {
                    if (_locktarget == null && hit)
                        _destPos = rhit.point;
                }
                break;
            case MouseEvent.PointerUp:
                dict_bool[PlayerBools.ContinueAttack] = false;
                break;
        }
    }

    #endregion [ Key Action && Mouse Action ]

    #region [ Update By State ]

    void UpdateMove()
    {
        CheckAttackable();
        Vector3 dir = _destPos - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.01f)
            State = PlayerState.Idle;
        else
        {
            float dist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.sqrMagnitude);
            _agent.Move(dir.normalized * dist);

            if(Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, _blockMask))
            {
                //���տ� �ִ��� ���콺�� ��� ������ ������� �ٴ� �ִϸ��̼� ���
                if (Input.GetMouseButton(1))
                    return;
            }

            if (dir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, 20.0f * Time.deltaTime);
            }
        }
    }

    void UpdateAttack()
    {
        if(_locktarget != null)
        {
            Vector3 dir = _locktarget.transform.position - transform.position;
            if(dir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, 20.0f * Time.deltaTime);
            }
        }
    }

    void UpdateSkill()
    {
        if(_mouseWorldPoint != transform.position && _mouseWorldPoint != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(_mouseWorldPoint);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, 20.0f * Time.deltaTime);

        }
    }
    #endregion [ Update By State ]

    #region [ KeyBoard Event ]

    void OnInteractEvent(bool btnDown)
    {
        if (_nearObj == null || _nearType == eInteract.Unknown)
            return;

        if (btnDown)
        {
            switch (_nearType)
            {
                case eInteract.Item:
                    {
                        if(_nearObj.TryGetComponent<Item>(out Item item))
                        {
                            if(item.Root())
                            {
                                ClearNearObject();
                            }
                        }
                    }
                    break;
            }
        }
    }

    #endregion [ KeyBoard Event ]

    #region [ State Event ]

    public void OnAttackEvent()
    {
        if(_locktarget != null)
        {
            if(_locktarget.TryGetComponent<MonsterCtrl>(out MonsterCtrl mc))
            {
                mc.OnDamage(_stat);
                if (mc.isDead)
                {
                    _locktarget = null;
                    dict_bool[PlayerBools.ContinueAttack] = false;
                }
            }
        }
    }

    public void OffAttackEvent()
    {
        if (dict_bool[PlayerBools.ContinueAttack])
            State = PlayerState.Attack;
        else
            State = PlayerState.Idle;
    }

    public void OnDamage(BaseStat stat)
    {
        if (dict_bool[PlayerBools.Dead])
            return;

        if (dict_bool[PlayerBools.ActDodge])
            return;

        dict_bool[PlayerBools.Dead] = _stat.GetHit(stat);
        StopCoroutine(OnDamageEvent());
        StartCoroutine(OnDamageEvent());
    }

    void OnDeadEvent()
    {
        gameObject.DestroyAPS();
        ChangeColor(Color.white);
    }
    #endregion [ State Event ]

    #region [ Stat Event ]

    public void EarnMoney(int gold)
    {
        _stat.Gold += gold;
    }

    public void AddPlusStat(eStat type, float value)
    {
        _stat.AddPlusStat(type, value);
    }

    public void UsePotion(eStat type, float value)
    {
        _stat.BuffEvent(type, value);
    }

    #endregion [ Stat Event ]

    #region [ Skill Event ]

    public void SkillEvent(eSkill type, SOSkill skill)
    {
        switch (type)
        {
            case eSkill.Slash:
                SlashEvent(skill);
                break;
            case eSkill.Heal:
                HealEvent(skill);
                break;
            case eSkill.Dodge:
                DodgeEvent(skill);
                break;
        }
    }

    void SlashEvent(SOSkill skill)
    {
        if (CheckMousePoint())
        {
            StopCoroutine(OnSlashEvent(skill));
            StartCoroutine(OnSlashEvent(skill));
        }
    }

    void HealEvent(SOSkill skill)
    {
        float value = _stat.MaxHP * skill.effectValue;
        UsePotion(eStat.HP, value);
        //StopCoroutine(OnHealEvent(skill.skillName));
        //StartCoroutine(OnHealEvent(skill.skillName));

    }

    void DodgeEvent(SOSkill skill)
    {
        if (CheckMousePoint())
        {
            StopCoroutine(OnDodgeEvent(skill.effectValue, _mouseWorldPoint));
            StartCoroutine(OnDodgeEvent(skill.effectValue, _mouseWorldPoint));
        }
    }

    #endregion [ Skill Event ]

    #region [ Coroutine ]

    IEnumerator OnDamageEvent()
    {
        if (dict_bool[PlayerBools.Dead])
        {
            State = PlayerState.Die;
            ChangeColor(Color.gray);
            yield return new WaitForSeconds(2.0f);
            OnDeadEvent();
            yield break;

        }
        ChangeColor(Color.red);
        yield return new WaitForSeconds(0.3f);
        ChangeColor(Color.white);

    }

    IEnumerator OnSlashEvent(SOSkill skill)
    {
        yield return null;
    }

    IEnumerator OnHealEvent(string name)
    {
        GameObject go = PoolingManager._pool.InstantiateAPS(name, transform.position, Quaternion.identity, Vector3.one, transform);
        ParticleSystem ps = go.GetComponentInChildren<ParticleSystem>();
        ps.Play(true);

        while (ps.IsAlive(true))
        {
            yield return null;
        }

        go.DestroyAPS();
    }

    IEnumerator OnDodgeEvent(float power, Vector3 dir)
    {
        _sType = eSkill.Dodge;
        State = PlayerState.Skill;
        dict_bool[PlayerBools.ActDodge] = true;
        _rb.AddForce(dir * power, ForceMode.Impulse);
        yield return new WaitForSeconds(0.7f);

        _mouseWorldPoint = transform.position;
        _destPos = transform.position;
        dict_bool[PlayerBools.ActDodge] = false;
        _sType = eSkill.Unknown;
        State = PlayerState.Move;
    }

    #endregion [ Coroutine ]

    #region [ OnTrigger ]

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Util.ConvertEnum(eTag.Interact)))
        {
            _nearObj = other.gameObject;
            if (_nearObj.TryGetComponent<InteractObject>(out InteractObject io))
            {
                _nearType = io.InteractType;
            }
            else
            {
                ClearNearObject();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Util.ConvertEnum(eTag.Interact)))
        {
            ClearNearObject();
        }
    }

    #endregion [ OnTrigger ]
}
