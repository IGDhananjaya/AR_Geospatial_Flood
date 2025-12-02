using UnityEngine;
using TMPro;

public class RainInfoManager : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text valueText;
    public TMP_Text warningText;

    public void SetRainInfo(string level)
    {
        titleText.enableAutoSizing = true;
        valueText.enableAutoSizing = true;
        warningText.enableAutoSizing = true;

        // Sesuai level yang diberikan
        switch (level)
        {
            case "Light":
                titleText.text = "Rainfall: Light";
                valueText.text = "< 7.5 mm / 6 hours < 10 mm / day";
                warningText.text = "Low risk. Minimal impact.";
                break;

            case "Medium":
                titleText.text = "Rainfall: Medium";
                valueText.text = "7.5 – 35 mm / 6 hours 10 – 50 mm / day";
                warningText.text = "Potential puddles in low areas.";
                break;

            case "Heavy":
                titleText.text = "Rainfall: Heavy";
                valueText.text = "35 – 70 mm / 6 hours 50 – 100 mm / day";
                warningText.text = "Urban flooding likely. Low-lying areas at risk.";
                break;

            case "VeryHeavy":
                titleText.text = "Rainfall: Very Heavy";
                valueText.text = "> 70 mm / 6 hours\n> 100 mm / day";
                warningText.text = "Severe flood risk. Evacuate if needed!";
                break;
        }
    }
}
