using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Define;

public class UI_TargetInfo : UI_Base
{
    enum GameObjects
    {
        Parent,
        TargetName,
        TargetHP
    }

    bool isTargetLock = false;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Close();
        StartCoroutine(TryTargetLock());
    }

    IEnumerator TryTargetLock()
    {
        while(Managers._scene.CurrentScene.CurrScene == Define.eScene.GameScene)
        {
            if (PlayerCtrl._inst.LockTarget != null)
            {
                if (!isTargetLock)
                    Open();
                else
                {
                    GameObject go = PlayerCtrl._inst.LockTarget;
                    if (go.TryGetComponent<MonsterCtrl>(out MonsterCtrl mc))
                    {
                        GetObject((int)GameObjects.TargetName).GetComponent<Text>().text = Util.ConvertEnum((TranslateMonsterName)mc.mType);
                        float ratio = mc._stat.HP / mc._stat.MaxHP;
                        GetObject((int)GameObjects.TargetHP).GetComponent<Image>().fillAmount = ratio;
                    }
                    else if(go.TryGetComponent<BossCtrl>(out BossCtrl bc))
                    {
                        GetObject((int)GameObjects.TargetName).GetComponent<Text>().text = Util.ConvertEnum((TranslateMonsterName)bc.mType);
                        float ratio = bc._stat.HP / bc._stat.MaxHP;
                        GetObject((int)GameObjects.TargetHP).GetComponent<Image>().fillAmount = ratio;
                    }
                }
            }
            else
            {
                if (isTargetLock)
                    Close();
            }
            yield return null;
        }
    }

    void Open()
    {
        isTargetLock = true;
        GetObject((int)GameObjects.Parent).SetActive(true);
    }

    void Close()
    {
        isTargetLock = false;
        GetObject((int)GameObjects.Parent).SetActive(false);
    }
}
