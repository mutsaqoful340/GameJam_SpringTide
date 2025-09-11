using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public static SFXPlayer Instance;
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySFX(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
