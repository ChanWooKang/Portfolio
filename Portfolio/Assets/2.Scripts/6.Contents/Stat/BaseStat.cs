using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BaseStat
{
    protected int _level;
    protected float _hp;
    protected float _maxhp;
    protected float _damage;
    protected float _defense;
    protected float _moveSpeed;

    #region [ Property ]

    public int Level { get { return _level; } set { _level = value; } }
    public float HP { get { return _hp; } set { _hp = value; } }
    public float MaxHP { get { return _maxhp; } set { _maxhp = value; } }
    public float Damage { get { return _damage; } set { _damage = value; } }
    public float Defense { get { return _defense; } set { _defense = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }

    #endregion [ Property ]

    public virtual bool GetHit(BaseStat attacker)
    {
        float per = 0.06f;
        float damage = Mathf.Max(0.5f, attacker._damage * ((_defense * per) / (1 + per * _defense)));

        //float damage = Mathf.Max(0.5f, attacker._damage - _defense);
        
        if(_hp > damage)
        {
            _hp -= damage;
            return false;
        }
        else
        {
            _hp = 0;
            return true;
        }
    }

    public virtual bool GetHit(float damage)
    {        
        if (_hp > damage)
        {
            _hp -= damage;
            return false;
        }
        else
        {
            _hp = 0;
            return true;
        }
    }
}
