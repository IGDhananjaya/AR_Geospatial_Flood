using UnityEngine;

public class UIManager1 : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panelInfo;
    public GameObject rainDropdown;

    private bool isInFlood = false;
    private bool isAtShelter = false;

    void Start()
    {
        HideAll();
    }

    // ✅ Masuk zona banjir
    public void ShowFloodUI()
    {
        isInFlood = true;
        isAtShelter = false;

        if (panelInfo) panelInfo.SetActive(true);
        if (rainDropdown) rainDropdown.SetActive(true);

        Debug.Log("🌊 UIManager: Menampilkan UI banjir");
    }

    // ✅ Keluar zona banjir
    public void HideFloodUI()
    {
        isInFlood = false;

        // Jika masih di shelter, hanya matikan dropdown
        if (isAtShelter)
        {
            if (rainDropdown) rainDropdown.SetActive(false);
        }
        else
        {
            HideAll();
        }

        Debug.Log("🚫 UIManager: Menyembunyikan UI banjir");
    }

    // ✅ Masuk shelter
    public void ShowShelterUI()
    {
        isAtShelter = true;
        isInFlood = false;

        if (panelInfo) panelInfo.SetActive(true);
        if (rainDropdown) rainDropdown.SetActive(false);

        Debug.Log("🏠 UIManager: Menampilkan UI shelter");
    }

    // ✅ Keluar shelter
    public void HideShelterUI()
    {
        isAtShelter = false;

        // Kalau sedang di banjir, tampilkan lagi UI banjir
        if (isInFlood)
        {
            if (panelInfo) panelInfo.SetActive(true);
            if (rainDropdown) rainDropdown.SetActive(true);
        }
        else
        {
            HideAll();
        }

        Debug.Log("↔️ UIManager: Menyembunyikan UI shelter");
    }

    // ✅ Menyembunyikan semua UI
    public void HideAll()
    {
        if (panelInfo) panelInfo.SetActive(false);
        if (rainDropdown) rainDropdown.SetActive(false);

        isAtShelter = false;
        isInFlood = false;
    }

    public bool IsAtShelter() => isAtShelter;
    public bool IsInFlood() => isInFlood;
}
