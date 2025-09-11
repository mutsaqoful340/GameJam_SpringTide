using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeBarController : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Setup")]
    public string exposedParam; // "SFXVolume" atau "BGMVolume"
    public Button minusButton;
    public Button plusButton;
    public Image[] bars; // masukkan urutan Bar1-Bar5
    public Sprite filledSprite;
    public Sprite emptySprite;

    private int currentLevel = 0; // default tengah (0-5)
    private int maxLevel = 5;

    private void Start()
    {
        // Load volume
        currentLevel = PlayerPrefs.GetInt(exposedParam, 3);
        ApplyVolume();

        // Button listener
        minusButton.onClick.AddListener(DecreaseVolume);
        plusButton.onClick.AddListener(IncreaseVolume);
    }

    public void IncreaseVolume()
    {
        Debug.Log("Tambah ditekan, level sekarang: " + currentLevel);
        if (currentLevel < maxLevel)
        {
            currentLevel++;
            SaveAndApply();
        }
    }


    public void DecreaseVolume()
    {
        if (currentLevel > 0)
        {
            currentLevel--;
            SaveAndApply();
        }
    }

    void SaveAndApply()
    {
        PlayerPrefs.SetInt(exposedParam, currentLevel);
        ApplyVolume();
    }

    void ApplyVolume()
    {
        for (int i = 0; i < bars.Length; i++)
        {
            bool isFilled = i < currentLevel;
            bars[i].sprite = isFilled ? filledSprite : emptySprite;

            // paksa refresh
            bars[i].SetNativeSize();
            bars[i].enabled = false;
            bars[i].enabled = true;
        }

        float volume = (float)currentLevel / maxLevel;
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat(exposedParam, dB);
    }

}
