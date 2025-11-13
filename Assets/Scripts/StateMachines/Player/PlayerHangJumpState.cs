using UnityEngine;

public class PlayerHangJumpState : PlayerBaseState
{
    //TODO: Currently hooked up to Front Flip; get double jump working first OR wire up differently
    //Will do after air movement / plain jump feels good.
    private readonly int JumpRef = Animator.StringToHash("Jump");
    private const float CrossFadeDuration = 0.1f;

    public PlayerHangJumpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.ForceReceiver.Jump(stateMachine.JumpForce);
        
        
        stateMachine.Animator.CrossFadeInFixedTime(JumpRef, CrossFadeDuration);
    }

    public override void Exit()
    {
    }

    public override void Tick(float deltaTime)
    {
        Vector3 movement = CalculateMovement();
        Move(movement, deltaTime);
        if (stateMachine.CharacterController.velocity.y <= 0f)
        {
            stateMachine.SwitchState(new PlayerFallState(stateMachine));
        }
        
        FaceTarget();
    }
}