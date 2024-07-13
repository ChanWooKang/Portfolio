using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class Cactus : MonsterStat
{
    enum AnimationIndex
    {
        PunchAttack = 0,
        HeadAttack = 1,
        FireBall = 2
    }
    

    public override void Attack(Animator anim)
    {
        int index = PickPattern();
        anim.SetTrigger(Util.ConvertEnum((AnimationIndex)index));
    }

    public override void SpeacialAttack(Vector3 targetPos)
    {
        GameObject go = PoolingManager._pool.InstantiateAPS("FireBall");
        FireBall fire = go.GetComponent<FireBall>();
        fire.ShootEvent(transform, targetPos, Damage);
    }

}
