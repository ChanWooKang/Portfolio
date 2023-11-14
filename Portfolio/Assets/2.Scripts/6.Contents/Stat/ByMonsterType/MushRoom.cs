using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class MushRoom : MonsterStat
{
    enum AnimationIndex
    {
        HeadAttack = 0,
        KickAttack = 1,
        BombAttack = 2
    }

    public override void Attack(Animator anim)
    {
        int index = PickPattern();

        anim.SetTrigger(Util.ConvertEnum((AnimationIndex)index));
    }
}
