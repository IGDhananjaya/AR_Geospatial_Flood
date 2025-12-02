using System.Collections;
using System.Collections.Generic;
// using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Home : MonoBehaviour
{
    public void HomePage()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUIClick(); // efek klik
            AudioManager.Instance.PlayBackground();
        }

        SceneManager.LoadScene("SampleScene");
    }
}