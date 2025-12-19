using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SoundData
{
    public string soundName;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource loopFXSource;

    [Header("Sound Library (Drag ALL sounds here)")]
    public SoundData[] musicLibrary;
    public SoundData[] sfxLibrary;

    [Header("Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;

    // Dictionary for fast lookup (Optimization)
    private Dictionary<string, AudioClip> sfxDictionary;
    private Dictionary<string, AudioClip> musicDictionary;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitializeLibrary();
            
            // Auto create sources if empty
            if(musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();
            if(sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
            if(loopFXSource == null) loopFXSource = gameObject.AddComponent<AudioSource>();

            musicSource.loop = true;
            loopFXSource.loop = true;
            
            musicSource.loop = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Convert Array to Dictionary for performance
    private void InitializeLibrary()
    {
        sfxDictionary = new Dictionary<string, AudioClip>();
        musicDictionary = new Dictionary<string, AudioClip>();

        foreach (var sound in sfxLibrary)
            if(!sfxDictionary.ContainsKey(sound.soundName)) sfxDictionary.Add(sound.soundName, sound.clip);

        foreach (var sound in musicLibrary)
            if (!musicDictionary.ContainsKey(sound.soundName)) musicDictionary.Add(sound.soundName, sound.clip);
    }

    void Start()
    {
        PlayMusic("bgm2");
    }

    // --- PLAY BY NAME ---
    public void PlayMusic(string name)
    {
        if (musicDictionary.ContainsKey(name))
        {
            AudioClip clip = musicDictionary[name];
            if (musicSource.clip == clip) return; 

            musicSource.clip = clip;
            musicSource.volume = masterVolume;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        if (sfxDictionary.ContainsKey(name))
            sfxSource.PlayOneShot(sfxDictionary[name], masterVolume);
    }
    
    public void PlaySFXLoop(string name)
    {
        if (sfxDictionary.ContainsKey(name))
        {
            AudioClip clip = sfxDictionary[name];
            
            // Jangan restart kalau suaranya sudah sama dan sedang main
            if (loopFXSource.isPlaying && loopFXSource.clip == clip) return;

            loopFXSource.clip = clip;
            loopFXSource.volume = masterVolume;
            loopFXSource.Play();
        }
        else
        {
            Debug.LogWarning($"Loop SFX: {name} not found!");
        }
    }

    public void StopSFXLoop()
    {
        loopFXSource.Stop();
        loopFXSource.clip = null;
    }

    public void PlaySFXRandomPitch(string name)
    {
        if (sfxDictionary.ContainsKey(name))
        {
            float originalPitch = sfxSource.pitch;
            sfxSource.pitch = Random.Range(0.85f, 1.15f);
            sfxSource.PlayOneShot(sfxDictionary[name], masterVolume);
            sfxSource.pitch = originalPitch; 
        }
    }
}