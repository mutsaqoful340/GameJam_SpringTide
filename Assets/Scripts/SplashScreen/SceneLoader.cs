using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public SFXPlayer sfxPlayer; // drag dari inspector

    public void LoadScene(string sceneName)
    {
        // Play SFX klik
        if (sfxPlayer != null && sfxPlayer.clickSFX != null)
            sfxPlayer.PlaySFX(sfxPlayer.clickSFX);

        // Pindah scene
        SceneManager.LoadScene(sceneName);
    }
}
