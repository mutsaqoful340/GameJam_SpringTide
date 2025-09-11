using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSail : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("MAINMENU"); // ganti dengan nama scene game kamu
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
