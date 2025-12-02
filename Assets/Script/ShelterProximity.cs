using UnityEngine;

public class ShelterProximity : MonoBehaviour
{
    [Header("References")]
    public Transform userCamera;         // Posisi pemain / kamera
    public Transform[] shelterPoints;    // Titik-titik shelter di scene
    public UIManager uiManager;          // Untuk tampilkan panel shelter
    public GameObject arrowToShelter;    // Objek panah (UI atau world-space)
    public ShelterInfoManager shelterInfoManager;

    [Header("Detection Settings")]
    [Tooltip("Radius dalam meter untuk deteksi masuk area shelter")]
    public float enterRadius = 25f;
    [Tooltip("Radius keluar area shelter (lebih besar sedikit untuk stabilitas)")]
    public float exitRadius = 30f;

    private Transform nearestShelter;
    private bool isNearShelter = false;

    void Update()
    {
        if (userCamera == null || shelterPoints == null || shelterPoints.Length == 0)
            return;

        // Temukan shelter terdekat
        float nearestDist = float.MaxValue;
        Transform closest = null;

        foreach (Transform shelter in shelterPoints)
        {
            if (shelter == null) continue;

            float dist = Vector3.Distance(userCamera.position, shelter.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                closest = shelter;
            }
        }

        nearestShelter = closest;
        if (nearestShelter == null) return;

        // Logika radius masuk & keluar
        if (!isNearShelter && nearestDist <= enterRadius)
        {
            EnterShelterZone();
        }
        else if (isNearShelter && nearestDist > exitRadius)
        {
            ExitShelterZone();
        }

        // Update arah panah jika masih di dalam zona
        if (isNearShelter)
            UpdateArrowDirection();
    }

    private void EnterShelterZone()
    {
        isNearShelter = true;
        Debug.Log("[ShelterProximity] Pengguna masuk area shelter.");

        uiManager?.ShowShelterInfoPanel();

        if (shelterInfoManager != null && nearestShelter != null)
        {
            float dist = Vector3.Distance(userCamera.position, nearestShelter.position);
            shelterInfoManager.UpdateShelterInfo(
                nearestShelter,
                nearestShelter.name,
                dist,
                "Available"
            );
            shelterInfoManager.SetReferences(userCamera);
        }

        if (arrowToShelter != null && !arrowToShelter.activeSelf)
            arrowToShelter.SetActive(true);
    }

    private void ExitShelterZone()
    {
        isNearShelter = false;
        Debug.Log("[ShelterProximity] Pengguna keluar area shelter.");

        uiManager?.HideShelterInfoPanel();
        shelterInfoManager?.ClearShelterInfo();

        if (arrowToShelter != null)
            arrowToShelter.SetActive(false);
    }

    private void UpdateArrowDirection()
    {
        if (arrowToShelter == null || nearestShelter == null || userCamera == null)
            return;

        Vector3 direction = nearestShelter.position - userCamera.position;
        direction.y = 0; // abaikan perbedaan tinggi

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        arrowToShelter.transform.rotation = Quaternion.Lerp(
            arrowToShelter.transform.rotation,
            targetRotation,
            Time.deltaTime * 5f
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (shelterPoints == null) return;

        foreach (Transform shelter in shelterPoints)
        {
            if (shelter == null) continue;

            // radius masuk (hijau muda)
            Gizmos.color = new Color(0f, 1f, 0f, 0.4f);
            Gizmos.DrawWireSphere(shelter.position, enterRadius);

            // radius keluar (kuning)
            Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
            Gizmos.DrawWireSphere(shelter.position, exitRadius);
        }
    }

    // ==========================================================
    // Public accessor untuk status shelter
    // ==========================================================
    public bool IsNearShelter()
    {
        return isNearShelter;
    }

}
