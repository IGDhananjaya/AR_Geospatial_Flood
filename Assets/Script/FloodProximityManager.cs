using UnityEngine;

public class FloodProximityManager : MonoBehaviour
{
    [System.Serializable]
    public class FloodPoint
    {
        public Transform pointTransform;
        [Header("Radius Settings")]
        public float enterRadius = 20f;
        public float exitRadius = 25f;
        public bool isCentralPoint = false;
    }

    [Header("Flood Zone Settings")]
    public FloodPoint[] floodPoints;
    public float centerRadiusMultiplier = 1.5f;
    public bool debugDraw = true;
    public bool autoFlood = false;

    [Header("References")]
    public ShelterProximity shelterProximity; // referensi ke script ShelterProximity
    public Transform player;
    public UIManager uiManager;
    public summonSmooth floodSimulator;

    private bool isInFloodArea = false;

    void Update()
    {
        if (autoFlood)
        {
            if (!isInFloodArea)
                EnterFloodArea();
            return;
        }

        if (player == null)
        {
            if (isInFloodArea)
                ExitFloodArea();
            return;
        }

        if (floodPoints == null || floodPoints.Length == 0)
            return;

        bool inside = false;

        foreach (var fp in floodPoints)
        {
            if (fp.pointTransform == null) continue;

            float dist = Vector3.Distance(player.position, fp.pointTransform.position);
            float enterR = fp.enterRadius;
            float exitR = fp.exitRadius;

            if (fp.isCentralPoint)
            {
                enterR *= centerRadiusMultiplier;
                exitR *= centerRadiusMultiplier;
            }

            if (!isInFloodArea && dist <= enterR)
            {
                inside = true;
                break;
            }
            else if (isInFloodArea && dist <= exitR)
            {
                inside = true;
                break;
            }
        }

        if (inside && !isInFloodArea)
            EnterFloodArea();
        else if (!inside && isInFloodArea)
            ExitFloodArea();
        // --- Tambahan: Jika pengguna sedang di shelter, abaikan flood zone ---
        if (shelterProximity != null && shelterProximity.IsNearShelter())
        {
            if (uiManager != null && uiManager.rainDropdown != null && uiManager.rainDropdown.activeSelf)
            {
                uiManager.rainDropdown.SetActive(false);
                Debug.Log("[FloodProximity] 🏠 Shelter priority: hiding dropdown.");
            }
            return; // hentikan deteksi flood untuk frame ini
        }

        isInFloodArea = inside;
    }

    private void EnterFloodArea()
    {
        isInFloodArea = true;
        Debug.Log("[FloodProximity] ✅ User entered flood area");

        // Aktifkan UI Flood & Dropdown
        uiManager?.SetFloodAreaState(true);
        uiManager?.ShowDropdownImmediate();

        // Jalankan animasi banjir default (Light)
        if (floodSimulator != null)
        {
            floodSimulator.gameObject.SetActive(true);
            floodSimulator.StartSummon();
            floodSimulator.SetFloodIntensity("Light");
            Debug.Log("[FloodProximity] 🌊 Flood animation started (Light).");
        }
        else
        {
            Debug.LogWarning("[FloodProximity] ❌ floodSimulator belum diisi di Inspector!");
        }

        // Aktifkan audio banjir
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayFloodSound();
            Debug.Log("[FloodProximity] 🔊 Flood sound started.");
        }
    }

    private void ExitFloodArea()
    {
        isInFloodArea = false;
        Debug.Log("[FloodProximity] 🚫 User exited flood area");

        uiManager?.SetFloodAreaState(false);

        // Sembunyikan air
        if (floodSimulator != null)
        {
            floodSimulator.StopSummon();
            Debug.Log("[FloodProximity] 🌊 Flood visual disabled.");
        }

        // Sembunyikan dropdown
        if (uiManager != null && uiManager.rainDropdown != null)
            uiManager.rainDropdown.SetActive(false);

        // Hentikan suara
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopFloodSound();
            Debug.Log("[FloodProximity] 🔇 Flood sound stopped.");
        }
    }

    public bool IsInFloodArea() => isInFloodArea;

    void OnDrawGizmosSelected()
    {
        if (!debugDraw || floodPoints == null) return;

        foreach (var fp in floodPoints)
        {
            if (fp.pointTransform == null) continue;
            float enterR = fp.enterRadius;
            float exitR = fp.exitRadius;
            if (fp.isCentralPoint)
            {
                enterR *= centerRadiusMultiplier;
                exitR *= centerRadiusMultiplier;
            }

            Gizmos.color = fp.isCentralPoint ? Color.red : Color.cyan;
            Gizmos.DrawWireSphere(fp.pointTransform.position, enterR);

            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawWireSphere(fp.pointTransform.position, exitR);
        }
    }
}
