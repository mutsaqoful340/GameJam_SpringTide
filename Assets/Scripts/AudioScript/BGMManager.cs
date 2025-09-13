using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    public string exposedParam = "BGMVolume";

    [Header("Clips Per Scene")]
    public AudioClip defaultBGM;
    public AudioClip menuBGM;
    public AudioClip level1BGM;

    [Tooltip("Daftar musik untuk Level2 (random pilih salah satu)")]
    public List<AudioClip> level2BGMs;

    [Tooltip("Musik utama Level2 (dipastikan play)")]
    public AudioClip level2BGM;

    [Tooltip("Musik tambahan Level2 (akan diputar bareng kalau ada)")]
    public AudioClip level2ExtraBGM;

    public AudioClip creditBGM;

    private AudioSource audioSource;   // sumber utama
    private AudioSource audioSource2;  // sumber tambahan (buat ambience / ekstra)

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;

        // tambahan audioSource kedua
        audioSource2 = gameObject.AddComponent<AudioSource>();
        audioSource2.loop = true;
        audioSource2.playOnAwake = false;

        // langsung register listener
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;
        Debug.Log("Scene Loaded: " + sceneName);

        // stop extra BGM dulu biar gak nyampur antar scene
        audioSource2.Stop();
        audioSource2.clip = null;

        switch (sceneName)
        {
            case "MAINMENU":
                PlayBGM(menuBGM);
                break;

            case "Level1":
                PlayBGM(level1BGM);
                break;

            case "!TrialChamber":
                HandleLevel2BGM();
                break;

            case "Setting":
                PlayBGM(creditBGM);
                break;

            default:
                PlayBGM(defaultBGM);
                break;
        }
    }

    private void HandleLevel2BGM()
    {
        // pilih random dari list kalau ada
        if (level2BGMs != null && level2BGMs.Count > 0)
        {
            int randomIndex = Random.Range(0, level2BGMs.Count);
            PlayBGM(level2BGMs[randomIndex]);
        }
        else
        {
            PlayBGM(level2BGM);
        }

        // kalau ada extra BGM, putar bareng
        if (level2ExtraBGM != null)
        {
            audioSource2.clip = level2ExtraBGM;
            audioSource2.Play();
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) clip = defaultBGM;
        if (clip == null) return;

        if (audioSource.clip == clip) return; // biar gak restart ulang

        audioSource.clip = clip;
        audioSource.Play();
    }
}
