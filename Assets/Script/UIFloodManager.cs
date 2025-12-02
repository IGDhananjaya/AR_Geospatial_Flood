using UnityEngine;

public class UIFloodManager : MonoBehaviour
{
    [Header("Flood UI References")]
    public GameObject panelInfo;       // Panel informasi umum banjir
    public GameObject rainDropdown;    // Dropdown curah hujan

    private bool isInFlood = false;

    public void EnterFloodArea()
    {
        if (isInFlood) return;
        isInFlood = true;
        ShowFloodUI();
    }

    public void ExitFloodArea()
    {
        if (!isInFlood) return;
        isInFlood = false;
        HideFloodUI();
    }

    private void ShowFloodUI()
    {
        panelInfo?.SetActive(true);
        rainDropdown?.SetActive(true);
    }

    private void HideFloodUI()
    {
        panelInfo?.SetActive(false);
        rainDropdown?.SetActive(false);
    }
}
