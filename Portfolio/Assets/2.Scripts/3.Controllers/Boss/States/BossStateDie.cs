using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class BossStateDie : TSingleton<BossStateDie>, IFSMState<BossCtrl>
{
    float cntTime;
    public void Enter(BossCtrl m)
    {
        m.ChangeLayer(eLayer.Disable);
        m.Agent.destination = m.transform.position;
        m.State = BossState.Die;
        cntTime = 0;
    }

    public void Execute(BossCtrl m)
    {
        if (m.isActiveAndEnabled)
        {
            cntTime += Time.deltaTime;
            if (cntTime > m.delayTime)
                m.OnDeadEvent();
        }
    }

    public void Exit(BossCtrl m)
    {
       
    }
}
