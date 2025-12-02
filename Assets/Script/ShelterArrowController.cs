using UnityEngine;

public class ShelterArrowController : MonoBehaviour
{
    [Header("References")]
    public Transform player;             // kamera / posisi user
    public Transform currentShelter;     // shelter yang dituju
    public float rotationSpeed = 5f;     // kecepatan rotasi panah

    [Header("Behavior")]
    public bool faceInWorldSpace = true; // true: panah 3D di dunia, false: panah di UI (Canvas)

    private void Update()
    {
        if (player == null || currentShelter == null)
            return;

        Vector3 direction = currentShelter.position - player.position;
        direction.y = 0; // abaikan ketinggian

        if (faceInWorldSpace)
        {
            // Panah dalam world-space
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }
        else
        {
            // Panah dalam UI Canvas (screen-space)
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(currentShelter.position);
            Vector3 dir = screenPoint - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle - 90), Time.deltaTime * rotationSpeed);
        }
    }

    // Fungsi ini dipanggil oleh ShelterProximity saat shelter baru terdeteksi
    public void SetTarget(Transform shelter)
    {
        currentShelter = shelter;
        if (shelter != null)
            Debug.Log($"[ShelterArrowController] Target shelter diatur ke {shelter.name}");
    }

    public void ClearTarget()
    {
        currentShelter = null;
    }
}
