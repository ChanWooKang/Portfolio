using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class BossStateAttack : TSingleton<BossStateAttack>, IFSMState<BossCtrl>
{
    public void Enter(BossCtrl m)
    {
        m.AttackNavSetting();
        m.Agent.SetDestination(m.transform.position);
        m.cntTime = 10;
    }

    public void Execute(BossCtrl m)
    {
        if (m.target == null)
            m.ChangeState(BossStateReturnHome._inst);
        else
        {
            
            if (m.IsCloseTarget(m.target.position, m._stat.AttackRange))
            {
                if(m.isAttack == false)
                    m.TurnTowardPlayer();

                m.cntTime += Time.deltaTime;
                if (m.cntTime > m._stat.AttackDelay && m.isAttack == false)
                {
                    m.AttackEvent();
                }
                else
                {
                    if (m.State != BossState.Idle && m.isAttack == false)
                        m.State = BossState.Idle;
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
