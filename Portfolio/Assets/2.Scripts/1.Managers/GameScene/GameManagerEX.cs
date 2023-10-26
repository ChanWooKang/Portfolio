using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class GameManagerEX : MonoBehaviour
{
    static GameManagerEX _uniqueInstance;
    public static GameManagerEX _inst { get { return _uniqueInstance; } }

    PlayerCtrl player;

    void Awake()
    {
        _uniqueInstance = this;
    }

    public void PlayerDeadAction(PlayerCtrl pc)
    {
        player = pc;
        player.gameObject.SetActive(false);
        player.ChangeColor(Color.white);

        Invoke("PlayerResurrectAction", 2.0f);
    }

    public void PlayerResurrectAction()
    {
        if(player != null)
        {
            player.OnResurrectEvent();
            player.gameObject.SetActive(true);
        }
    }
}
