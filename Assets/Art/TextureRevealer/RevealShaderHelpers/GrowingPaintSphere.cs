using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingPaintSphere : MonoBehaviour
{
    [SerializeField] private GameObject spherePrefab;

    [SerializeField] public float minRadius = 0.1f;
    [SerializeField] public float maxRadius = 2f;
    [SerializeField] public float growthSpeed = 1f;

    [SerializeField] public float fadeDelay = 5f; // Time before starting to fade
    [SerializeField] public float fadeDuration = 2f;

    [SerializeField]
    public float highlightAlpha = 1f; //To change with sound; currently just used with bell so hard-coded a bit

    private float targetAlpha = 0.2f; //This will always be 0.2 / the faded colour

    public float hardness = 0.8f;
    public float strength = 1f;
    public Color paintColor = Color.white;

    private GameObject spawnedSphere;
    private SphereCollider currentCollider;
    private float currentScale;
    private float timeSinceSphereMade;
    private bool isFading = false;
    private bool fadeComplete = false;
    private HashSet<Paintable> affectedPaintables = new HashSet<Paintable>();

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
        if (currentScale < maxRadius)
        {
            currentScale += growthSpeed * Time.deltaTime;
            currentScale = Mathf.Min(currentScale, maxRadius);
            spawnedSphere.transform.localScale = Vector3.one * currentScale;
            currentCollider.radius = currentScale;
        }

        float paintRadius = currentScale;

        Collider[] hitColliders = Physics.OverlapSphere(currentCollider.transform.position, currentScale);

        foreach (Collider col in hitColliders)
        {
            if (col.TryGetComponent(out Paintable p))
            {
                Vector3 closestPoint = col.ClosestPoint(currentCollider.transform.position);

                float currentAlpha = highlightAlpha;
                if (isFading && !fadeComplete)
                {
                    float fadeProgress = Mathf.Clamp01((timeSinceSphereMade - fadeDelay) / fadeDuration);
                    currentAlpha = Mathf.Lerp(1f, targetAlpha, fadeProgress);

                    if (fadeProgress >= 1f)
                    {
                        fadeComplete = true;
                    }
                }
                else if (fadeComplete)
                {
                    currentAlpha = targetAlpha;
                }

                Color colour = new Color(paintColor.r, paintColor.g, paintColor.b, currentAlpha); //adding this for the fadeout on objects
                ColourManager.instance.paint(p, closestPoint, paintRadius, hardness, strength, colour, 0);

                affectedPaintables.Add(p);
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

        if (fadeComplete && timeSinceSphereMade >= fadeDelay + fadeDuration + 1f)
        {
            Destroy(gameObject);
        }
    }
}