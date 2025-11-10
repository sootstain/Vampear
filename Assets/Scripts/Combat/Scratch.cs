using System.Collections.Generic;
using UnityEngine;

public class Scratch : MonoBehaviour
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
        
        Debug.Log("Adding to list");
        collidedWith.Add(other);
        
        if (other.TryGetComponent(out Health health))
        {
            Debug.Log("Take damage!");
            health.TakeDamage(10);
        }

    }
}
