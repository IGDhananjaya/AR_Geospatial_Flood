using UnityEngine;
using UnityEngine.UI;

public class RainDropdownHandler : MonoBehaviour
{
    public Dropdown rainDropdown;
    public summon floodSimulator;
    public RainInfoManager rainInfoManager;

    void Start()
    {
        rainDropdown.onValueChanged.AddListener(delegate { OnRainIntensityChanged(); });
        OnRainIntensityChanged(); // Panggil saat pertama tampil
    }

    public void OnRainIntensityChanged()
    {
        string selected = rainDropdown.options[rainDropdown.value].text;

        // Simpan ke RainManager (penting!)
        if (RainManager.Instance != null)
        {
            RainManager.Instance.SetRainIntensity(selected.ToLower());
        }

        // Jalankan animasi jika objek sudah aktif
        if (floodSimulator != null && floodSimulator.gameObject.activeInHierarchy)
        {
            floodSimulator.StartAnimation(selected);
        }

        // Update info UI
        if (rainInfoManager != null)
        {
            string cleaned = selected.Replace(" ", "").Replace("-", "").ToLower();
            switch (cleaned)
            {
                case "light":
                    rainInfoManager.SetRainInfo("Light");
                    break;
                case "medium":
                    rainInfoManager.SetRainInfo("Medium");
                    break;
                case "heavy":
                    rainInfoManager.SetRainInfo("Heavy");
                    break;
                case "veryheavy":
                    rainInfoManager.SetRainInfo("VeryHeavy");
                    break;
                default:
                    rainInfoManager.SetRainInfo("Light");
                    break;
            }
        }
    }
}