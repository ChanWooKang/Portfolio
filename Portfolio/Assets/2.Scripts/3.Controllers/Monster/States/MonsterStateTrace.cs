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
        m.BaseNavSetting();
    }

    public void Execute(MonsterCtrl m)
    {
        if (UI_WorldMap.ActivatedWorldMap)
            return;

        if (m.IsTooFar())
        {
            m.ChangeState(MonsterStateReturn._inst);
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
