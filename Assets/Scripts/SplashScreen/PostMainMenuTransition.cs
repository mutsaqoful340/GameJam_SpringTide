using UnityEngine;
using UnityEngine.SceneManagement;

public class PostMainMenuTransition : MonoBehaviour
{
    void Update()
    {
        SceneManager.LoadScene("PengenalanCerita");
    }
}
