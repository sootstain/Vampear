using UnityEngine;

public class ThrowDagger : MonoBehaviour
{
    public GameObject daggerPrefab;
    public GameObject player;
    public Transform throwPoint;
    public float throwForce = 20f;
    
    public void Throw()
    {
        Vector3 direction = (player.transform.position - throwPoint.position).normalized;

        var dagger = Instantiate(daggerPrefab, throwPoint.position, Quaternion.LookRotation(direction));
        
        Rigidbody rb = dagger.GetComponent<Rigidbody>();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.linearVelocity = direction * throwForce;
    }
}
