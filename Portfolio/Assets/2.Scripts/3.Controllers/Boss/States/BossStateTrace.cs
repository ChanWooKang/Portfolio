using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class BossStateTrace : TSingleton<BossStateTrace>, IFSMState<BossCtrl>
{    
    public void Enter(BossCtrl m)
    {
        m.BaseNavSetting();
        m.Agent.speed = m._stat.TraceSpeed;
        m.cntTime = 0;
    }

    public void Execute(BossCtrl m)
    {
        if (UI_WorldMap.ActivatedWorldMap)
            return;

        if (m.target != null)
        {
            if (m.IsCloseTarget(m.target.position, m._stat.TraceRange))
            {
                if (m.State != BossState.Trace)
                    m.State = BossState.Trace;

                m.MoveFunc(m.target.position);
                if (m.IsCloseTarget(m.target.position, m._stat.AttackRange))
                    m.ChangeState(BossStateAttack._inst);
            }
            else
            {

                if (m.State != BossState.Idle)
                {
                    m.State = BossState.Idle;
                }

                m.cntTime += Time.deltaTime;
                if (m.cntTime > 5.0f)
                {
                    m.ChangeState(BossStateReturnHome._inst);
                }
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
