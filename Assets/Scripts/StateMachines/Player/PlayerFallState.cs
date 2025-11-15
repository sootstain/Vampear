using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    private Vector3 momentum; 
    private bool isSubscribed = false;
    
    public PlayerFallState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        // Try to subscribe here
        if (stateMachine.LedgeDetection != null)
        {
            stateMachine.LedgeDetection.OnLedgeDetected += HandleLedgeDetection;
            isSubscribed = true;
        }
        
    }

    public override void Enter()
    {
        
        stateMachine.InputReader.DashEvent += OnDash;
        Debug.Log("LedgeDetection component: " + (stateMachine.LedgeDetection != null ? "Found" : "NULL!"));
        momentum = stateMachine.CharacterController.velocity;
        momentum.y = 0f;
        stateMachine.LedgeDetection.ResetDetection();
        
        // Subscribe here if not already subscribed
        if (!isSubscribed && stateMachine.LedgeDetection != null)
        {
            stateMachine.LedgeDetection.OnLedgeDetected += HandleLedgeDetection;
            isSubscribed = true;
            Debug.Log("Subscribed in Enter()");
        }
    }

    public override void Exit()
    {
        stateMachine.InputReader.DashEvent -= OnDash;
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
            stateMachine.LandEffect.Play();
            stateMachine.Animator.SetBool("isGrounded", true);
            ReturnToMoveStates();
            return;       
        }
        FaceTarget();
    }
    
    private void HandleLedgeDetection(Vector3 ledgeForward, Vector3 surfaceNormal)
    {
        stateMachine.SwitchState(new PlayerHangState(stateMachine, ledgeForward, surfaceNormal));
    }
    private void OnDash()
    {
        if (stateMachine.HasDashAvailable)
        {
            stateMachine.SwitchState(new PlayerDashState(stateMachine));
        }
    }
}