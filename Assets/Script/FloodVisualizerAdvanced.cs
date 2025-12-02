using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class FloodAreaVisualizerAdvanced : MonoBehaviour
{
    [System.Serializable]
    public class FloodPoint
    {
        public Transform pointTransform;
        public float enterRadius = 20f;
        public Color color = Color.cyan;
    }

    [Header("Flood Centers (multi-radius supported)")]
    public List<FloodPoint> floodPoints = new List<FloodPoint>();

    [Header("Visual Settings")]
    public bool showOverlap = true;
    public float centerSphereSize = 0.4f;
    public Color overlapColor = new Color(0f, 0.5f, 1f, 0.1f); // biru muda transparan

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (floodPoints == null || floodPoints.Count == 0)
            return;

        // Tampilkan masing-masing titik flood
        foreach (var fp in floodPoints)
        {
            if (fp.pointTransform == null) continue;

            Gizmos.color = fp.color;
            Gizmos.DrawSphere(fp.pointTransform.position, centerSphereSize);
            Gizmos.DrawWireSphere(fp.pointTransform.position, fp.enterRadius);
        }

        // Garis penghubung antar titik
        Gizmos.color = Color.cyan;
        for (int i = 0; i < floodPoints.Count - 1; i++)
        {
            if (floodPoints[i].pointTransform != null && floodPoints[i + 1].pointTransform != null)
            {
                Gizmos.DrawLine(
                    floodPoints[i].pointTransform.position,
                    floodPoints[i + 1].pointTransform.position
                );
            }
        }

        // Overlap radius visual
        if (showOverlap)
        {
            for (int i = 0; i < floodPoints.Count; i++)
            {
                for (int j = i + 1; j < floodPoints.Count; j++)
                {
                    var p1 = floodPoints[i];
                    var p2 = floodPoints[j];
                    if (p1.pointTransform == null || p2.pointTransform == null) continue;

                    float dist = Vector3.Distance(p1.pointTransform.position, p2.pointTransform.position);
                    if (dist < (p1.enterRadius + p2.enterRadius))
                    {
                        // Tampilkan garis overlap
                        Gizmos.color = overlapColor;
                        Vector3 mid = (p1.pointTransform.position + p2.pointTransform.position) / 2;
                        Gizmos.DrawSphere(mid, 0.5f);
                        Gizmos.DrawLine(p1.pointTransform.position, p2.pointTransform.position);
                    }
                }
            }
        }
    }
#endif
}
