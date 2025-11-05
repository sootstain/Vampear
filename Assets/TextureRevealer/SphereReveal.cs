using System;
using UnityEngine;

public class SphereReveal : MonoBehaviour
{
    //TODO: Make this work with sound multiplier
    
    //TODO: MAKE THIS WORK WITH SOUNDWAVES SHADER, i cry fr why tf-
    
    [SerializeField] public float minRadius = 0.1f;
    [SerializeField] public float maxRadius = 2f;
    public float hardness = 0.8f;
    public float strength = 1f;
    public Color paintColor = Color.white;
    
    private SphereCollider currentSphere;

    private void Start()
    {
        currentSphere = GetComponent<SphereCollider>();
    }
    
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, currentSphere.radius);
         
        foreach (Collider col in hitColliders)
        {
            col.TryGetComponent(out Paintable p);
            if (p != null) { 
                Vector3 closestPoint = col.ClosestPoint(transform.position); 
                PaintManager.instance.paint(p, closestPoint, currentSphere.radius, hardness, strength, paintColor); 
            } 
        } 
    }
}
