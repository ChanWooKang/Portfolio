using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class BossStateTrace : TSingleton<BossStateTrace>, IFSMState<BossCtrl>
{
    public void Enter(BossCtrl m)
    {
        m.Agent.speed = m._stat.TraceSpeed;
        m.State = BossState.Trace;
    }

    public void Execute(BossCtrl m)
    {
        if (UI_WorldMap.ActivatedWorldMap)
            return;

        if (m.target != null)
        {
            if (m.IsCloseTarget(m.target.position, m._stat.TraceRange))
            {
                m.MoveFunc(m.target.position);
                if (m.IsCloseTarget(m.target.position, m._stat.AttackRange))
                    m.ChangeState(BossStateAttack._inst);
            }
        }
        else
        {
            //게임오버 안내 재생

            m.ChangeState(BossStateReturnHome._inst);
        }
            
    }

    public void Exit(BossCtrl m)
    {
       
    }
}
