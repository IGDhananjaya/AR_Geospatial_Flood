using UnityEngine;
using TMPro; // pakai TMP namespace

public class RainDropdownHandlerSmooth_TMP : MonoBehaviour
{
    [Header("References")]
    public TMP_Dropdown rainDropdown;        // TMP Dropdown UI
    public summonSmooth floodSimulator;      // Script animasi banjir

    private bool isManualMode = false;

    private void Start()
    {
        if (rainDropdown != null)
            rainDropdown.onValueChanged.AddListener(delegate { OnRainIntensityChanged(); });

        // Aktifkan dropdown saat pengguna di area banjir
        SetDropdownActive(true);

        // Sinkronisasi awal dari RainManager (opsional)
        if (RainManager.Instance != null)
        {
            string initialRain = RainManager.Instance.GetRainIntensity();
            SetDropdownValue(initialRain);
            Debug.Log($"[RainDropdownHandlerSmooth_TMP] Initialized from RainManager: {initialRain}");
        }
    }

    /// <summary>
    /// Dipanggil dari RainInfoManager untuk sinkronisasi otomatis data real API.
    /// </summary>
    public void SyncWithRainData(string rainCategory, string floodRisk)
    {
        // Abaikan kontrol dari API agar dropdown tetap manual
        Debug.Log($"[RainDropdownHandlerSmooth_TMP] (Ignored) Sync request from API: {rainCategory}/{floodRisk}");
    }

    public void OnRainIntensityChanged()
    {
        if (!isManualMode)
        {
            Debug.Log("[RainDropdownHandlerSmooth_TMP] Manual mode tidak aktif, abaikan input user.");
            return;
        }

        if (rainDropdown == null || floodSimulator == null)
        {
            Debug.LogWarning("[RainDropdownHandlerSmooth_TMP] Referensi belum diisi di Inspector.");
            return;
        }

        string selected = rainDropdown.options[rainDropdown.value].text.Trim();
        Debug.Log($"[RainDropdownHandlerSmooth_TMP] Manual override selected: {selected}");

        // Jalankan animasi sesuai pilihan dropdown
        switch (selected.ToLower())
        {
            case "low":
                floodSimulator.SetFloodIntensity("Low");
                break;
            case "medium":
                floodSimulator.SetFloodIntensity("Medium");
                break;
            case "high":
                floodSimulator.SetFloodIntensity("High");
                break;
            default:
                floodSimulator.StopSummon(); // hentikan animasi jika tidak cocok
                break;
        }

        UpdateDropdownColor(rainDropdown.value);
    }

    public void SetDropdownActive(bool active)
    {
        isManualMode = active;

        if (rainDropdown != null)
        {
            rainDropdown.interactable = active;

            // Ubah warna agar visualnya jelas aktif / nonaktif
            var image = rainDropdown.captionImage;
            if (image != null)
                image.color = active ? Color.white : new Color(1f, 1f, 1f, 0.5f);

            Debug.Log($"[RainDropdownHandlerSmooth_TMP] Manual override {(active ? "enabled" : "disabled")}");
        }
    }

    private void SetDropdownValue(string rainIntensity)
    {
        if (rainDropdown == null) return;

        string normalized = rainIntensity.ToLower().Trim();
        for (int i = 0; i < rainDropdown.options.Count; i++)
        {
            string option = rainDropdown.options[i].text.ToLower().Trim();
            if (option.Contains(normalized))
            {
                rainDropdown.value = i;
                UpdateDropdownColor(i);
                break;
            }
        }
    }

    /// <summary>
    /// Warna dropdown sesuai intensitas hujan
    /// </summary>
    private void UpdateDropdownColor(int index)
    {
        string selected = rainDropdown.options[index].text.ToLower();
        Color color = Color.white;

        switch (selected)
        {
            case "low":
                color = new Color(0.6f, 1f, 0.6f); // hijau muda
                break;
            case "medium":
                color = new Color(1f, 1f, 0.6f);   // kuning muda
                break;
            case "high":
                color = new Color(1f, 0.6f, 0.6f); // merah muda
                break;
        }

        // ubah tampilan utama
        if (rainDropdown.captionImage != null)
            rainDropdown.captionImage.color = color;
    }
}
