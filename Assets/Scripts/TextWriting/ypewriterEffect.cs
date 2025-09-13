using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Settings")]
    public TextMeshProUGUI textEng;   // Drag TMP Text ke sini
    public CanvasGroup textCanvas;    // Drag panel/text yg ada CanvasGroup

    [Header("Dialogues per Sesi")]
    [TextArea] public string[] sentencesEng;  // dialog versi Inggris

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip[] audioClips; // dubbing per sesi

    [Header("Timing Settings")]
    public float[] textDelays;        // delay per sesi sebelum teks muncul
    public float fadeDuration = 1f;   // durasi fade in/out teks
    public float holdAfterAudio = 0.5f; // tahan teks setelah audio selesai

    [Header("Scene Settings")]
    public string nextSceneName = "NextScene"; // ganti dengan nama scene tujuan

    private int index = 0;

    void Start()
    {
        textCanvas.alpha = 0f; // pastikan teks awal transparan
        StartCoroutine(ShowSentence());
    }

    IEnumerator ShowSentence()
    {
        while (index < sentencesEng.Length)
        {
            textEng.text = sentencesEng[index];

            // === Mainkan audio dulu ===
            if (audioSource != null && index < audioClips.Length)
            {
                audioSource.Stop();
                audioSource.clip = audioClips[index];
                audioSource.Play();
            }

            // === Delay khusus untuk sesi ini ===
            float delay = (index < textDelays.Length) ? textDelays[index] : 0f;
            if (delay > 0f)
                yield return new WaitForSeconds(delay);

            // Fade in teks
            yield return StartCoroutine(FadeCanvas(0f, 1f, fadeDuration));

            // Tunggu audio selesai
            if (audioSource != null && audioSource.clip != null)
            {
                yield return new WaitWhile(() => audioSource.isPlaying);
            }

            // Tahan sebentar setelah audio selesai
            yield return new WaitForSeconds(holdAfterAudio);

            // Fade out teks
            yield return StartCoroutine(FadeCanvas(1f, 0f, fadeDuration));

            index++;
        }

        // === Setelah semua sesi selesai ===
        Debug.Log("Dialog selesai semua sesi.");

        // Delay 2-3 detik dengan layar hitam
        yield return new WaitForSeconds(2.5f); // bisa atur ke 2 atau 3 sesuai kebutuhan

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }


    IEnumerator FadeCanvas(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            textCanvas.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        textCanvas.alpha = to;
    }
}
