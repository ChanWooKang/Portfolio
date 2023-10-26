using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class MonsterStateReturn : TSingleton<MonsterStateReturn>, IFSMState<MonsterCtrl>
{
    public void Enter(MonsterCtrl m)
    {
        m.Agent.ResetPath();
        m.Agent.updatePosition = false;
        m.Agent.velocity = Vector3.zero;

        m.Agent.speed = m._stat.MoveSpeed;
        m.State = MonsterState.Patrol;
        m.BaseNavSetting();
    }

    public void Execute(MonsterCtrl m)
    {
        if (UI_WorldMap.ActivatedWorldMap)
            return;

        if (m.IsCloseTarget(m._offSet, 0.5f))
        { 
            if(m.target != null)
            {
                if (m.IsCloseTarget(m.target.position, m._stat.TraceRange))
                {
                    m.ChangeState(MonsterStateTrace._inst);
                }
                else
                {
                    m.ChangeState(MonsterStatePatrol._inst);
                }
            }
            else
            {
                m.ChangeState(MonsterStatePatrol._inst);
            }
        }
        else
        {
            if (m.State != MonsterState.Patrol)
                m.State = MonsterState.Patrol;

            m.MoveFunc(m._offSet);
        }
    }

    public void Exit(MonsterCtrl m)
    {
       
    }
}
