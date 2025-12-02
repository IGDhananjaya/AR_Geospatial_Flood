using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelFloodInfo;
    public GameObject panelShelterInfo;
    public GameObject panelWarning;

    [Header("Arrow / Direction UI")]
    public GameObject arrowToShelter;

    [Header("Dropdown Hujan")]
    public GameObject rainDropdown;

    private bool shelterPanelVisible = false;
    private bool isInFloodArea = false;

    private void Start()
    {
        if (panelFloodInfo != null) panelFloodInfo.SetActive(true);
        if (panelShelterInfo != null) panelShelterInfo.SetActive(false);
        if (arrowToShelter != null) arrowToShelter.SetActive(false);
        if (panelWarning != null) panelWarning.SetActive(false);
        if (rainDropdown != null) rainDropdown.SetActive(false);
    }

    // ======================= Flood UI =======================

    public void SetFloodAreaState(bool inFlood)
    {
        isInFloodArea = inFlood;
        Debug.Log(inFlood
            ? "[UIManager] Pengguna berada di area banjir."
            : "[UIManager] Pengguna keluar dari area banjir.");
    }

    public void UpdateFloodDropdownState(string floodRisk, bool isInFloodZone)
    {
        if (rainDropdown == null) return;

        bool shouldShow = isInFloodZone && (floodRisk == "Medium" || floodRisk == "High");

        if (shouldShow && !rainDropdown.activeSelf)
        {
            rainDropdown.SetActive(true);
            Debug.Log("[UIManager] Dropdown hujan diaktifkan (risiko medium/high).");
        }
        else if (!shouldShow && rainDropdown.activeSelf)
        {
            rainDropdown.SetActive(false);
            Debug.Log("[UIManager] Dropdown hujan disembunyikan (risiko rendah / di luar area).");
        }
    }

    public void ShowDropdownImmediate()
    {
        if (rainDropdown != null && !rainDropdown.activeSelf)
        {
            rainDropdown.SetActive(true);
            Debug.Log("[UIManager] Dropdown diaktifkan segera (paralel dengan animasi).");
        }
    }

    // ======================= Shelter UI =======================

    public void ShowShelterInfoPanel()
    {
        if (panelShelterInfo == null) return;

        if (!shelterPanelVisible)
        {
            panelShelterInfo.SetActive(true);
            shelterPanelVisible = true;
            Debug.Log("[UIManager] Panel Shelter diaktifkan.");
        }

        if (arrowToShelter != null)
        {
            arrowToShelter.SetActive(true);
            Debug.Log("[UIManager] Panah ke shelter diaktifkan.");
        }
    }

    public void HideShelterInfoPanel()
    {
        if (panelShelterInfo == null) return;

        if (shelterPanelVisible)
        {
            panelShelterInfo.SetActive(false);
            shelterPanelVisible = false;
            Debug.Log("[UIManager] Panel Shelter disembunyikan.");
        }

        if (arrowToShelter != null)
        {
            arrowToShelter.SetActive(false);
            Debug.Log("[UIManager] Panah ke shelter disembunyikan.");
        }
    }

    // ======================= Warning UI =======================

    public void ShowWarning(string message)
    {
        if (panelWarning != null)
        {
            panelWarning.SetActive(true);
            Debug.Log($"[UIManager] WARNING: {message}");
        }
    }

    public void HideWarning()
    {
        if (panelWarning != null)
        {
            panelWarning.SetActive(false);
            Debug.Log("[UIManager] Warning disembunyikan.");
        }
    }

    // ======================= Accessors =======================

    public bool IsInFloodArea() => isInFloodArea;
    public bool IsShelterPanelVisible() => shelterPanelVisible;
}
