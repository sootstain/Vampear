using UnityEngine;

public class CreateSphere : MonoBehaviour
{

    private float minRadius;
    [SerializeField] private float maxRadius;
    private GameObject currentSphere;
    [SerializeField] private GameObject spherePrefab;
    private float currentScale;
    [SerializeField] private float growthSpeed = 1f;
    
    private void Start()
    {
        currentSphere = Instantiate(spherePrefab, Vector3.zero, Quaternion.identity);

        currentSphere.transform.localScale = Vector3.one * minRadius;

        currentScale = minRadius;
    }

    private void Update()
    {
        if (currentSphere != null && currentScale < maxRadius)
        {
            currentScale += growthSpeed * Time.deltaTime;
            currentScale = Mathf.Min(currentScale, maxRadius);
            currentSphere.transform.localScale = Vector3.one * currentScale;
        }
    }
}
