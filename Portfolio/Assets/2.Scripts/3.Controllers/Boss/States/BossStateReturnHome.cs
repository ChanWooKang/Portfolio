using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class BossStateReturnHome : TSingleton<BossStateReturnHome>, IFSMState<BossCtrl>
{
    public void Enter(BossCtrl m)
    {
        m.Agent.speed = m._stat.MoveSpeed;
        m.State = BossState.Return;
    }

    public void Execute(BossCtrl m)
    {
        if (m.IsCloseTarget(m._offSet, 0.5f))
        {
            if(m.target != null)
            {
                m.ChangeState(BossStateTrace._inst);
            }
            else
            {
                m.ChangeState(BossStateInitial._inst);
            }

        }
        else
        {
            m.MoveFunc(m._offSet);
        }
    }

    public void Exit(BossCtrl m)
    {
        
    }
}
