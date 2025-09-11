using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TypewriterEffect : MonoBehaviour
{
    [Header("UI Settings")]
    public TextMeshProUGUI textUI;
    [TextArea] public string fullText;
    public float delay = 0.05f;

    [Header("Audio Settings")]
    public AudioSource audioSource;

    [Header("Scene Settings")]
    public float duration = 10f; // durasi cutscene
    public string nextSceneName; // isi di Inspector

    void Start()
    {
        // Play audio kalau ada
        if (audioSource != null)
            audioSource.Play();

        // Jalankan teks dengan efek ketikan
        StartCoroutine(ShowText());

        // Panggil pindah scene setelah durasi
        Invoke(nameof(LoadNextScene), duration);
    }

    IEnumerator ShowText()
    {
        textUI.text = "";
        foreach (char c in fullText)
        {
            textUI.text += c;
            yield return new WaitForSeconds(delay);
        }
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
