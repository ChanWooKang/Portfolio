using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class MonsterStateAttack : TSingleton<MonsterStateAttack>, IFSMState<MonsterCtrl>
{
    public void Enter(MonsterCtrl m)
    {
        m.nowCombo = eCombo.Hit1;
    }

    public void Execute(MonsterCtrl m)
    {
        if (m.target == null)
            m.ChangeState(MonsterStatePatrol._inst);
        else
        {
            m.TurnTowardPlayer();
            if (m.IsCloseTarget(m.target.position, m._stat.AttackRange))
            {
                m.cntTime += Time.deltaTime;
                if (m.cntTime > m._stat.AttackDelay && m.isAttack == false)
                {
                    m.AttackFunc();
                    m.cntTime = 0;
                }
            }
            else
            {
                if (m.isAttack == false)
                    m.ChangeState(MonsterStateTrace._inst);
            }
        }
    }

    public void Exit(MonsterCtrl m)
    {
        
    }
}
