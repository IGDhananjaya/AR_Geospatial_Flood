using UnityEngine;

public class FloodTrigger : MonoBehaviour
{
    [SerializeField] private UIFloodManager floodUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            floodUI?.EnterFloodArea();
            Debug.Log("Player entered flood area");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            floodUI?.ExitFloodArea();
            Debug.Log("Player exited flood area");
        }
    }
}
