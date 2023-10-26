using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class BossStateIdle : TSingleton<BossStateIdle>, IFSMState<BossCtrl>
{
    public void Enter(BossCtrl m)
    {
        m.State = BossState.Sleep;
    }

    public void Execute(BossCtrl m)
    {
        Debug.Log("���� ���� ���� : IDLE");
    }

    public void Exit(BossCtrl m)
    {
        
    }
}
