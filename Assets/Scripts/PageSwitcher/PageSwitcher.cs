using UnityEngine;

public class PageManager : MonoBehaviour
{
    public GameObject[] pages; // daftar halaman
    private int currentPage = 0;

    void Start()
    {
        ShowPage(0); // mulai dari page 0
    }

    public void ShowPage(int index)
    {
        if (index < 0 || index >= pages.Length) return;

        // Matikan semua dulu
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == index);
        }

        currentPage = index;
    }

    public void NextPage()
    {
        int next = currentPage + 1;
        if (next < pages.Length)
            ShowPage(next);
    }

    public void PrevPage()
    {
        int prev = currentPage - 1;
        if (prev >= 0)
            ShowPage(prev);
    }
}
