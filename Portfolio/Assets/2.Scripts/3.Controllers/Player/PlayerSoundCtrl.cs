using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class PlayerSoundCtrl : MonoBehaviour
{
    PlayerCtrl pc;
    AudioSource source;
    public void UpdateSound()
    {        
        if(pc == null)
            pc = GetComponent<PlayerCtrl>();
        
        switch (pc.State)
        {
            case PlayerState.Idle:
                StopAudio();
                break;
            case PlayerState.Move:
                SetClip(eSoundList.Player_Move,true);
                break;
            case PlayerState.Attack:
                SetClip(eSoundList.Player_Hit);
                break;
            case PlayerState.Skill:
                StopAudio();
                switch (pc.SkillState)
                {
                    case eSkill.Dodge:
                        SetClip(eSoundList.Player_Dodge);
                        break;
                    case eSkill.Spin:
                        SetClip(eSoundList.Player_Spin, true);
                        break;
                    case eSkill.Cry:
                        SetClip(eSoundList.Player_Cry);
                        break;
                    case eSkill.Heal:
                        SetClip(eSoundList.Player_Heal);
                        break;
                    case eSkill.Slash:
                        SoundManager._inst.Play(eSoundList.Player_Slash);
                        break;
                }
                break;
        }
    }
  
    public void GetHitSound()
    {
        SetClip(eSoundList.GetHit);
    }

    public void RootingSound()
    {
        SetClip(eSoundList.Player_Pickup);
    }

    public void LevelUpSound()
    {
        SetClip(eSoundList.Player_LevelUp);
    }

    public void UsePotionSound()
    {
        SoundManager._inst.Play(eSoundList.Player_Heal);
    }

    public void StopAudio()
    {
        if (source == null)
            source = GetComponent<AudioSource>();

        if (source.isPlaying)
            source.Stop();
        source.clip = null;
    }

    void SetClip(eSoundList sList, bool isLoop = false)
    {
        if (source == null)
            source = GetComponent<AudioSource>();

        AudioClip clip = SoundManager._inst.GetOrAddAudioClip(sList);

        if(clip != null)
        {
            if (isLoop)
            {
                source.clip = clip;
                source.loop = true;
                source.Play();
            }
            else
            {
                source.loop = false;
                source.PlayOneShot(clip);
            }
        }
        
    }
}
