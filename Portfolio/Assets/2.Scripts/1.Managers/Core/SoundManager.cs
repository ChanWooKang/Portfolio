using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class SoundManager : MonoBehaviour
{
    AudioSource[] audioSources = new AudioSource[(int)eSound.Max_Cnt];
    List<AudioSource> list_Sources = new List<AudioSource>();
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    [SerializeField]List<Audios> _audios;

    [System.Serializable]
    class Audios
    {
        public string Key;
        public AudioClip clip;
    }

    static SoundManager _uniqueInstance;
    public static SoundManager _inst { get {return _uniqueInstance; } }

    [SerializeField,Range(0,1)]
    public float BGMValue;
    [SerializeField, Range(0, 1)]
    public float SFXValue;
    
    void Awake()
    {
        _uniqueInstance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        
    }

    public void Init()
    {

        string[] soundName = System.Enum.GetNames(typeof(eSound));
        //for(int i = 0; i < soundName.Length -1; i++)
        //{
        //    GameObject go = new GameObject { name = soundName[i] };
        //    AudioSource source = go.AddComponent<AudioSource>();
        //    go.transform.parent = transform;
        //    list_Sources.Add(source);
        //}


        for (int i = 0; i < soundName.Length - 1; i++)
        {
            GameObject go = new GameObject { name = soundName[i] };
            audioSources[i] = go.AddComponent<AudioSource>();
            go.transform.parent = transform;
        }

        audioSources[(int)eSound.BGM].loop = true;

    }

    public void Clear()
    {
        if (audioSources != null)
        {
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.clip = null;
                audioSource.Stop();
            }
        }

        if (_audioClips != null)
        {
            _audioClips.Clear();
        }
    }

    public void Play(eSoundList Key, eSound type = eSound.SFX, bool isLoop = false)
    {
        AudioClip audioClip = GetOrAddAudioClip(Key.ToString(), type);
       
        Play(audioClip, type, isLoop);
    }

    public void Play(AudioClip audioClip, eSound type = eSound.SFX, bool isLoop = false)
    {
        if (audioClip == null)
            return;

        if (type == eSound.BGM)
        {

            AudioSource audioSource = audioSources[(int)eSound.BGM];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.clip = audioClip;
            audioSource.volume = BGMValue;
            audioSource.Play();


        }
        else
        {
            AudioSource audioSource = audioSources[(int)eSound.SFX];
            audioSource.volume = SFXValue;
            if (isLoop)
            {
                audioSource.clip = audioClip;
                audioSource.loop = true;
                audioSource.Play();
            }
            else
            {
                audioSource.loop = false;
                audioSource.PlayOneShot(audioClip);
            }
            
        }

    }

    public void StopAudio(eSound type = eSound.SFX)
    {
        AudioSource audioSource = audioSources[(int)eSound.SFX];
        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    public AudioClip GetOrAddAudioClip(eSoundList Key, eSound type = eSound.SFX)
    {
        return GetOrAddAudioClip(Key.ToString(), type);
    }

    public AudioClip GetOrAddAudioClip(string Key, eSound type = eSound.SFX)
    {
        AudioClip audioClip = null;

        if (type == eSound.BGM)
        {
            audioClip = SearchSound(Key);
        }
        else
        {

            if (_audioClips.TryGetValue(Key, out audioClip) == false)
            {
                audioClip = SearchSound(Key);
                _audioClips.Add(Key, audioClip);
            }

        }

        if (audioClip == null)
        {
            //Debug.Log($"AudioClip Missing {Key}");
        }

        return audioClip;
    }


    AudioClip SearchSound(string Key)
    {
        AudioClip temp = null;
        if(_audios != null)
        {
            for(int i = 0; i < _audios.Count; i++)
            {
                if(_audios[i].Key == Key)
                {
                    temp = _audios[i].clip;
                    break;
                }
            }
        }
        return temp;
    }
}
