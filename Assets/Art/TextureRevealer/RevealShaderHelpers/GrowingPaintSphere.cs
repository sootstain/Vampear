using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GrowingPaintSphere : MonoBehaviour
{
    [Header("Sphere Settings")]
    public float minRadius = 0.1f;
    public float maxRadius = 2f;
    public float growthSpeed = 1f;

    [Header("Fade Settings")]
    public float fadeDelay = 5f;
    public float fadeDuration = 2f;
    public float highlightAlpha = 1f;
    private float targetAlpha = 0.2f;

    [Header("Paint Settings")]
    public Color paintColor = Color.white;
    public float hardness = 0.8f;
    public float strength = 1f;

    private float currentScale;
    private float timeSinceSphereMade;
    private bool isFading = false;
    private bool fadeComplete = false;
    private HashSet<Paintable> affectedPaintables = new HashSet<Paintable>();

    private SphereCollider triggerCollider;

    void Start()
    {
        triggerCollider = gameObject.AddComponent<SphereCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.radius = minRadius;
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        currentScale = minRadius;
        timeSinceSphereMade = 0f;
    }

    void Update()
    {
        if (currentScale < maxRadius)
        {
            currentScale += growthSpeed * Time.deltaTime;
            currentScale = Mathf.Min(currentScale, maxRadius);
            triggerCollider.radius = currentScale;
        }

        timeSinceSphereMade += Time.deltaTime;

        if (timeSinceSphereMade >= fadeDelay && !isFading)
            isFading = true;

        float currentAlpha = highlightAlpha;
        if (isFading && !fadeComplete)
        {
            float fadeProgress = Mathf.Clamp01((timeSinceSphereMade - fadeDelay) / fadeDuration);
            currentAlpha = Mathf.Lerp(highlightAlpha, targetAlpha, fadeProgress);
            if (fadeProgress >= 1f)
                fadeComplete = true;
        }
        else if (fadeComplete)
        {
            currentAlpha = targetAlpha;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, currentScale);
        foreach (Collider col in hitColliders)
        {
            if (col.TryGetComponent(out Paintable p))
            {
                Vector3 closestPoint = col.ClosestPoint(transform.position);
                Color colour = new Color(paintColor.r, paintColor.g, paintColor.b, currentAlpha);
                ColourManager.instance.paint(p, closestPoint, currentScale, hardness, strength, colour, 0);
                affectedPaintables.Add(p);
            }
        }

        //GETTING RID OF THIS BECAUSE HOW ELSE TO MAKE ENEMIES DISAPPEAR AFTER THEY WALK OUT :(
        /*if (fadeComplete && timeSinceSphereMade >= fadeDelay + fadeDuration + 1f)
        {
            Destroy(gameObject);
        }*/
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Paintable p))
        {
            Vector3 center = p.GetComponent<Renderer>().bounds.center;
            p.ForceAlphaZero = true;
            ColourManager.instance.paint(p, center, 100f, 1f, 1f, new Color(paintColor.r, paintColor.g, paintColor.b, 0f), 0);
            affectedPaintables.Remove(p);
        }
    }
}
