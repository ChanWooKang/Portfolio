using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class BossStateReturnHome : TSingleton<BossStateReturnHome>, IFSMState<BossCtrl>
{
    public void Enter(BossCtrl m)
    {
        m.BaseNavSetting();
        m.Agent.speed = m._stat.MoveSpeed;
        m.State = BossState.Return;
    }

    public void Execute(BossCtrl m)
    {
        Debug.Log("현재 보스 상태 : Return");
        if (m.IsCloseTarget(m._offSet, 0.5f))
        {
            if(m.target != null)
            {
                if (m.IsCloseTarget(m.target.position, m._stat.TraceRange))
                {
                    m.ChangeState(BossStateTrace._inst);
                }
                else
                {
                    if (m.State != BossState.Sleep)
                        m.State = BossState.Sleep;
                }
                
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
