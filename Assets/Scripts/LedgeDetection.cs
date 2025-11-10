using System;
using UnityEngine;

public class LedgeDetection : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Detection Settings")]
    [SerializeField] private float upperDetectionHeight = 1.5f;
    [SerializeField] private float lowerDetectionHeight = 0.7f;
    [SerializeField] private float forwardDetectionOffset = 0.1f;
    //Controls the forward raycast in front. The larger the value the more you can detect ahead of you
    //This can detect edges inside the walls or cubes
    [SerializeField] private float forwardReach = 1.5f;

    [Header("Hang Position")] 
    // vertical and horizontal offset should be TIGHT for the hang position to match the animation
    // if it is too big it will look bad and it WILL latch onto walls not intended
    [SerializeField] private float verticalHangOffset = 0.2f;
    [SerializeField] private float horizontalHangOffset = 0.2f;
    
    public event Action<Vector3, Vector3> OnLedgeDetected;
    
    private Vector3 detectedHangPosition;
    private bool ledgeDetected = false;

    
    private void Update()
    {
        if (characterController.velocity.y < 0)
        {
            CheckForLedge();
        }
    }
    

    private void CheckForLedge()
    {
        
        Transform playerTransform = transform;
        
        float forwardReach = 1.5f;
        
        Vector3 lineDownStart = playerTransform.position + Vector3.up * upperDetectionHeight + playerTransform.forward * forwardReach;
        Vector3 lineDownEnd = playerTransform.position + Vector3.up * lowerDetectionHeight + playerTransform.forward * forwardReach;


        if (Physics.Linecast(lineDownStart, lineDownEnd, out RaycastHit downHit, groundLayer))
        {

            Vector3 lineFwdStart = new Vector3(playerTransform.position.x, downHit.point.y - forwardDetectionOffset,
                playerTransform.position.z);
            Vector3 lineFwdEnd = lineFwdStart + playerTransform.forward;

            if (Physics.Linecast(lineFwdStart, lineFwdEnd, out RaycastHit fwdHit, groundLayer))
            {

                if (!ledgeDetected)
                {
                    Vector3 hangPos = new Vector3(
                        fwdHit.point.x,
                        downHit.point.y,
                        fwdHit.point.z
                    );
                    hangPos += fwdHit.normal * horizontalHangOffset;
                    hangPos += Vector3.down * verticalHangOffset;

                    detectedHangPosition = hangPos;
                    Vector3 ledgeForward = -fwdHit.normal;
                    Vector3 surfaceNormal = fwdHit.normal;
    
                    ledgeDetected = true;
    
                    OnLedgeDetected?.Invoke(ledgeForward, surfaceNormal);
                }
            }
        }
    }

    public Vector3 GetHangPosition()
    {
        return detectedHangPosition;
    }

    public void ResetDetection()
    {
        ledgeDetected = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (characterController == null) return;
        
        Transform playerTransform = transform;
        
        // Draw detection zones
        Gizmos.color = Color.yellow;
        Vector3 upperPoint = playerTransform.position + Vector3.up * upperDetectionHeight + playerTransform.forward;
        Vector3 lowerPoint = playerTransform.position + Vector3.up * lowerDetectionHeight + playerTransform.forward;
        Gizmos.DrawLine(upperPoint, lowerPoint);
        Gizmos.DrawWireSphere(upperPoint, 0.1f);
        Gizmos.DrawWireSphere(lowerPoint, 0.1f);
    }
}