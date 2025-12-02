using UnityEngine;

public class FloodController : MonoBehaviour
{
    public ScanController scan;
    public float heightOffsetM = 0f; // kalau perlu tambah ketinggian

    void Awake()
    {
        gameObject.SetActive(false);
        scan.OnGeoReady += OnGeoReady;
    }
    void OnDestroy() { scan.OnGeoReady -= OnGeoReady; }

    void OnGeoReady(double lat, double lon, double alt)
    {
        // Karena kita pakai Geospatial Creator Anchor, WaterFlood sudah jadi child-nya.
        // Di sini cukup aktifkan. (Kalau perlu, sesuaikan Y dengan alt+offsetM).
        gameObject.SetActive(true);
    }
}
