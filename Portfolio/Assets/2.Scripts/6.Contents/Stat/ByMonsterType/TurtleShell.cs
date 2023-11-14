using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class TurtleShell : MonsterStat
{
    enum AnimationIndex
    {
        BodyAttack = 0,
        SpinAttack = 1,
        BombAttack = 2
    }

    public override void Attack(Animator anim)
    {
        int index = PickPattern();

        anim.SetTrigger(Util.ConvertEnum((AnimationIndex)index));
    }
}
