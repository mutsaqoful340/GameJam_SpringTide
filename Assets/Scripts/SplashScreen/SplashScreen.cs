using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup logoCanvasGroup;
    [SerializeField] private float logoDelay = 1f;     // delay sebelum logo muncul
    [SerializeField] private float fadeDuration = 1f;  // durasi fade in
    [SerializeField] private float stayDuration = 2f;  // lama logo stay
    [SerializeField] private string nextScene = "MainMenu";

    private void Start()
    {
        logoCanvasGroup.alpha = 0;
        StartCoroutine(PlaySplash());
    }

    private System.Collections.IEnumerator PlaySplash()
    {
        // Tunggu sebelum logo muncul
        yield return new WaitForSeconds(logoDelay);

        // Fade in logo
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            logoCanvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        // Tunggu logo stay
        yield return new WaitForSeconds(stayDuration);

        // Pindah scene berikutnya
        SceneManager.LoadScene(nextScene);
    }
}
