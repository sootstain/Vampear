using UnityEngine;

public class GrowingPaintSphere : MonoBehaviour
{
    [SerializeField] private GameObject spherePrefab;

    [SerializeField] public float minRadius = 0.1f;
    [SerializeField] public float maxRadius = 2f;
    [SerializeField] public float growthSpeed = 1f;
    [SerializeField] public float fadeDelay = 5f; // Time before starting to fade

    public float hardness = 0.8f;
    public float strength = 1f;
    public Color paintColor = Color.white;
    
    private GameObject spawnedSphere;
    private SphereCollider currentCollider;
    private float currentScale;
    private float timeSinceSphereMade;
    private bool isFading = false;
    
    void Start()
    {
        SpawnSphere();
        timeSinceSphereMade = 0f;
    }

    private void SpawnSphere()
    {
        spawnedSphere = Instantiate(spherePrefab, transform.position, Quaternion.identity);
        currentCollider = spawnedSphere.GetComponent<SphereCollider>();
        spawnedSphere.transform.localScale = Vector3.one * minRadius;
        currentScale = minRadius;
    }

    private void PaintSphere()
    {
        // Grow the sphere if not at max size yet
        if (currentScale < maxRadius)
        {
            currentScale += growthSpeed * Time.deltaTime;
            currentScale = Mathf.Min(currentScale, maxRadius);
            spawnedSphere.transform.localScale = Vector3.one * currentScale;
            currentCollider.radius = currentScale;
        }
        
        float paintRadius = currentScale * 0.5f;
        
        Collider[] hitColliders = Physics.OverlapSphere(currentCollider.transform.position, currentScale);

        foreach (Collider col in hitColliders)
        {
            if (col.TryGetComponent(out Paintable p))
            {
                Vector3 closestPoint = col.ClosestPoint(currentCollider.transform.position);
                int fadeValue = isFading ? 1 : 0;
                ColourManager.instance.paint(p, closestPoint, paintRadius, hardness, strength, paintColor, fadeValue);
            }
        }
        
        
    }

    void Update()
    {
        timeSinceSphereMade += Time.deltaTime;

        if (timeSinceSphereMade >= fadeDelay && !isFading)
        {
            isFading = true;
        }

        PaintSphere();
    }
}