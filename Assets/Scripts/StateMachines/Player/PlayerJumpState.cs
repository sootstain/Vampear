using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private readonly int JumpRef = Animator.StringToHash("Jump");
    private const float CrossFadeDuration = 0.1f;
    private Vector3 momentum;

    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine) 
    {
        
    }

    public override void Enter()
    {
        stateMachine.ForceReceiver.Jump(stateMachine.JumpForce);
        momentum = stateMachine.CharacterController.velocity;
        momentum.y = 0f; //only jump determines y movement
        stateMachine.Animator.CrossFadeInFixedTime(JumpRef, CrossFadeDuration);
        stateMachine.LedgeDetection.OnLedgeDetected += HandleLedgeDetection;
    }

    public override void Exit()
    {
        stateMachine.LedgeDetection.OnLedgeDetected -= HandleLedgeDetection;
    }

    public override void Tick(float deltaTime)
    {
        Move(momentum, deltaTime);

        if (stateMachine.CharacterController.velocity.y <= 0f)
        {
            stateMachine.SwitchState(new PlayerFallState(stateMachine));
            return;
        }
        
        FaceTarget();
    }
    
    private void HandleLedgeDetection(Vector3 ledgeForward)
    {
        stateMachine.SwitchState(new PlayerHangState(stateMachine, ledgeForward));
    }
}
