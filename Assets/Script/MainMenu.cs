using System.Collections;
using System.Collections.Generic;
// using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void TombolKeluar()
    {
        Application.Quit();
        Debug.Log("Game Close");
    }

    public void Scan()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUIClick(); // bunyi klik tombol
            AudioManager.Instance.StopBackground(); // hentikan musik
        }

        SceneManager.LoadScene("ScanAR");
    }

    public void ScanARKampus()
    {
        SceneManager.LoadScene("ScanARKampus");
    }

    public void ScanARKampusShorted()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUIClick(); // bunyi klik tombol
            AudioManager.Instance.StopBackground(); // hentikan musik
        }

        SceneManager.LoadScene("ScanARKampusShorted");
    }

    public void ScanARKampusTitikShelter()
    {
        SceneManager.LoadScene("ScanARKampusTitikShelter");
    }

    public void ScanARRumah()
    {
        SceneManager.LoadScene("ScanARRumah");
    }

    public void ScanARRumahBanjir()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUIClick(); // bunyi klik tombol
            AudioManager.Instance.StopBackground(); // hentikan musik
        }

        SceneManager.LoadScene("ScanARRumahBanjir");
    }

    public void Guide()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUIClick();
            AudioManager.Instance.PlayBackground();
        }

        SceneManager.LoadScene("Guide");
    }

    public void About()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUIClick();
            AudioManager.Instance.PlayBackground();
        }

        SceneManager.LoadScene("About");
    }
}
