using UnityEngine;

public class UIShelterManager : MonoBehaviour
{
    [Header("Shelter UI References")]
    public GameObject panelInfo;       // Panel informasi shelter (bisa sama prefab dengan flood)
    public GameObject rainDropdown;    // Dropdown disembunyikan saat di shelter

    private bool isAtShelter = false;

    public void EnterShelter()
    {
        if (isAtShelter) return;
        isAtShelter = true;
        ShowShelterUI();
    }

    public void ExitShelter()
    {
        if (!isAtShelter) return;
        isAtShelter = false;
        HideShelterUI();
    }

    private void ShowShelterUI()
    {
        panelInfo?.SetActive(true);
        rainDropdown?.SetActive(false);
    }

    private void HideShelterUI()
    {
        panelInfo?.SetActive(false);
    }
}
