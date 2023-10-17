using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class SoundManager : MonoBehaviour
{
    AudioSource[] audioSources = new AudioSource[(int)eSound.Max_Cnt];
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    [SerializeField]List<Audios> _audios;

    [System.Serializable]
    class Audios
    {
        public string name;
        public AudioClip clip;
    }

    static SoundManager _uniqueInstance;
    public static SoundManager _inst { get {return _uniqueInstance; } }

    public Dictionary<string, AudioClip> Clips { get { return _audioClips; } }

    void Awake()
    {
        _uniqueInstance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Init();
    }

    

    public void Init()
    {

        string[] soundName = System.Enum.GetNames(typeof(eSound));
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

    public void Play(string path, eSound type = eSound.SFX, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
    }

    public void Play(AudioClip audioClip, eSound type = eSound.SFX, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == eSound.BGM)
        {

            AudioSource audioSource = audioSources[(int)eSound.BGM];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();


        }
        else
        {
            AudioSource audioSource = audioSources[(int)eSound.SFX];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    AudioClip GetOrAddAudioClip(string path, eSound type = eSound.SFX)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";

        AudioClip audioClip = null;

        if (type == eSound.BGM)
        {
            audioClip = Managers._resource.Load<AudioClip>(path);
        }
        else
        {

            if (_audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Managers._resource.Load<AudioClip>(path);
                _audioClips.Add(path, audioClip);
            }

        }

        if (audioClip == null)
        {
            Debug.Log($"AudioClip Missing {path}");
        }

        return audioClip;
    }
}
