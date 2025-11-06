using System.Collections.Generic;
using UnityEngine;

public class Scratch : MonoBehaviour
{
    [SerializeField] private Collider playerCollider;
    
    //TODO: FIX THIS
    //private List<Collider> collidedWith = new List<Collider>(); //to check if hit with current anim scratch

    
    
    private void OnEnable()
    {
        //collidedWith.Clear();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other == playerCollider) return;
        
        //if (collidedWith.Contains(other)) return;
        //collidedWith.Add(other);
        
        if (other.TryGetComponent<Health>(out Health health))
        {

            health.TakeDamage(10);
        }

    }
}
