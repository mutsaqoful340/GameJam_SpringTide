using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSail : MonoBehaviour
{
    [Header("SFX")]
    public AudioClip clickSFX; // drag clip klik ke sini lewat Inspector

    public void StartGame()
    {
        // mainkan sound effect klik
        if (SFXPlayer.Instance != null && clickSFX != null)
        {
            SFXPlayer.Instance.PlaySFX(clickSFX);
        }

        // lalu ganti scene
        SceneManager.LoadScene("MAINMENU");
    }

    public void QuitGame()
    {
        // mainkan sound effect klik
        if (SFXPlayer.Instance != null && clickSFX != null)
        {
            SFXPlayer.Instance.PlaySFX(clickSFX);
        }

        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
