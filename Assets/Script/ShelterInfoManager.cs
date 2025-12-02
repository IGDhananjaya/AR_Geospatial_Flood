using UnityEngine;
using TMPro;

public class ShelterInfoManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text titleText;     // Nama shelter
    public TMP_Text distanceText;  // Jarak ke shelter
    public TMP_Text statusText;    // Status atau pesan tambahan

    [Header("Settings")]
    public string defaultMessage = "No shelter nearby.";

    private Transform currentShelter;
    private Transform player;

    public void SetReferences(Transform playerTransform)
    {
        player = playerTransform;
    }

    /// <summary>
    /// Dipanggil oleh ShelterProximity ketika user berada dalam radius shelter
    /// </summary>
    public void UpdateShelterInfo(Transform shelter, string shelterName, float distance, string status)
    {
        if (titleText == null || distanceText == null || statusText == null) return;

        currentShelter = shelter;

        titleText.text = $"🏠 {shelterName}";
        distanceText.text = $"Distance: {distance:F1} m";
        statusText.text = $"Status: {status}";
    }

    /// <summary>
    /// Disembunyikan ketika user keluar dari radius semua shelter
    /// </summary>
    public void ClearShelterInfo()
    {
        if (titleText == null || distanceText == null || statusText == null) return;

        titleText.text = defaultMessage;
        distanceText.text = "";
        statusText.text = "";
    }

    private void Update()
    {
        // Perbarui jarak secara real-time selama user masih di radius shelter
        if (currentShelter != null && player != null)
        {
            float dist = Vector3.Distance(player.position, currentShelter.position);
            distanceText.text = $"Distance: {dist:F1} m";
        }
    }
}
