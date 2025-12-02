using UnityEngine;
using System;

public class summonSmooth : MonoBehaviour
{
    public event Action OnFloodAnimationFinished;

    [Header("Durasi Animasi Naik (detik)")]
    public float riseDuration = 3f;

    [Header("Objek Flood (opsional, jika berbeda dari objek ini)")]
    public GameObject floodObject;

    [Header("Ketinggian Air per Intensitas (cm)")]
    public float lowHeightCm = 0f;
    public float mediumHeightCm = 40f;
    public float highHeightCm = 80f;
    public float veryHeavyHeightCm = 80f;

    [Header("Efek Halus")]
    [Range(0f, 1f)] public float smoothness = 0.15f;

    [Header("Offset Manual (meter)")]
    [Tooltip("Untuk menurunkan posisi air agar tidak bertumpuk dengan permukaan AR")]
    public float heightOffset = 0f;

    private Vector3 initialLocalPosition;
    private float currentY;
    private float targetY;
    private bool rising = false;
    private bool initialized = false;

    void Start()
    {
        SetToGround();

        // ambil posisi lokal awal objek air (polyshape)
        initialLocalPosition = transform.localPosition + Vector3.up * heightOffset;
        currentY = initialLocalPosition.y;
        targetY = initialLocalPosition.y + (lowHeightCm / 100f);

        if (floodObject != null)
            floodObject.SetActive(true);

        initialized = true;
        Debug.Log($"[summonSmooth] Initialized at localY={initialLocalPosition.y:F2}");
    }

    void Update()
    {
        if (!initialized || !rising) return;

        currentY = Mathf.Lerp(currentY, targetY, Time.deltaTime / smoothness);
        transform.localPosition = new Vector3(initialLocalPosition.x, currentY, initialLocalPosition.z);

        if (Mathf.Abs(currentY - targetY) < 0.001f)
        {
            transform.localPosition = new Vector3(initialLocalPosition.x, targetY, initialLocalPosition.z);
            rising = false;
            OnFloodAnimationFinished?.Invoke();
        }
    }

    // Ubah tinggi air secara langsung (meter)
    public void UpdateFloodHeight(float heightMeters)
    {
        transform.localPosition = initialLocalPosition;
        currentY = initialLocalPosition.y;
        targetY = initialLocalPosition.y + heightMeters;
        rising = true;

        Debug.Log($"[summonSmooth] UpdateFloodHeight: {heightMeters:F2}m → targetY={targetY:F2}");
    }

    public void SetFloodIntensity(string intensity)
    {
        float riseHeight = 0f;
        switch (intensity.ToLower().Replace(" ", "").Replace("-", ""))
        {
            case "low": riseHeight = lowHeightCm / 100f; break;
            case "medium": riseHeight = mediumHeightCm / 100f; break;
            case "high": riseHeight = highHeightCm / 100f; break;
            default: riseHeight = lowHeightCm / 100f; break;
        }
        UpdateFloodHeight(riseHeight);
    }


    private void SetToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 5f))
        {
            transform.localPosition = transform.parent.InverseTransformPoint(hit.point);
            Debug.Log($"[summonSmooth] SetToGround hit at {hit.point.y:F2}");
        }
        else
        {
            Debug.LogWarning("[summonSmooth] No ground hit detected. Using current local position.");
        }
    }

    public void StartSummon()
    {
        if (floodObject != null)
            floodObject.SetActive(true);
        Debug.Log("[summonSmooth] Flood started manually.");
    }

    public void StopSummon()
    {
        if (floodObject != null)
            floodObject.SetActive(false);
        Debug.Log("[summonSmooth] Flood stopped manually.");
    }
}
