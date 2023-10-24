using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class BossStateDie : TSingleton<BossStateDie>, IFSMState<BossCtrl>
{
    public void Enter(BossCtrl m)
    {
        m.ChangeLayer(eLayer.Disable);
        m.Agent.destination = m.transform.position;
        m.State = BossState.Die;
        
    }

    public void Execute(BossCtrl m)
    {
        
    }

    public void Exit(BossCtrl m)
    {
       
    }
}
