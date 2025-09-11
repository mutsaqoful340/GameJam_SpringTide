using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    public static BGMPlayer Instance;
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayBGM(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopBGM()
    {
        audioSource.Stop();
    }
}
