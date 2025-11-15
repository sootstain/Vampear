using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("Shake Settings")]
    [SerializeField] private float defaultIntensity = 0.3f;
    [SerializeField] private float defaultDuration = 0.2f;
    [SerializeField] private AnimationCurve shakeFalloff = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private Transform cameraTransform;
    private Vector3 originalPosition;
    private Coroutine currentShake;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        cameraTransform = Camera.main.transform;
        originalPosition = cameraTransform.localPosition;
    }
    
    public void Shake(float intensity = -1f, float duration = -1f)
    {
        if (intensity < 0) intensity = defaultIntensity;
        if (duration < 0) duration = defaultDuration;

        if (currentShake != null)
        {
            StopCoroutine(currentShake);
        }

        currentShake = StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float currentIntensity = intensity * shakeFalloff.Evaluate(t);

            // Generate random offset
            float xOffset = Random.Range(-1f, 1f) * currentIntensity;
            float yOffset = Random.Range(-1f, 1f) * currentIntensity;

            // Apply shake
            cameraTransform.localPosition = originalPosition + new Vector3(xOffset, yOffset, 0f);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        
        cameraTransform.localPosition = originalPosition;
        currentShake = null;
    }
    
    public void ShakeDirectional(Vector3 direction, float intensity = -1f, float duration = -1f)
    {
        if (intensity < 0) intensity = defaultIntensity;
        if (duration < 0) duration = defaultDuration;

        if (currentShake != null)
        {
            StopCoroutine(currentShake);
        }

        currentShake = StartCoroutine(ShakeDirectionalCoroutine(direction.normalized, intensity, duration));
    }

    private IEnumerator ShakeDirectionalCoroutine(Vector3 direction, float intensity, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float currentIntensity = intensity * shakeFalloff.Evaluate(t);
            
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up);
            float mainOffset = Random.Range(-0.3f, 1f) * currentIntensity;
            float perpOffset = Random.Range(-0.5f, 0.5f) * currentIntensity;

            Vector3 offset = direction * mainOffset + perpendicular * perpOffset;
            cameraTransform.localPosition = originalPosition + offset;

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        cameraTransform.localPosition = originalPosition;
        currentShake = null;
    }

    private void LateUpdate()
    {
        if (currentShake == null)
        {
            originalPosition = cameraTransform.localPosition;
        }
    }
}