using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    private readonly int FallRef = Animator.StringToHash("Fall");
    private const float CrossFadeDuration = 0.1f;
    private Vector3 momentum; 
    private bool isSubscribed = false;
    
    public PlayerFallState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        // Try to subscribe here
        if (stateMachine.LedgeDetection != null)
        {
            stateMachine.LedgeDetection.OnLedgeDetected += HandleLedgeDetection;
            isSubscribed = true;
            Debug.Log("Subscribed in constructor");
        }
    }

    public override void Enter()
    {
        Debug.Log("Falling");
        Debug.Log("LedgeDetection component: " + (stateMachine.LedgeDetection != null ? "Found" : "NULL!"));
        
        momentum = stateMachine.CharacterController.velocity;
        momentum.y = 0f;
        stateMachine.Animator.CrossFadeInFixedTime(FallRef, CrossFadeDuration);
        stateMachine.LedgeDetection.ResetDetection();
        
        // Subscribe here if not already subscribed
        if (!isSubscribed && stateMachine.LedgeDetection != null)
        {
            stateMachine.LedgeDetection.OnLedgeDetected += HandleLedgeDetection;
            isSubscribed = true;
            Debug.Log("Subscribed in Enter()");
        }
        
        Debug.Log("Event subscribed in Fall State");
    }

    public override void Exit()
    {
        if (isSubscribed)
        {
            stateMachine.LedgeDetection.OnLedgeDetected -= HandleLedgeDetection;
            isSubscribed = false;
        }
    }

    public override void Tick(float deltaTime)
    {
        Move(momentum, deltaTime);
        
        //if hit the ground
        if (stateMachine.CharacterController.isGrounded)
        {
            Debug.Log("Back to normal");
            ReturnToMoveStates();
            return;       
        }
        
        FaceTarget();
    }
    
    private void HandleLedgeDetection(Vector3 ledgeForward, Vector3 surfaceNormal)
    {
        Debug.Log("HANDLE LEDGE DETECTION CALLED IN FALL STATE!");
        stateMachine.SwitchState(new PlayerHangState(stateMachine, ledgeForward, surfaceNormal));
    }
}