using UnityEngine;

public class GrowingPaintSphere : MonoBehaviour
{
    [Header("Growth Settings")]
    [SerializeField] public float minRadius = 0.1f;
    [SerializeField] public float maxRadius = 2f;
    [SerializeField] public float growthSpeed = 1f;

    [Header("Paint Settings")]
    public float hardness = 0.8f;
    public float strength = 1f;
    public Color paintColor = Color.white;

    private SphereCollider currentSphere;
    private float currentScale;

    void Start()
    {
        currentSphere = GetComponent<SphereCollider>();

        // Start the sphere small
        transform.localScale = Vector3.one * minRadius;
        currentScale = minRadius;
    }

    void Update()
    {
        // Grow the sphere
        if (currentScale < maxRadius)
        {
            currentScale += growthSpeed * Time.deltaTime;
            currentScale = Mathf.Min(currentScale, maxRadius);
            transform.localScale = Vector3.one * currentScale;
        }

        // Compute world-space radius
        float worldRadius = currentSphere.radius * transform.lossyScale.x;

        // Find overlapping colliders
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, worldRadius);

        foreach (Collider col in hitColliders)
        {
            if (col.TryGetComponent(out Paintable p))
            {
                Vector3 closestPoint = col.ClosestPoint(transform.position);
                PaintManager.instance.paint(p, closestPoint, worldRadius, hardness, strength, paintColor);
            }
        }
    }
}
