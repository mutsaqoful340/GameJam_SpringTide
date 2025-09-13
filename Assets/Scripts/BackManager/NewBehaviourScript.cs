// BackToPreviousScene.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToPreviousScene : MonoBehaviour
{
    // Panggil method ini dari OnClick button
    public void Back()
    {
        int current = SceneManager.GetActiveScene().buildIndex;
        int previous = current - 1;

        if (previous >= 0)
        {
            SceneManager.LoadScene(previous);
        }
        else
        {
            // Jika tidak ada scene sebelumnya, bisa kembali ke scene index 0
            // atau keluar aplikasi. Pilih salah satu sesuai kebutuhan:
            SceneManager.LoadScene(0);
            // Atau: Application.Quit();
        }
    }

    // Optional: method untuk load by name (jika mau pakai nama)
    public void BackTo(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
