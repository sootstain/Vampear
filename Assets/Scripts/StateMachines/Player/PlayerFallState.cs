using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    private readonly int FallRef = Animator.StringToHash("Fall");
    private const float CrossFadeDuration = 0.1f;
    private Vector3 momentum; 
    
    public PlayerFallState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        
    }

    public override void Enter()
    {
        momentum = stateMachine.CharacterController.velocity;
        momentum.y = 0f;
        stateMachine.Animator.CrossFadeInFixedTime(FallRef, CrossFadeDuration);

        stateMachine.LedgeDetection.OnLedgeDetected += HandleLedgeDetection;
    }

    public override void Exit()
    {
        stateMachine.LedgeDetection.OnLedgeDetected -= HandleLedgeDetection;
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
    
    private void HandleLedgeDetection(Vector3 ledgeForward)
    {
        stateMachine.SwitchState(new PlayerHangState(stateMachine, ledgeForward));
    }
}
