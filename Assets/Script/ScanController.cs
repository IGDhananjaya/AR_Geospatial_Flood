using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions;
using Unity.XR.CoreUtils;   // untuk XROrigin


public class ScanController : MonoBehaviour
{
    public enum ScanState { Initializing, Localizing, Ready, Degraded, Fallback }

    [Header("AR")]
    public ARSession arSession;              // drag: AR Session
    // public ARSessionOrigin arOrigin;         // drag: AR Session Origin
    public XROrigin xrOrigin;            // kalau kamu ingin tetap expose Origin
    public ARCameraManager arCameraManager;  // drag: AR Camera (komponen ini)
    public AREarthManager earthManager;      // drag: GeospatialManager (komponen AREarthManager)
    public ARCoreExtensions arCoreExtensions;// drag: AR Core Extensions

    [Header("UI")]
    public TMP_Text statusText;              // drag: Canvas/GeospatialStatus (TMP)
    public Button btnStartScan;              // drag: Canvas/Btn_Home (sementara)
    public GameObject coachPanel;            // drag: Canvas/CoachPanel (kita buat di langkah 2)

    [Header("Thresholds (meter)")]
    public float horizontalAccReady = 2.5f;
    public float verticalAccReady = 4.0f;
    public float degradeAfterSeconds = 2.0f;

    public System.Action<double, double, double> OnGeoReady; // lat,lon,alt

    ScanState _state = ScanState.Initializing;
    float _lostTimer;
    bool _locked;

    void Awake()
    {
        Application.targetFrameRate = 60;
        if (btnStartScan) btnStartScan.onClick.AddListener(OnStartScan);
        if (coachPanel) coachPanel.SetActive(false);
        SetStatus("Init… membuka kamera & lokasi");
    }

    void Update()
    {
        // Fallback bila Geospatial tidak aktif
        if (earthManager == null || earthManager.EarthState != EarthState.Enabled)
        { SetState(ScanState.Fallback); SetStatus("Geospatial tidak tersedia (Fallback)."); return; }

        var ss = arSession?.subsystem; if (ss == null) return;
        var tracking = ss.trackingState;

        if (tracking != UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
        {
            _lostTimer += Time.deltaTime;
            if (_state == ScanState.Ready && _lostTimer > degradeAfterSeconds) SetState(ScanState.Degraded);
            if (_state == ScanState.Initializing || _state == ScanState.Localizing) SetStatus("Mencari tracking… gerakkan ponsel menyapu area.");
            return;
        }
        _lostTimer = 0f;

        var pose = earthManager.CameraGeospatialPose;
        double h = pose.HorizontalAccuracy, v = pose.VerticalAccuracy;

        if (_state == ScanState.Initializing)
        { SetState(ScanState.Localizing); ShowCoach(true); SetStatus("Localizing VPS… lihat sekeliling."); }

        if (_state == ScanState.Localizing || _state == ScanState.Degraded)
        {
            if (h <= horizontalAccReady && v <= verticalAccReady)
            {
                SetState(ScanState.Ready); ShowCoach(false);
                if (!_locked) { _locked = true; OnGeoReady?.Invoke(pose.Latitude, pose.Longitude, pose.Altitude); }
                SetStatus($"Ready ✓  HAcc:{h:0.0}m  VAcc:{v:0.0}m");
            }
            else SetStatus($"Localizing… HAcc:{h:0.0}m  VAcc:{v:0.0}m");
        }
        else if (_state == ScanState.Ready) SetStatus($"Ready ✓  HAcc:{h:0.0}m  VAcc:{v:0.0}m");
    }

    void OnStartScan()
    {
        if (_state == ScanState.Initializing || _state == ScanState.Fallback)
        { SetState(ScanState.Localizing); ShowCoach(true); }
    }

    void SetState(ScanState s) { _state = s; }
    void SetStatus(string t) { if (statusText) statusText.text = t; }
    void ShowCoach(bool on) { if (coachPanel) coachPanel.SetActive(on); }
}
