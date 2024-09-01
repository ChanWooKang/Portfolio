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

    public PlayerEquipCtrl pec;
    public PlayerSoundCtrl psc;    
    public SkillCryCtrl _cry;
    public SkillHealCtrl _heal;
    public PotionEffect _potion;
    public ParticleSystem levelUpEffect;
    public PortalEventCtrl _portal;

    //레이어 마스크 
    int _clickMask;
    int _blockMask;

    [SerializeField]
    Transform RespawnPos;

    bool isClickMonster = false;
    bool isBossField = false;
    float hitcntTime = 0;
    float hitDamage = 0;
    [SerializeField, Range(0.2f, 0.5f)] float hitRate;
    [SerializeField, Range(0.2f, 0.5f)]float hitTime;
    [SerializeField, Range(2.0f, 4.0f)] float attackRange;

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

    Coroutine RegerectionCoroutine;
    Coroutine SlashCoroutine;

    #endregion [ Data ]

    #region [ Property ]

    public eSkill SkillState { get { return _sType; } }
    public PlayerState State
    {
        get { return _state; }
        set
        {
            _state = value;
            if (_anim == null)
                _anim = GetComponent<Animator>();

            psc.UpdateSound();
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
                case PlayerState.Return:
                    _anim.CrossFade("Return", 0.1f, -1, 0);
                    break;
            }
        }
    }
    public static PlayerCtrl _inst { get { return _uniqueInstance; } }
    public GameObject LockTarget { get { return _locktarget; } set { _locktarget = value; } }
    public Dictionary<PlayerBools,bool> Bools { get { return dict_bool; } }

    public NavMeshAgent Agent { get { return _agent; } }
    #endregion [ Property ]

    
    void Awake()
    {
        InitComponent();
    }

    void Start()
    {
        Managers._input.KeyAction -= OnKeyBoardEvent;
        Managers._input.KeyAction += OnKeyBoardEvent;
        Managers._input.MouseAction -= OnMouseEvent;
        Managers._input.MouseAction += OnMouseEvent;

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

        _clickMask = (1 << (int)eLayer.Ground) | (1 << (int)eLayer.Monster);
        _blockMask = (1 << (int)eLayer.Block) | (1 << (int)eLayer.TransBlock);

        _destPos = transform.position;
        _mouseWorldPoint = transform.position;

        _locktarget = null;
        _nearObj = null;
        _nearType = eInteract.Unknown;

        dict_bool = new Dictionary<PlayerBools, bool>();
        for (int i = 0; i < (int)PlayerBools.Max_Cnt; i++)
            dict_bool.Add((PlayerBools)i, false);
    }

    public void InitData()
    {        
        StatSetting();
        MinimapCamera._inst.InstiatieMarker(true, transform);
        RegerectionCoroutine = StartCoroutine(RegenerateStat());
    }

    void StatSetting()
    {
        if (Managers.IsNew)
        {
            _stat.Init();
        }
        else
        {
            _stat.LoadPlayer();
        }
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

    void FreezeRotate()
    {
        if (_sType == eSkill.Dodge)
            return;

        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    public void ChangeColor(Color color)
    {
        foreach (Renderer mesh in _meshs)
            mesh.material.color = color;
    }

    void CheckAttackable(float range = 2.0f)
    {
        if (isClickMonster == false)
            return;        
            
        if(CheckDistance(range))
            State = PlayerState.Attack;        
    }

    bool CheckDistance(float range)
    {
        if (_locktarget == null)
            return false;

        Vector3 goalPos = _locktarget.transform.position;
        Vector3 dir = goalPos - transform.position;
        dir = dir.normalized;

        Ray ray = new Ray(transform.position + Vector3.up, dir * range);
        if (Physics.Raycast(ray, out RaycastHit rhit, range, (1 << (int)eLayer.Monster)))
        {
            return true;
        }
        else
        {
            return false;
        }
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

    public void PlayPortalEvent()
    {
        _portal.PlayPortal();
    }

    public void ReturnHome()
    {
        _portal.StopPortal();
        _agent.enabled = false;
        transform.position = RespawnPos.position;
        _agent.enabled = true;
        State = PlayerState.Idle;
        
    }    

    
    #endregion [ Sub Function ]

    #region [ Key Action && Mouse Action ]

    void OnKeyBoardEvent()
    {
        if (dict_bool[PlayerBools.Dead] || State == PlayerState.Return)
            return;

        if (UI_WorldMap.ActivatedWorldMap || UI_Inventory.ActivatedInventory || UI_Shop.ActivatedShop || UI_Quest.ActivatedQuestWindow)
            return;

        if (dict_bool[PlayerBools.ActDodge])
            return;

        if (_portal.isActivePortal)
            _portal.StopPortal();

        OnInteractEvent(Input.GetKeyDown(KeyCode.F));

        if (Input.GetKeyDown(KeyCode.B))
        {
            Stop();
            State = PlayerState.Return;           
        }

        //For Test
        if (Input.GetKeyDown(KeyCode.G))
        {
            _stat.EXP += 3001;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            _stat.EXP += 301;
            _stat.AddPlusStat(eStat.HP, 500);
            _stat.AddPlusStat(eStat.MP, 500);
            _stat.AddPlusStat(eStat.Damage, 50);
            _stat.AddPlusStat(eStat.Defense, 10);
        }
    }    

    void OnMouseEvent(MouseEvent evt)
    {
        //플레이어 상태가 회피중이거나 죽었을때 작용 X
        if (dict_bool[PlayerBools.Dead] || dict_bool[PlayerBools.ActDodge])
            return;

        //UI가 열려 있을 경우 플레이어 이동 제어
        if (UI_WorldMap.ActivatedWorldMap || UI_Inventory.ActivatedInventory || UI_Quest.ActivatedQuestWindow)
            return;        

        //포탈이 작동 중인 경우 포탈 취소 작업
        if (_portal.isActivePortal)
            _portal.StopPortal();

        switch (_state)
        {
            case PlayerState.Idle:
            case PlayerState.Move:
            case PlayerState.Return:
                OnMouseEvent_IDLEMOVE(evt);
                break;
            case PlayerState.Attack:                
                if (evt == MouseEvent.PointerUp)
                    dict_bool[PlayerBools.ContinueAttack] = false;
                else
                {
                    if (CheckDistance(attackRange) == false)
                        dict_bool[PlayerBools.ContinueAttack] = false;
                }
                break;
            case PlayerState.Skill:
                if (_sType == eSkill.Spin)
                    OnMouseEvent_SPINMOVE(evt);
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
                        if (_agent.updatePosition == false)
                            _agent.updatePosition = true;

                        _destPos = rhit.point;
                        if (State != PlayerState.Move)
                            State = PlayerState.Move;
                        dict_bool[PlayerBools.ContinueAttack] = true;
                        if (rhit.collider.gameObject.layer == (int)eLayer.Monster)
                        {
                            isClickMonster = true;
                            _locktarget = rhit.transform.gameObject;
                        }
                        else
                        {
                            isClickMonster = false;
                            if (isBossField == false)
                            {
                                _locktarget = null;
                            }                           
                        }
                            
                    }
                    else
                    {
                        if (State == PlayerState.Move)
                            State = PlayerState.Idle;
                    }
                }
                break;
            case MouseEvent.Press:
                {
                    if(isBossField == false)
                    {
                        if (_locktarget == null && hit)
                            _destPos = rhit.point;
                    }
                    else
                    {
                        if (hit)
                            _destPos = rhit.point;
                    }
                    
                }
                break;
            case MouseEvent.PointerUp:
                isClickMonster = false;
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
        CheckAttackable(attackRange);
        Vector3 dir = _destPos - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.01f)
            State = PlayerState.Idle;
        else
        {
            float dist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.sqrMagnitude);
            _agent.Move(dir.normalized * dist);

            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, _blockMask))
            {
                //벽앞에 있더라도 마우스를 계속 누르고 있을경우 뛰는 애니메이션 재생
                if (Input.GetMouseButton(0))
                    return;
                else
                    Stop();
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
                                psc.RootingSound();
                                ClearNearObject();
                            }
                        }
                    }
                    break;
                case eInteract.Shop:
                case eInteract.NPC:
                    {
                        GameManagerEX._inst.ShowText(_nearObj, _nearType);
                    }
                    break;
                    
            }
        }
    }    

    public void Stop()
    {
        _destPos = transform.position;
        _agent.velocity = Vector3.zero;
        State = PlayerState.Idle;
    }

    #endregion [ KeyBoard Event ]

    #region [ State Event ]

    public void LevelUp()
    {
        psc.LevelUpSound();
        //레벨업시 체크 
        levelUpEffect.Play(true);
        GameManagerEX._inst.information.CheckSkillAble();
    }

    public void OnAttackEvent()
    {
        if(_locktarget != null)
        {
            if(_locktarget.TryGetComponent(out MonsterCtrl mc))
            {
                mc.OnDamage(_stat);
                if (mc.isDead)
                {
                    _locktarget = null;
                    dict_bool[PlayerBools.ContinueAttack] = false;
                }
            }
            else if(_locktarget.TryGetComponent(out BossCtrl bc))
            {
                bc.OnDamage(_stat);
                if (bc.isDead)
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

    public void OnDamage()
    {
        StopCoroutine(OnDamageEvent());
        StartCoroutine(OnDamageEvent());
    }

    public void OnDamage(BaseStat stat)
    {
        if (dict_bool[PlayerBools.Dead] || dict_bool[PlayerBools.ActDodge])
            return;

        dict_bool[PlayerBools.Dead] = _stat.GetHit(stat);
        OnDamage();
    }

    public void OnDamage(float damage)
    {
        if (dict_bool[PlayerBools.Dead] || dict_bool[PlayerBools.ActDodge])
            return;

        dict_bool[PlayerBools.Dead] = _stat.GetHit(damage);
        OnDamage();
    }

    public void OnDeadEvent()
    {
        AttackNavSetting();
        GameManagerEX._inst.GameOver();
        
    }

    public void OnResurrectEvent()
    {
        _locktarget = null;
        dict_bool[PlayerBools.Dead] = false;
        transform.position = RespawnPos.position;
        transform.rotation = Quaternion.identity;
    }

    public void OnStartRegenarte()
    {   
        State = PlayerState.Idle;
       

        if (RegerectionCoroutine != null)
            StopCoroutine(RegerectionCoroutine);
        RegerectionCoroutine = StartCoroutine(RegenerateStat());
    }

    public void SetInBossField(GameObject boss = null,bool Inside = false)
    {
        isBossField = Inside;
        _locktarget = boss;
    }
    #endregion [ State Event ]

    #region [ Stat Event ]

    public void ResetStat()
    {
        _stat.Init();
    }

    public void EarnMoney(int gold)
    {
        _stat.Gold += gold;
    }

    public bool TryUseMoney(int gold)
    {
        return _stat.TryUseMoney(gold);
    }

    public void AddPlusStat(eStat type, float value)
    {
        _stat.AddPlusStat(type, value);
    }

    public void UsePotion(eStat type, float value)
    {
        psc.UsePotionSound();
        _stat.UsePotion(type, value);
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
            if(SlashCoroutine != null)
                StopCoroutine(OnSlashEvent(skill, _mouseWorldPoint));
            SlashCoroutine = StartCoroutine(OnSlashEvent(skill, _mouseWorldPoint));
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

    public void PotionEvent(ePotion type)
    {
        _potion.EffectPlay(type);
    }

    public void CancelSlash(GameObject go)
    {
        if (SlashCoroutine != null)
            StopCoroutine(SlashCoroutine);

        if (go.activeSelf)
            go.DestroyAPS();
    }

    #endregion [ Skill Event ]

    #region [ Coroutine ]

    IEnumerator RegenerateStat()
    {
        float hpRate = _stat.MaxHP * 0.025f;
        float mpRate = _stat.MaxMP * 0.05f;
        while (dict_bool[PlayerBools.Dead] == false)
        {
            if (_stat.HP < _stat.MaxHP)
            {
                _stat.HP = Mathf.Min(_stat.HP + hpRate, _stat.MaxHP);
            }

            if (_stat.MP < _stat.MaxMP)
            {
                _stat.MP = Mathf.Min(_stat.MP + mpRate, _stat.MaxMP);                
            }

            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator OnDamageEvent()
    {
        psc.GetHitSound();
        FloatText.Create("FloatText", false, transform.position, (int)_stat.AttackedDamage);
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

        //마우스 위치로 회전
        Quaternion rot = Quaternion.LookRotation(_mouseWorldPoint);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, 20.0f);

        _sType = eSkill.Slash;
        State = PlayerState.Skill;
        
        yield return new WaitForSeconds(0.2f);        
        GameObject go = PoolingManager._pool.InstantiateAPS(skill.skillName);
        SkillSlashCtrl ssc = go.GetComponent<SkillSlashCtrl>();
        ssc.SlashEvent(skill, transform, dir);

        dict_bool[PlayerBools.ActSkill] = false;

        _sType = eSkill.Unknown;
        State = PlayerState.Move;

        //2초후 소멸
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
        pec.nowWeapon.WeaponUse(damage, skill.duration);

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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            hitcntTime = 1;
            hitDamage = other.GetComponentInParent<Boss_Flame>().Damage * hitRate;
        }

        if (other.CompareTag("Boss"))
        {
            if(other.TryGetComponent<BossCtrl>(out BossCtrl bc))
            {
                if(bc.HandDamage>0)
                {
                    OnDamage(bc.HandDamage);
                    bc.HandDamage = 0;   
                }
            }
        }

        if (other.CompareTag("FireBall"))
        {
            if(other.TryGetComponent(out FireBall fb))
            {
                OnDamage(fb.Damage);                
            }
        }

    }
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

        if (other.CompareTag("Fire"))
        {
            hitcntTime += Time.deltaTime;
            if (hitcntTime > hitTime)
            {
                if (hitDamage > 0) 
                    OnDamage(hitDamage);

                hitcntTime = 0;
            }
        }

        if (other.CompareTag("Boss"))
        {
            if (other.TryGetComponent<BossCtrl>(out BossCtrl bc))
            {
                if (bc.HandDamage > 0)
                {
                    OnDamage(bc.HandDamage);
                    bc.HandDamage = 0;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Util.ConvertEnum(eTag.Interact)))
        {
            if (other.TryGetComponent<Shop>(out Shop shop))
            {
                shop.CloseShop();
            }

            ClearNearObject();
        }

        if (other.CompareTag("Fire"))
        {
            hitDamage = 0;
            hitcntTime = 0;
        }

       
    }

    #endregion [ OnTrigger ]
}
