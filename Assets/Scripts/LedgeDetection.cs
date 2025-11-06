using UnityEngine;
using System; 
public class LedgeDetection : MonoBehaviour
{
    public event Action<Vector3> OnLedgeDetected;

    private void OnTriggerEnter(Collider other)
    {
        OnLedgeDetected?.Invoke(other.transform.forward);
    }
}
