using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        soundDictionary = new Dictionary<SoundID, AudioClip>();
        foreach (var mapping in soundMappings)
        {
            soundDictionary[mapping.id] = mapping.clip;
        }

        // --- 初始化音效池 ---
        for (int i = 0; i < sfxPoolSize; i++)
        {
            AudioSource source = Instantiate(sfxSourcePrefab, transform);
            sfxPool.Add(source);
        }
    }
    
    private GameManager gameManager;

    [Header("BGM Source")] 
    [SerializeField] private AudioSource musicSource;
    
    [Header("BGS Source")]
    [SerializeField] private AudioSource sfxSourcePrefab; 
    [SerializeField] private int sfxPoolSize = 10;
    private List<AudioSource> sfxPool = new List<AudioSource>();
    
    // 音频映射 only use for editor.
    [System.Serializable]
    public class SoundMapping
    {
        public SoundID id;
        public AudioClip clip;
    }
    
    [Header("音频列表")]
    [SerializeField] private List<SoundMapping> soundMappings;
    private Dictionary<SoundID, AudioClip> soundDictionary;

    void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.RegisterHandler("PlayMusic", PlayMusic);
        gameManager.RegisterHandler("PlaySFX", PlaySFX);
    }

    public object PlayMusic(params object[] args)
    {
        if (args.Length >= 1 && args[0] is SoundID musicID)
        {
            if (soundDictionary.TryGetValue(musicID, out AudioClip clip))
            {
                musicSource.clip = clip;
                musicSource.loop = true;
                musicSource.Play();
            }
        }
        return null;
    }

    public object PlaySFX(params object[] args)
    {
        if (args.Length >= 1 && args[0] is SoundID musicID)
        {
            if (soundDictionary.TryGetValue(musicID, out AudioClip clip))
            {
                AudioSource source = GetAvailableSfxSource();
                source.clip = clip; // 音乐通常是循环的
                source.Play();
            }
        }
        return null;
    }
    
    private AudioSource GetAvailableSfxSource()
    {
        foreach (var source in sfxPool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        // Force stop first one.
        return sfxPool[0];
    }
}

public enum SoundID
{
    // WriteDown.
    // 音乐音效名字写这里
}
