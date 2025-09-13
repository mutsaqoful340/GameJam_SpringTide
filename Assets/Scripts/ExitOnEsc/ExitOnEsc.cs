using UnityEngine;

public class ExitOnEsc : MonoBehaviour
{
    void Update()
    {
        // cek kalau ESC ditekan
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();

#if UNITY_EDITOR
            // biar saat di editor juga "keluar" (stop playmode)
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
