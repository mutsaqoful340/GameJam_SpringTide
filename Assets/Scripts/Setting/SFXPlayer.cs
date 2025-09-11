using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public static SFXPlayer Instance;
    private AudioSource audioSource;

    [Header("Default Clips")]
    public AudioClip clickSFX; // 🎵 default click

    void Awake()
    {
        if (Instance == null) Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    // shortcut buat click button
    public void PlayClick()
    {
        PlaySFX(clickSFX);
    }
}
