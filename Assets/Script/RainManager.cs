using UnityEngine;

public class RainManager : MonoBehaviour
{
    public static RainManager Instance;

    public string currentRainIntensity = "low"; // default awal

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // supaya tetap hidup antar scene (kalau perlu)
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetRainIntensity(string newIntensity)
    {
        currentRainIntensity = newIntensity;
    }

    public string GetRainIntensity()
    {
        return currentRainIntensity;
    }
}