using UnityEngine;

public class GrowingPaintSphere : MonoBehaviour
{
    [SerializeField] private GameObject spherePrefab;

    [SerializeField] public float minRadius = 0.1f;
    [SerializeField] public float maxRadius = 2f;
    [SerializeField] public float growthSpeed = 1f;

    public float hardness = 0.8f;
    public float strength = 1f;
    public Color paintColor = Color.white;
    
    private GameObject spawnedSphere;
    private SphereCollider currentCollider;
    private float currentScale;

    void Start()
    {
        spawnedSphere = Instantiate(spherePrefab, transform.position, Quaternion.identity);
        currentCollider = spawnedSphere.GetComponent<SphereCollider>();

        spawnedSphere.transform.localScale = Vector3.one * minRadius;
        currentScale = minRadius;
    }

    void Update()
    {
        if (currentScale < maxRadius)
        {
            currentScale += growthSpeed * Time.deltaTime;
            currentScale = Mathf.Min(currentScale, maxRadius);
            spawnedSphere.transform.localScale = Vector3.one * currentScale;
            
            currentCollider.radius = currentScale;
        }
        
        float paintRadius = currentScale * 0.5f; //this doesn't really work, IDK the scale is super annoying :)
        
        //Also this shit doesn't work with the sphere unless I make it huge so wtf
        Collider[] hitColliders = Physics.OverlapSphere(currentCollider.transform.position, currentScale);

        foreach (Collider col in hitColliders)
        {
            if (col.TryGetComponent(out Paintable p))
            {
                Vector3 closestPoint = col.ClosestPoint(currentCollider.transform.position);
                ColourManager.instance.paint(p, closestPoint, paintRadius, hardness, strength, paintColor);
            }
        }
    }
}
