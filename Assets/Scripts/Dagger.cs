using System;
using UnityEngine;

public class Dagger : MonoBehaviour
{
    [field: SerializeField] public int Damage { get; set; }
    private float lifespan = 5f;
    
    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        other.GetComponent<Health>().TakeDamage(Damage);
        Destroy(gameObject);
    }

    public void Update()
    {
        lifespan -= Time.deltaTime;
        if (lifespan <= 0f) Destroy(gameObject);
    }
}
