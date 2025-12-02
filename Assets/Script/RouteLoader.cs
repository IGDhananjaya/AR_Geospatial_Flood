using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Newtonsoft.Json; // ⚡ Install Newtonsoft Json via Unity Package Manager (atau import DLL)

// Struktur GeoJSON
[System.Serializable]
public class GeoJsonFeatureCollection
{
    public string type;
    public List<GeoJsonFeature> features;
}

[System.Serializable]
public class GeoJsonFeature
{
    public string type;
    public GeoJsonGeometry geometry;
    public Dictionary<string, object> properties;
}

[System.Serializable]
public class GeoJsonGeometry
{
    public string type;
    public List<List<double>> coordinates; // Asumsi LineString
}

public class RouteLoader : MonoBehaviour
{
    [Header("Static Load (File Lokal)")]
    public TextAsset geoJsonFile;

    [Header("Dynamic Load (API)")]
    public string apiUrl;
    public bool useDynamicLoad = false;

    [Header("Output")]
    public List<Vector2> parsedCoords = new List<Vector2>();

    void Start()
    {
        if (useDynamicLoad)
            StartCoroutine(LoadFromApi(apiUrl));
        else
            LoadFromFile();
    }

    // Load dari file lokal
    public void LoadFromFile()
    {
        if (geoJsonFile == null)
        {
            Debug.LogError("GeoJSON file belum diassign di Inspector!");
            return;
        }

        ParseGeoJson(geoJsonFile.text);
    }

    // Load dari API
    IEnumerator LoadFromApi(string url)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Gagal fetch: " + req.error);
                yield break;
            }
            ParseGeoJson(req.downloadHandler.text);
        }
    }

    // Parse isi GeoJSON
    void ParseGeoJson(string jsonText)
    {
        parsedCoords.Clear();

        try
        {
            var fc = JsonConvert.DeserializeObject<GeoJsonFeatureCollection>(jsonText);
            if (fc.features == null || fc.features.Count == 0)
            {
                Debug.LogWarning("Tidak ada fitur di GeoJSON!");
                return;
            }

            foreach (var feature in fc.features)
            {
                if (feature.geometry != null && feature.geometry.type == "LineString")
                {
                    foreach (var coord in feature.geometry.coordinates)
                    {
                        if (coord.Count >= 2)
                        {
                            double lon = coord[0];
                            double lat = coord[1];
                            parsedCoords.Add(new Vector2((float)lat, (float)lon));
                        }
                    }
                }
            }

            Debug.Log($"Parsed {parsedCoords.Count} koordinat dari GeoJSON");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error parsing GeoJSON: " + e.Message);
        }
    }

    public List<Vector2> GetRoute()
    {
        return parsedCoords;
    }
}
