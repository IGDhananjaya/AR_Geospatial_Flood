using UnityEngine;
using UnityEngine.XR.ARFoundation; // ⬅️ Untuk ARPlane
using System;

public class summon : MonoBehaviour
{
    public event Action OnFloodAnimationFinished;

    [Header("Durasi Animasi Naik (detik)")]
    public float riseDuration = 3f;
    public GameObject floodObject; // objek banjir (misalnya particle / plane dengan animasi)

    [Header("Ketinggian Air per Intensitas (cm)")]
    public float lightHeightCm = 5f;
    public float mediumHeightCm = 15f;
    public float heavyHeightCm = 40f;
    public float veryHeavyHeightCm = 80f;

    private Vector3 initialLocalPosition;
    private float targetRiseY;
    private float elapsedTime = 0f;
    private bool rising = false;

    void Start()
    {
        // Cari tanah/plane di bawah objek ini
        SetToGround();

        string currentIntensity = RainManager.Instance != null ? RainManager.Instance.GetRainIntensity() : "light";
        StartAnimation(currentIntensity);
    }

    void Update()
    {
        if (rising)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / riseDuration);

            float newY = Mathf.Lerp(initialLocalPosition.y, targetRiseY, t);
            transform.localPosition = new Vector3(initialLocalPosition.x, newY, initialLocalPosition.z);

            if (elapsedTime >= riseDuration)
            {
                rising = false;
                OnFloodAnimationFinished?.Invoke();
            }
        }
    }

    public void StartAnimation(string rainIntensity)
    {
        ResetAnimation();

        float riseHeight = 0f;
        switch (rainIntensity.ToLower())
        {
            case "light": riseHeight = lightHeightCm / 100f; break;
            case "medium": riseHeight = mediumHeightCm / 100f; break;
            case "heavy": riseHeight = heavyHeightCm / 100f; break;
            case "very heavy": riseHeight = veryHeavyHeightCm / 100f; break;
            default: riseHeight = 0f; break;
        }

        targetRiseY = initialLocalPosition.y + riseHeight;
        rising = true;
    }

    public void ResetAnimation()
    {
        elapsedTime = 0f;
        rising = false;
        transform.localPosition = initialLocalPosition;
    }

    private void SetToGround()
    {
        // Raycast ke bawah dari posisi awal
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 5f))
        {
            // Set posisi awal tepat di atas ground
            initialLocalPosition = transform.parent.InverseTransformPoint(hit.point);
            transform.localPosition = initialLocalPosition;
        }
        else
        {
            // fallback kalau tidak ada ground terdeteksi
            initialLocalPosition = transform.localPosition;
        }
    }

    public void StartSummon()
    {
        if (floodObject != null)
            floodObject.SetActive(true); // munculkan banjir
    }

    public void StopSummon()
    {
        if (floodObject != null)
            floodObject.SetActive(false); // sembunyikan banjir
    }
}
