using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class MonsterStateTrace : TSingleton<MonsterStateTrace>, IFSMState<MonsterCtrl>
{
    public void Enter(MonsterCtrl m)
    {
        m.Agent.speed = m._stat.TraceSpeed;
        m.State = MonsterState.Trace;
    }

    public void Execute(MonsterCtrl m)
    {
        if (m.IsTooFar())
        {
            m.ChangeState(MonsterStatePatrol._inst);
            return;
        }

        if (m.target != null)
        {
            if (m.IsCloseTarget(m.target.position, m._stat.TraceRange))
            {
                m.MoveFunc(m.target.position);
                if (m.IsCloseTarget(m.target.position, m._stat.AttackRange))
                    m.ChangeState(MonsterStateAttack._inst);

            }
            else
                m.ChangeState(MonsterStatePatrol._inst);
        }
        else
            m.ChangeState(MonsterStatePatrol._inst);
    }

    public void Exit(MonsterCtrl m)
    {
       
    }
}
