using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SessionTransition : MonoBehaviour
{
    [Header("UI Settings")]
    public CanvasGroup textCanvas; // drag text / panel ke sini
    public float fadeDuration = 1f; // durasi fade out

    [Header("Scene Settings")]
    public string nextSceneName = "NextScene"; // ganti dengan nama scene tujuan
    public float waitBeforeFade = 2f; // waktu tunggu sebelum fade out

    public void StartTransition()
    {
        StartCoroutine(TransitionFlow());
    }

    IEnumerator TransitionFlow()
    {
        // Tunggu sebelum mulai fade
        yield return new WaitForSeconds(waitBeforeFade);

        // Fade out text
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            textCanvas.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }
        textCanvas.alpha = 0f;

        // Pindah scene
        SceneManager.LoadScene(nextSceneName);
    }
}
