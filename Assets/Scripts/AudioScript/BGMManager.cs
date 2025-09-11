using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

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
    public AudioClip level2BGM;
    public AudioClip creditBGM;

    private AudioSource audioSource;

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
        audioSource.loop = true;
        audioSource.playOnAwake = false;

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

        switch (sceneName)
        {
            case "MAINMENU":
                PlayBGM(menuBGM);
                break;
            case "Level1":
                PlayBGM(level1BGM);
                break;
            case "!TrialChamber":
                PlayBGM(level2BGM);
                break;
            case "Setting":
                PlayBGM(creditBGM);
                break;
            default:
                PlayBGM(defaultBGM);
                break;
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
