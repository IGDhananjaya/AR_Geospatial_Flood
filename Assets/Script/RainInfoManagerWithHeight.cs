using UnityEngine;
using TMPro;

public class RainInfoManagerWithHeight : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text titleText;
    public TMP_Text valueText;
    public TMP_Text warningText;

    [Header("Flood Info")]
    public TMP_Text floodHeightText; // teks tambahan untuk tinggi banjir

    private float currentFloodHeight = 0f;
    private string currentLevel = "Light";

    public void SetRainInfo(string level)
    {
        if (titleText == null || valueText == null || warningText == null)
        {
            Debug.LogWarning("[RainInfoManagerWithHeight] Missing TMP references");
            return;
        }

        currentLevel = level;

        // Aktifkan autosizing agar teks menyesuaikan ukuran panel
        titleText.enableAutoSizing = true;
        valueText.enableAutoSizing = true;
        warningText.enableAutoSizing = true;
        if (floodHeightText != null)
            floodHeightText.enableAutoSizing = true;

        switch (level)
        {
            case "Light":
                titleText.text = "Rainfall: Light";
                valueText.text = "< 7.5 mm / 6 hours\n< 10 mm / day";
                warningText.text = "Low risk. Minimal impact.";
                UpdateFloodHeight(0.05f);
                break;

            case "Medium":
                titleText.text = "Rainfall: Medium";
                valueText.text = "7.5 – 35 mm / 6 hours\n10 – 50 mm / day";
                warningText.text = "Potential puddles in low areas.";
                UpdateFloodHeight(0.15f);
                break;

            case "Heavy":
                titleText.text = "Rainfall: Heavy";
                valueText.text = "35 – 70 mm / 6 hours\n50 – 100 mm / day";
                warningText.text = "Urban flooding likely. Low-lying areas at risk.";
                UpdateFloodHeight(0.40f);
                break;

            case "VeryHeavy":
                titleText.text = "Rainfall: Very Heavy";
                valueText.text = "> 70 mm / 6 hours\n> 100 mm / day";
                warningText.text = "Severe flood risk. Evacuate if needed!";
                UpdateFloodHeight(0.80f);
                break;

            default:
                titleText.text = "Rainfall: Unknown";
                valueText.text = "-";
                warningText.text = "-";
                UpdateFloodHeight(0f);
                break;
        }
    }

    public void UpdateFloodHeight(float heightMeters)
    {
        currentFloodHeight = heightMeters;

        if (floodHeightText != null)
        {
            float cm = heightMeters * 100f;
            floodHeightText.text = $"Flood Height: {cm:F1} cm";
        }
    }

    public void SetCustomFloodHeight(float heightCm)
    {
        UpdateFloodHeight(heightCm / 100f);
    }

    public float GetCurrentFloodHeight()
    {
        return currentFloodHeight;
    }
}
