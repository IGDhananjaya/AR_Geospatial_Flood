using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Google.XR.ARCoreExtensions; // untuk ARGeospatialAnchor
using UnityEngine.XR.ARFoundation;

public class ArrowSpawner : MonoBehaviour
{
    [Header("AR Components")]
    [SerializeField] private ARAnchorManager anchorManager;
    [SerializeField] private GameObject arrowPrefab;

    [Header("CSV Settings")]
    public string fileName = "route.csv"; // nama file di StreamingAssets
    public float spacing = 2f; // jarak antar panah dalam meter

    private List<ARGeospatialAnchor> spawnedAnchors = new List<ARGeospatialAnchor>();

    void Start()
    {
        LoadAndSpawn();
    }

    private void LoadAndSpawn()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"CSV file tidak ditemukan di {filePath}");
            return;
        }

        string[] lines = File.ReadAllLines(filePath);

        List<(double lat, double lng, double alt)> geoPoints = new List<(double, double, double)>();

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] values = line.Split(',');
            if (values.Length < 2) continue;

            double lat = double.Parse(values[0]);
            double lng = double.Parse(values[1]);
            double alt = (values.Length > 2) ? double.Parse(values[2]) : 0; // opsional alt

            geoPoints.Add((lat, lng, alt));
        }

        Debug.Log($"Loaded {geoPoints.Count} points dari CSV.");

        // Spawn anchors dan arrows
        for (int i = 0; i < geoPoints.Count; i++)
        {
            var point = geoPoints[i];

            ARGeospatialAnchor anchor = anchorManager.AddAnchor(
                point.lat,
                point.lng,
                point.alt,
                Quaternion.identity);

            if (anchor != null)
            {
                spawnedAnchors.Add(anchor);

                GameObject arrow = Instantiate(arrowPrefab, anchor.transform);

                // Rotasi panah ke titik berikutnya
                if (i < geoPoints.Count - 1)
                {
                    Vector3 dir = (Vector3.forward); // default arah
                    arrow.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
                }
            }
            else
            {
                Debug.LogWarning($"Gagal membuat anchor di {point.lat}, {point.lng}");
            }
        }
    }

    public void ClearArrows()
    {
        foreach (var anchor in spawnedAnchors)
        {
            if (anchor != null) Destroy(anchor.gameObject);
        }
        spawnedAnchors.Clear();
    }
}
