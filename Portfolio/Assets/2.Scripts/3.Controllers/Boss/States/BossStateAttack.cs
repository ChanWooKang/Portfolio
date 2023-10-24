using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class BossStateAttack : TSingleton<BossStateAttack>, IFSMState<BossCtrl>
{
    public void Enter(BossCtrl m)
    {
        m.Agent.SetDestination(m.transform.position);
    }

    public void Execute(BossCtrl m)
    {
        if (m.target == null)
            m.ChangeState(BossStateReturnHome._inst);
        else
        {
            m.TurnTowardPlayer();
            if (m.IsCloseTarget(m.target.position, m._stat.AttackRange))
            {
                m.cntTime += Time.deltaTime;
                if (m.cntTime > m._stat.AttackDelay && m.isAttack == false)
                {
                    m.AttackEvent();
                }
            }
            else
            {
                if (m.isAttack == false)
                    m.ChangeState(BossStateTrace._inst);
            }
        }
    }

    public void Exit(BossCtrl m)
    {
       
    }
}
