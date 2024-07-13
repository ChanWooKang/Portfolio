using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class BossStateDisable : TSingleton<BossStateDisable>, IFSMState<BossCtrl>
{
    public void Enter(BossCtrl m)
    {
        m.gameObject.SetActive(false);
    }

    public void Execute(BossCtrl m)
    {
        
    }

    public void Exit(BossCtrl m)
    {
        
    }
}
