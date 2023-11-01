using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScenePlayer : MonoBehaviour
{
    enum Animations
    {
        IDLE,
        Victory,
        Level,
        End
    }

    Animator _ani;
    Animations _state = Animations.IDLE;
    float[] rate = { 0.6f, 0.8f, 1.0f };
    float cntTime;
    void Start()
    {
        _ani = GetComponent<Animator>();
        StartCoroutine(PlayAnim());
        Managers._input.KeyAction -= OnKeyBoard;
        Managers._input.KeyAction += OnKeyBoard;
    }

    void OnKeyBoard()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Managers._scene.CurrentScene.Quit();
        }
    }

    IEnumerator PlayAnim()
    {
        cntTime = 0;
        while (Managers._scene.CurrentScene.CurrScene == Define.eScene.MainScene)
        {
            if(_state == Animations.IDLE)
            {
                cntTime += Time.deltaTime;
                if(cntTime > 3.0f)
                {
                    PickAnim();
                    cntTime = 0;
                }
                
            }
            yield return null;
            
        }

    }

    public void End()
    {
        _state = Animations.IDLE;
    }

    void PickAnim()
    {
        float value = Random.Range(0f, 1.0f);
        Debug.Log(value);
        if(value < rate[0])
        {
            _state = Animations.IDLE;
        }
        else if( value < rate[1])
        {
            _state = Animations.Victory;
        }
        else
        {
            _state = Animations.Level;
        }

        Debug.Log(_state);
        ChangeAnim();
    }

    void ChangeAnim()
    {
        
        if (_ani == null)
            _ani.GetComponent<Animator>();
        switch (_state)
        {
            case Animations.IDLE:
                _ani.CrossFade("IDLE", 0.1f);
                _state = Animations.IDLE;
                break;
            case Animations.Victory:
                _ani.CrossFade("Victory", 0.1f, -1, 0);
                _state = Animations.Victory;
                break;
            case Animations.Level:
                _ani.CrossFade("Level", 0.1f, -1, 0);
                _state = Animations.Level;
                break;
        }
    }
}
