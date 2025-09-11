using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public float duration = 10f;
    public string nextSceneName;

    void Start()
    {
        Invoke("LoadNextScene", duration);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
