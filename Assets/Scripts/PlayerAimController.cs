using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAimController : MonoBehaviour
{

    public float RotationDamping = 0.5f;
    public float AimSmoothing = 0.3f;
    public float AimPlaneDistance = 100f;
    //public bool RotatePlayerToAim = true;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask aimLayerMask = ~0; //all
    
    private Transform playerTransform;
    private Vector3 currentAimDirection;
    private Vector3 smoothedAimDirection;
    private Vector3 worldAimPoint;
    private Collider[] playerColliders;
    
    
    private void Start()
    {
        playerTransform = transform.parent;
        playerColliders = playerTransform.GetComponentsInChildren<Collider>();
        
        smoothedAimDirection = playerTransform.forward;
        currentAimDirection = playerTransform.forward;
    }
    
    public void UpdateAimDirection()
    {

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        RaycastHit[] hits = Physics.RaycastAll(ray, 1000f, aimLayerMask, QueryTriggerInteraction.Ignore);
        
        bool foundValidHit = false;
        float closestDistance = float.MaxValue;
        Vector3 closestPoint = Vector3.zero;
        
        foreach (RaycastHit hit in hits)
        {
            if (IsPlayerCollider(hit.collider))
                continue;
                
            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                closestPoint = hit.point;
                foundValidHit = true;
            }
        }
        
        if (foundValidHit)
        {
            worldAimPoint = closestPoint;
        }
        else
        {
            worldAimPoint = ray.origin + ray.direction * AimPlaneDistance;
        }
        
        Vector3 aimDir = worldAimPoint - playerTransform.position;
        aimDir.y = 0;
        
        if (aimDir.sqrMagnitude > 0.001f)
        {
            currentAimDirection = aimDir.normalized;
            if (AimSmoothing > 0)
            {
                smoothedAimDirection = Vector3.Slerp(
                    smoothedAimDirection, 
                    currentAimDirection, 
                    1f - AimSmoothing
                );
            }
            else
            {
                smoothedAimDirection = currentAimDirection;
            }
        }
    }
    
    private bool IsPlayerCollider(Collider col)
    {
        if (playerColliders == null || playerColliders.Length == 0)
            return false;
            
        foreach (Collider playerCol in playerColliders)
        {
            if (col == playerCol)
                return true;
        }
        return false;
    }
    
    public void RotatePlayerToAimDirection(float deltaTime)
    {
        if (playerTransform == null || smoothedAimDirection.sqrMagnitude < 0.001f)
            return;
        
        Quaternion targetRotation = Quaternion.LookRotation(smoothedAimDirection, Vector3.up);
        

        float angleDiff = Quaternion.Angle(playerTransform.rotation, targetRotation);

        if (RotationDamping > 0)
        {
            playerTransform.rotation = Quaternion.Slerp(
                playerTransform.rotation,
                targetRotation,
                Damper.Damp(1, RotationDamping, deltaTime)
            );
        }
        else
        {
            playerTransform.rotation = targetRotation;
        }
    }

}

public static class Damper
{
    public static float Damp(float initial, float dampTime, float deltaTime)
    {
        if (dampTime < Mathf.Epsilon || Mathf.Abs(initial) < Mathf.Epsilon)
            return 1;
        if (deltaTime < Mathf.Epsilon)
            return 0;
        return 1 - Mathf.Exp(-deltaTime / dampTime);
    }
}