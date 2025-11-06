using UnityEngine;

public class OverlapRevealer : MonoBehaviour
{
    //For the player

    [SerializeField] private float radius = 5f;
    public float hardness = 0.5f;
    public float strength = 1f;
    public Color paintColor = Color.white;
    public LayerMask layerMaskSphere; //so this doesn't hit the player
    private void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, layerMaskSphere);

        foreach (Collider col in hitColliders)
        {
            if (col.TryGetComponent(out Paintable p))
            {
                Vector3 closestPoint = col.ClosestPoint(transform.position);
                ColourManager.instance.paint(p, closestPoint, radius, hardness, strength, paintColor);
            }
        }
    }
}
