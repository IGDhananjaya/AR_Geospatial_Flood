using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Background Audio")]
    public AudioSource bgSource;
    public AudioClip bgClip;

    [Header("Flood Audio")]
    public AudioSource floodSource;
    public AudioClip floodClip;

    [Header("UI Audio")]
    public AudioSource uiClickSource;
    public AudioClip uiClickClip;

    void Awake()
    {
        // Singleton pattern agar hanya ada satu AudioManager sepanjang permainan
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Mainkan musik latar secara otomatis saat aplikasi dibuka
        PlayBackground();
    }

    // ===== BACKGROUND MUSIC =====
    public void PlayBackground()
    {
        if (bgSource != null && bgClip != null)
        {
            bgSource.clip = bgClip;
            bgSource.loop = true;
            if (!bgSource.isPlaying)
                bgSource.Play();
        }
    }

    public void StopBackground()
    {
        if (bgSource != null && bgSource.isPlaying)
        {
            bgSource.Stop();
        }
    }

    // ===== FLOOD SOUND =====
    public void PlayFloodSound()
    {
        if (floodSource != null && floodClip != null)
        {
            floodSource.clip = floodClip;
            floodSource.loop = true;
            if (!floodSource.isPlaying)
                floodSource.Play();
        }
    }

    public void StopFloodSound()
    {
        if (floodSource != null && floodSource.isPlaying)
        {
            floodSource.Stop();
        }
    }

    // ===== UI CLICK SOUND =====
    public void PlayUIClick()
    {
        if (uiClickSource != null && uiClickClip != null)
        {
            uiClickSource.PlayOneShot(uiClickClip);
        }
    }
}
