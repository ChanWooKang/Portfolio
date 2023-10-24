using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class BossStateInitial : TSingleton<BossStateInitial>, IFSMState<BossCtrl>
{
    public void Enter(BossCtrl m)
    {
        m._offSet = m.transform.position;
        m._stat.SetStat(m.mType);
        m.Init();
        m.ChangeState(BossStateIdle._inst);
    }

    public void Execute(BossCtrl m)
    {
        
    }

    public void Exit(BossCtrl m)
    {
        
    }
}
