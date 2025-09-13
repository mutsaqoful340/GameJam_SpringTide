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

    [Header("Typing Settings")]
    public float delay = 0.05f; // kecepatan ketikan

    [Header("Scene Settings")]
    public string nextSceneName = "NextScene"; // ganti dengan nama scene tujuan
    public float waitBeforeFade = 2f; // tunggu sebelum fade out
    public float fadeDuration = 1f;   // durasi fade out

    private int index = 0;
    private Coroutine typingCoroutine;

    void Start()
    {
        ShowSentence(); // mulai sesi pertama
    }

    void ShowSentence()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentence());

        // play audio dubbing sesuai sesi
        if (audioSource != null && index < audioClips.Length)
        {
            audioSource.Stop();
            audioSource.clip = audioClips[index];
            audioSource.Play();
        }
    }

    IEnumerator TypeSentence()
    {
        textEng.text = "";

        string sentence = sentencesEng[index];

        // ketikan huruf demi huruf
        foreach (char c in sentence)
        {
            textEng.text += c;
            yield return new WaitForSeconds(delay);
        }

        // tunggu audio selesai kalau ada
        if (audioSource != null && audioSource.clip != null)
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
        }

        // pindah ke sesi berikutnya
        NextSentence();
    }

    void NextSentence()
    {
        if (index < sentencesEng.Length - 1)
        {
            index++;
            ShowSentence();
        }
        else
        {
            Debug.Log("Dialog selesai semua sesi.");
            StartCoroutine(EndDialogueAndTransition());
        }
    }

    IEnumerator EndDialogueAndTransition()
    {
        // Tunggu beberapa detik
        yield return new WaitForSeconds(waitBeforeFade);

        // Fade out teks
        if (textCanvas != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                textCanvas.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }
            textCanvas.alpha = 0f;
        }

        // Pindah scene
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
