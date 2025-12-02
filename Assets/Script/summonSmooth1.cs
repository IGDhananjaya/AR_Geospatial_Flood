using UnityEngine;
using System;

public class summonSmooth1 : MonoBehaviour
{
    public event Action OnFloodAnimationFinished;

    [Header("Durasi Animasi Naik (detik)")]
    public float riseDuration = 3f;

    [Header("Objek Flood (Air/Plane)")]
    public GameObject floodObject;

    [Header("Ketinggian Air per Intensitas (cm)")]
    public float lightHeightCm = 3f;
    public float mediumHeightCm = 15f;
    public float heavyHeightCm = 40f;
    public float veryHeavyHeightCm = 80f;

    [Header("Efek Halus")]
    [Range(0f, 1f)] public float smoothness = 0.15f;

    [Header("Offset Manual dari Editor (meter)")]
    [Tooltip("Gunakan ini jika kamu ingin posisi banjir terlihat lebih rendah dari aslinya.")]
    public float heightOffset = 0f;

    private Vector3 initialLocalPosition;
    private float currentY;
    private float targetY;
    private bool rising = false;
    private bool initialized = false;

    void Start()
    {
        SetToGround();
        initialLocalPosition = transform.localPosition + Vector3.up * heightOffset;
        currentY = initialLocalPosition.y;
        targetY = initialLocalPosition.y + (lightHeightCm / 100f); // mulai dari banjir ringan

        if (floodObject != null)
            floodObject.SetActive(true); // selalu aktif

        initialized = true;
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

    // Ubah tinggi air dengan nilai meter
    public void UpdateFloodHeight(float heightMeters)
    {
        targetY = initialLocalPosition.y + heightMeters;
        rising = true;
    }

    public void SetFloodIntensity(string intensity)
    {
        float riseHeight = 0f;
        switch (intensity.ToLower())
        {
            case "light": riseHeight = lightHeightCm / 100f; break;
            case "medium": riseHeight = mediumHeightCm / 100f; break;
            case "heavy": riseHeight = heavyHeightCm / 100f; break;
            case "veryheavy": riseHeight = veryHeavyHeightCm / 100f; break;
            default: riseHeight = lightHeightCm / 100f; break;
        }
        UpdateFloodHeight(riseHeight);
    }

    private void SetToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 5f))
        {
            transform.localPosition = transform.parent.InverseTransformPoint(hit.point);
        }
    }

    public void StartSummon()
    {
        if (floodObject != null)
            floodObject.SetActive(true);
    }

    public void StopSummon()
    {
        if (floodObject != null)
            floodObject.SetActive(false);
    }
}
