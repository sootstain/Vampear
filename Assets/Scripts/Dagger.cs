using System;
using UnityEngine;

public class Dagger : MonoBehaviour
{
    [field: SerializeField] public int Damage { get; set; }
    
    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        other.GetComponent<Health>().TakeDamage(Damage);
        Destroy(gameObject);
    }
}
