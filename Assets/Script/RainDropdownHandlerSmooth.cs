using UnityEngine;
using UnityEngine.UI;

public class RainDropdownHandlerSmooth : MonoBehaviour
{
    [Header("References")]
    public Dropdown rainDropdown;           // Dropdown UI
    public summonSmooth floodSimulator;     // Script animasi banjir

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
            Debug.Log($"[RainDropdownHandlerSmooth] Initialized from RainManager: {initialRain}");
        }
    }

    /// <summary>
    /// Dipanggil dari RainInfoManager untuk sinkronisasi otomatis data real API.
    /// </summary>
    public void SyncWithRainData(string rainCategory, string floodRisk)
    {
        // Abaikan kontrol dari API agar dropdown tetap manual
        Debug.Log($"[RainDropdownHandlerSmooth] (Ignored) Sync request from API: {rainCategory}/{floodRisk}");
    }

    public void OnRainIntensityChanged()
    {
        if (!isManualMode)
        {
            Debug.Log("[RainDropdownHandlerSmooth] Manual mode tidak aktif, abaikan input user.");
            return;
        }

        if (rainDropdown == null || floodSimulator == null)
        {
            Debug.LogWarning("[RainDropdownHandlerSmooth] Referensi belum diisi di Inspector.");
            return;
        }

        string selected = rainDropdown.options[rainDropdown.value].text.Trim();
        Debug.Log($"[RainDropdownHandlerSmooth] Manual override selected: {selected}");

        // Jalankan animasi sesuai pilihan dropdown
        switch (selected.ToLower())
        {
            case "light":
                floodSimulator.SetFloodIntensity("Light");
                break;
            case "medium":
                floodSimulator.SetFloodIntensity("Medium");
                break;
            case "heavy":
                floodSimulator.SetFloodIntensity("Heavy");
                break;
            case "very heavy":
            case "veryheavy":
                floodSimulator.SetFloodIntensity("VeryHeavy");
                break;
            default:
                floodSimulator.StopSummon(); // Gantikan ClearFlood()
                break;
        }
    }

    public void SetDropdownActive(bool active)
    {
        isManualMode = active;

        if (rainDropdown != null)
        {
            rainDropdown.interactable = active;
            // Ubah warna agar visualnya jelas aktif / nonaktif
            var dropdownImage = rainDropdown.GetComponent<Image>();
            if (dropdownImage != null)
                dropdownImage.color = active ? Color.white : new Color(1f, 1f, 1f, 0.5f);

            Debug.Log($"[RainDropdownHandlerSmooth] Manual override {(active ? "enabled" : "disabled")}");
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
                break;
            }
        }
    }
}
