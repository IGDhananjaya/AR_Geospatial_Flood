using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class RainInfoManager_RealData : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text rainInfoText;   // 🔹 gabungan dari title + rain category
    public TMP_Text demText;
    public TMP_Text ndviText;
    public TMP_Text ndwiText;
    public TMP_Text tpiText;
    public TMP_Text floodRiskText;

    [Header("API Settings")]
    [Tooltip("Contoh: https://flood-api-xxxx.a.run.app/data")]
    public string apiUrl = "https://flood-api-584316128379.asia-southeast1.run.app/data";
    public float refreshInterval = 60f; // waktu update data

    [Header("References")]
    public summonSmooth floodSimulator;
    public RainDropdownHandlerSmooth rainDropdownHandler;
    public UIManager uiManager;
    public FloodVisualizerHybrid floodVisualizer;

    private float latitude = -8.1153f;
    private float longitude = 115.0884f;
    private bool locationReady = false;
    private bool isFetching = false;

    private void Start()
    {
        Debug.Log("[RainInfo] 🌦️ Initializing location...");
        ShowWaitingMessage();
        StartCoroutine(InitializeLocation());
    }

    // ==========================================================
    // 📍 GPS Initialization + Auto Retry
    // ==========================================================
    private IEnumerator InitializeLocation()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogWarning("[RainInfo] ⚠️ GPS disabled by user. Using default coordinates.");
            locationReady = true;
            StartCoroutine(FetchRainInfo());
            StartCoroutine(UpdateRainInfoLoop());
            yield break;
        }

        Input.location.Start();
        int maxWait = 10;

        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            Debug.Log($"[RainInfo] ⏳ Waiting for GPS... ({maxWait}s)");
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        Debug.Log($"[RainInfo] GPS Status: {Input.location.status}");

        if (Input.location.status == LocationServiceStatus.Running)
        {
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            Debug.Log($"[RainInfo] ✅ Location obtained: lat={latitude}, lon={longitude}");

            locationReady = true;
            StartCoroutine(FetchRainInfo());
            StartCoroutine(UpdateRainInfoLoop());
        }
        else
        {
            Debug.LogWarning("[RainInfo] ❌ GPS failed. Retrying in 5 seconds...");
            yield return new WaitForSeconds(5);
            StartCoroutine(InitializeLocation());
        }

        Input.location.Stop();
    }

    // ==========================================================
    // 🔁 Update Loop
    // ==========================================================
    private IEnumerator UpdateRainInfoLoop()
    {
        while (locationReady)
        {
            yield return new WaitForSeconds(refreshInterval);
            StartCoroutine(FetchRainInfo());
        }
    }

    // ==========================================================
    // 🌧️ Fetch Data dari API
    // ==========================================================
    private IEnumerator FetchRainInfo()
    {
        if (isFetching) yield break;
        isFetching = true;

        string url = string.Format(System.Globalization.CultureInfo.InvariantCulture,
            "{0}?lat={1}&lon={2}", apiUrl, latitude, longitude);

        Debug.Log($"[RainInfo] 🌍 Requesting API ({System.DateTime.Now:T}) → {url}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            Debug.Log($"[RainInfo] 📥 API Response ({System.DateTime.Now:T}) | Status: {request.result}");

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[RainInfo] ❌ Failed: {request.error} | Code: {request.responseCode}");
                isFetching = false;
                yield break;
            }

            string json = request.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("[RainInfo] ⚠️ Empty JSON response.");
                isFetching = false;
                yield break;
            }

            RainDataRoot dataRoot = null;
            try
            {
                dataRoot = JsonUtility.FromJson<RainDataRoot>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[RainInfo] ❌ JSON parsing error: {e.Message}");
                isFetching = false;
                yield break;
            }

            if (dataRoot == null)
            {
                Debug.LogWarning("[RainInfo] ⚠️ dataRoot is null after parsing.");
                isFetching = false;
                yield break;
            }

            HideWaitingMessage();
            UpdatePanel(dataRoot);

            // Activate dropdown and sync with other components
            uiManager?.UpdateFloodDropdownState(dataRoot.flood_risk, true);
            uiManager?.ShowDropdownImmediate();

            rainDropdownHandler?.SyncWithRainData(dataRoot.rain_category, dataRoot.flood_risk);

            if (floodVisualizer != null && dataRoot.flood_zones != null)
            {
                List<FloodVisualizerHybrid.FloodZone> zones = new List<FloodVisualizerHybrid.FloodZone>();
                foreach (FloodZoneData z in dataRoot.flood_zones)
                {
                    zones.Add(new FloodVisualizerHybrid.FloodZone
                    {
                        lat = z.lat,
                        lon = z.lon,
                        risk = z.risk
                    });
                }
                floodVisualizer.UpdateFloodZones(zones);
            }

            // Run flood animation
            floodSimulator?.gameObject.SetActive(true);
            floodSimulator?.SetFloodIntensity(dataRoot.rain_category);
            Debug.Log($"[RainInfo] 💧 Flood animation synced ({dataRoot.rain_category}/{dataRoot.flood_risk})");

            isFetching = false;
        }
    }

    // ==========================================================
    // 🧩 UI Update
    // ==========================================================
    private void UpdatePanel(RainDataFull data)
    {
        uiManager?.SetFloodAreaState(true);

        Color riskColor = GetRiskColor(data.flood_risk);

        // 🔹 Combine title + category + data rain
        if (rainInfoText != null)
            rainInfoText.text = $"Rainfall Intensity (6 hr avg): {data.rain_category} {data.rain:F1} mm/day";

        // 🔹 Other info
        if (demText != null) demText.text = $"DEM: {data.dem:F1} m";
        if (ndviText != null) ndviText.text = $"NDVI: {data.ndvi:F3}";
        if (ndwiText != null) ndwiText.text = $"NDWI: {data.ndwi:F3}";
        if (tpiText != null) tpiText.text = $"TPI: {data.tpi:F2}";
        if (floodRiskText != null)
        {
            floodRiskText.text = $"Flood Risk: {data.flood_risk}";
            floodRiskText.color = riskColor;
        }
    }

    // ==========================================================
    // 🕒 UI State Handling
    // ==========================================================
    private void ShowWaitingMessage()
    {
        if (rainInfoText != null) rainInfoText.text = "⏳ Loading rainfall data...";
        if (demText != null) demText.text = "-";
        if (ndviText != null) ndviText.text = "-";
        if (ndwiText != null) ndwiText.text = "-";
        if (tpiText != null) tpiText.text = "-";
        if (floodRiskText != null) floodRiskText.text = "Flood Risk: -";
    }

    private void HideWaitingMessage() => Debug.Log("[RainInfo] ✅ Data received, panel updated.");

    private Color GetRiskColor(string risk)
    {
        switch (risk)
        {
            case "High": return Color.red;
            case "Medium": return new Color(1f, 0.65f, 0f);
            case "Low": return Color.green;
            default: return Color.white;
        }
    }
}

// ==========================================================
// 📦 JSON Data Structures
// ==========================================================
[System.Serializable]
public class RainDataFull
{
    public float dem;
    public string flood_risk;
    public float lat;
    public float lon;
    public float ndvi;
    public float ndwi;
    public float rain;
    public string rain_category;
    public string timestamp;
    public float tpi;
}

[System.Serializable]
public class FloodZoneData
{
    public float lat;
    public float lon;
    public string risk;
}

[System.Serializable]
public class RainDataRoot : RainDataFull
{
    public List<FloodZoneData> flood_zones;
}
