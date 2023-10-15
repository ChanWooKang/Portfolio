using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Define;
using System;

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

    WeaponCtrl _weapon;
    SkillCryCtrl _cry;
    SkillHealCtrl _heal;

    //레이어 마스크 
    int _clickMask;
    int _blockMask;

    // 이동 좌표
    Vector3 _destPos;
    // 스킬 입력 시 마우스 위치 월드 좌표 저장
    Vector3 _mouseWorldPoint;

    //마우스 클릭시 오브젝트 고정
    GameObject _locktarget;

    //상호작용 오브젝트 인식
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
                            case eSkill.Cry:
                                _anim.CrossFade("Cry", 0.1f, -1, 0);
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
                        case eSkill.Spin:
                            UpdateMoveDuringSKill();
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
        _meshs = transform.GetChild(0).GetComponentsInChildren<Renderer>();
        _stat = new PlayerStat();

        //스킬 관련 처리
        // 휠윈드 기능
        _weapon = GetComponentInChildren<WeaponCtrl>();
        // 함성 기능
        _cry = GetComponentInChildren<SkillCryCtrl>();
        // 힐링 기능
        _heal = GetComponentInChildren<SkillHealCtrl>();
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

        StartCoroutine(RegenerateStat());
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
            case MouseEvent.Press:
                {
                    if (hit)
                    {
                        _destPos = rhit.point;                        
                    }
                }
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
                //벽앞에 있더라도 마우스를 계속 누르고 있을경우 뛰는 애니메이션 재생
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

    void UpdateMoveDuringSKill()
    {
        Vector3 dir = _destPos - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude >= 0.01f)
        {
            float dist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.sqrMagnitude);
            _agent.Move(dir.normalized * dist);

            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, _blockMask))
                return;

            if(dir != Vector3.zero)
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

    public bool UseMP(float value)
    {
        return _stat.UseMP(value);
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
            case eSkill.Spin:
                SpinEvent(skill);
                break;
            case eSkill.Cry:
                CryEvent(skill);
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
            StopCoroutine(OnSlashEvent(skill, _mouseWorldPoint));
            StartCoroutine(OnSlashEvent(skill, _mouseWorldPoint));
        }
    }

    void HealEvent(SOSkill skill)
    {
        float value = _stat.MaxHP * skill.effectValue;
        UsePotion(eStat.HP, value);
        _heal.HealEffect();
    }

    void SpinEvent(SOSkill skill)
    {
        // 연동 및 세팅
        StopCoroutine(OnSpinEvent(skill, skill.duration));
        StartCoroutine(OnSpinEvent(skill, skill.duration));
    }

    void CryEvent(SOSkill skill)
    {       
        StopCoroutine(OnCryEvent(skill, skill.duration));
        StartCoroutine(OnCryEvent(skill,skill.duration));
    }

    void DodgeEvent(SOSkill skill)
    {
        if (CheckMousePoint())
        {
            StopCoroutine(OnDodgeEvent(skill.effectValue, _mouseWorldPoint));
            StartCoroutine(OnDodgeEvent(skill.effectValue, _mouseWorldPoint));
        }
        
    }

    public void EndSkillAnim()
    {
        _sType = eSkill.Unknown;
        Vector3 dir = _destPos - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.01f)
            State = PlayerState.Idle;
        else
            State = PlayerState.Move;
    }

    #endregion [ Skill Event ]

    #region [ Coroutine ]

    IEnumerator RegenerateStat()
    {
        while (dict_bool[PlayerBools.Dead] == false)
        {
            if (_stat.HP < _stat.MaxHP)
            {
                _stat.HP = Mathf.Min(_stat.HP + 5, _stat.MaxHP);
            }

            if (_stat.MP < _stat.MaxMP)
            {
                _stat.MP = Mathf.Min(_stat.MP + 15, _stat.MaxMP);                
            }

            yield return new WaitForSeconds(1);
        }
    }

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

    IEnumerator OnSlashEvent(SOSkill skill, Vector3 dir)
    {       

        dict_bool[PlayerBools.ActSkill] = true;

        Quaternion rot = Quaternion.LookRotation(_mouseWorldPoint);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, 20.0f);


        _sType = eSkill.Slash;
        State = PlayerState.Skill;
        
        yield return new WaitForSeconds(0.2f);
        //Test
        GameObject go = PoolingManager._pool.InstantiateAPS(skill.skillName);
        SkillSlashCtrl ssc = go.GetComponent<SkillSlashCtrl>();
        ssc.SlashEvent(skill, transform, dir);

        dict_bool[PlayerBools.ActSkill] = false;

        _sType = eSkill.Unknown;
        State = PlayerState.Move;

        yield return new WaitForSeconds(2.0f);
        go.DestroyAPS();
    }

    IEnumerator OnSpinEvent(SOSkill skill, float time)
    {
        dict_bool[PlayerBools.ActSkill] = true;
        _sType = eSkill.Spin;
        State = PlayerState.Skill;

        float damage = _stat.Damage * skill.effectValue;

        //콜라이더 생성
        _weapon.WeaponUse(damage, skill.duration);

        yield return new WaitForSeconds(time);

        dict_bool[PlayerBools.ActSkill] = false;

        _sType = eSkill.Unknown;

        Vector3 dir = _destPos - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.01f)
            State = PlayerState.Idle;
        else
            State = PlayerState.Move;

    }

    IEnumerator OnCryEvent(SOSkill skill, float time)
    {
        _sType = eSkill.Cry;
        State = PlayerState.Skill;        
        yield return new WaitForSeconds(0.53f);
        float damage = _stat.Damage * skill.effectValue;
        _cry.CryActive(damage, skill.duration);

        List<STAT> list = skill.sList;
        for(int i = 0; i < list.Count; i++)
        {
            AddPlusStat(list[i].statType, list[i].sValue);
        }

        yield return new WaitForSeconds(time);

        for (int i = 0; i < list.Count; i++)
        {
            AddPlusStat(list[i].statType, -list[i].sValue);
        }
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
