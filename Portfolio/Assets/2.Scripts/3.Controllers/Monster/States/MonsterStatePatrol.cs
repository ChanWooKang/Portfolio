using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class MonsterStatePatrol : TSingleton<MonsterStatePatrol>, IFSMState<MonsterCtrl>
{
    public void Enter(MonsterCtrl m)
    {
        m.BaseNavSetting();
        m.targetPos = m._offSet;
        m.cntTime = 0;
        m.Agent.speed = m._stat.MoveSpeed;
        m.State = MonsterState.Patrol;
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
                m.ChangeState(MonsterStateTrace._inst);
            else
            {
                if (m.IsCloseTarget(m.targetPos, 1.0f))
                {
                    m.cntTime += Time.deltaTime;
                    if (m.cntTime > m.delayTime)
                    {
                        m.cntTime = 0;
                        m.targetPos = m.GetRandomPos();
                    }
                    else
                    {
                        switch (m.mType)
                        {
                            default:
                                if (m.State != MonsterState.Sense)
                                    m.State = MonsterState.Sense;
                                break;
                        }
                    }
                }
                else
                {
                    if (m.State != MonsterState.Patrol)
                        m.State = MonsterState.Patrol;
                    m.MoveFunc(m.targetPos);
                }
            }
        }
        else
        {
            if (m.IsCloseTarget(m.targetPos, 1.0f))
            {
                m.cntTime += Time.deltaTime;
                if (m.cntTime > m.delayTime)
                {
                    m.cntTime = 0;
                    m.targetPos = m.GetRandomPos();
                }
                else
                {
                    switch (m.mType)
                    {
                        default:
                            if (m.State != MonsterState.Sense)
                                m.State = MonsterState.Sense;
                            break;
                    }
                }
            }
            else
            {
                if (m.State != MonsterState.Patrol)
                    m.State = MonsterState.Patrol;
                m.MoveFunc(m.targetPos);
            }
        }


    }

    public void Exit(MonsterCtrl m)
    {
        
    }
}
