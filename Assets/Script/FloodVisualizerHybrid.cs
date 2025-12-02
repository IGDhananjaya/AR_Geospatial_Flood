using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Menampilkan area banjir berdasarkan risiko (Low / Medium / High / VeryHigh)
/// dari data flood_zones yang dikirim oleh RainInfoManager_RealData.
/// </summary>
public class FloodVisualizerHybrid : MonoBehaviour
{
    [Header("Flood Visualization Settings")]
    [Tooltip("Prefab kecil (sphere/plane) untuk mewakili titik area banjir")]
    public GameObject floodMarkerPrefab;

    [Tooltip("Ukuran marker (meter)")]
    public float markerScale = 3f;

    [Tooltip("Waktu transisi perubahan warna (smooth)")]
    public float colorTransitionSpeed = 2f;

    [Header("Color per Flood Risk")]
    public Color lowRiskColor = new Color(0.2f, 0.6f, 1f, 0.3f);    // biru muda
    public Color mediumRiskColor = new Color(1f, 0.65f, 0f, 0.5f); // oranye
    public Color highRiskColor = new Color(1f, 0f, 0f, 0.6f);      // merah
    public Color veryHighRiskColor = new Color(0.6f, 0f, 1f, 0.7f); // ungu

    [Header("Auto Update Settings")]
    public bool autoRefresh = false;
    public float refreshInterval = 60f;

    [Header("Positioning Reference")]
    [Tooltip("Posisi user (biasanya AR Camera) untuk acuan visualisasi")]
    public Transform player;

    private List<GameObject> activeMarkers = new List<GameObject>();
    private float timer;

    [System.Serializable]
    public class FloodZone
    {
        public float lat;
        public float lon;
        public string risk;
    }

    // === Dipanggil oleh RainInfoManager_RealData ===
    public void UpdateFloodZones(List<FloodZone> zones)
    {
        if (zones == null || zones.Count == 0)
        {
            Debug.LogWarning("[FloodVisualizerHybrid] Tidak ada zona banjir dari API.");
            return;
        }

        ClearFloodMarkers();

        foreach (FloodZone zone in zones)
        {
            Vector3 worldPos = LatLonToWorld(zone.lat, zone.lon);
            if (float.IsNaN(worldPos.x) || float.IsNaN(worldPos.z)) continue;

            GameObject marker = Instantiate(floodMarkerPrefab, worldPos, Quaternion.identity, transform);
            marker.transform.localScale = Vector3.one * markerScale;

            Renderer renderer = marker.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = GetColorForRisk(zone.risk);
            }

            activeMarkers.Add(marker);
        }

        Debug.Log($"[FloodVisualizerHybrid] {activeMarkers.Count} zona banjir diperbarui.");
    }

    private void Update()
    {
        if (autoRefresh)
        {
            timer += Time.deltaTime;
            if (timer >= refreshInterval)
            {
                timer = 0f;
                Debug.Log("[FloodVisualizerHybrid] Refresh flood zones (auto mode).");
                // TODO: Bisa memanggil ulang RainInfoManager_RealData di sini jika dibutuhkan
            }
        }

        // Optional: efek transisi warna halus (smoothing)
        foreach (var marker in activeMarkers)
        {
            if (marker == null) continue;
            Renderer renderer = marker.GetComponent<Renderer>();
            if (renderer == null) continue;

            Color target = renderer.material.color;
            renderer.material.color = Color.Lerp(renderer.material.color, target, Time.deltaTime * colorTransitionSpeed);
        }
    }

    private Color GetColorForRisk(string risk)
    {
        switch (risk)
        {
            case "Medium": return mediumRiskColor;
            case "High": return highRiskColor;
            case "VeryHigh": return veryHighRiskColor;
            default: return lowRiskColor;
        }
    }

    private void ClearFloodMarkers()
    {
        foreach (GameObject obj in activeMarkers)
        {
            if (obj != null)
                Destroy(obj);
        }
        activeMarkers.Clear();
    }

    /// <summary>
    /// Konversi dari lat/lon ke posisi dunia Unity relatif terhadap posisi pemain.
    /// </summary>
    private Vector3 LatLonToWorld(float lat, float lon)
    {
        if (player == null)
        {
            Debug.LogWarning("[FloodVisualizerHybrid] Player transform belum diatur!");
            return Vector3.zero;
        }

        // Perbedaan posisi relatif terhadap lokasi user
        float deltaLat = (lat - 0) * 111000f; // 1 derajat lat ~ 111 km
        float deltaLon = (lon - 0) * 111000f * Mathf.Cos(lat * Mathf.Deg2Rad);

        // Proyeksikan sekitar posisi user
        Vector3 offset = new Vector3(deltaLon, 0, deltaLat);
        Vector3 basePos = player.position;
        Vector3 worldPos = basePos + offset * 0.001f; // skala kecil agar muat di scene

        return worldPos;
    }

    // Visual debug di editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        foreach (GameObject marker in activeMarkers)
        {
            if (marker != null)
                Gizmos.DrawWireSphere(marker.transform.position, markerScale);
        }
    }
}
