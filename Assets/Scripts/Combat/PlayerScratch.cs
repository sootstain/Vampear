using System.Collections.Generic;
using UnityEngine;

public class PlayerScratch : MonoBehaviour
{
    [SerializeField] private Collider playerCollider;
    private List<Collider> collidedWith = new List<Collider>(); //to check if hit with current anim scratch
    
    private void OnEnable()
    {
        collidedWith.Clear();
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other == playerCollider) return;
        
        if (collidedWith.Contains(other)) return;
        
        //Debug.Log("Adding to list");
        collidedWith.Add(other);
        
        if (other.TryGetComponent(out Health health))
        {
            Debug.Log("Take damage!");
            health.TakeDamage(10); //arbitrary for now :)
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 hitNormal = (other.transform.position - transform.position).normalized;
            health.SpawnSplatter(hitPoint, hitNormal);
        }
        
        if (other.TryGetComponent(out ForceReceiver forceReceiver))
        {
            Vector3 direction = (other.transform.position - playerCollider.transform.position).normalized;
            forceReceiver.AddForce(direction * 5); //knockback force, currently arbitrary
        }

    }
}
