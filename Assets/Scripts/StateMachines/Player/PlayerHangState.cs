using UnityEngine;

public class PlayerHangState : PlayerBaseState
{
    private readonly int HangRef = Animator.StringToHash("Hang");
    private const float CrossFadeDuration = 0.1f;
    private const float SnapSpeed = 15f;
    
    private Vector3 targetHangPosition;
    private Vector3 ledgeForward;
    private bool hasSnapped = false;
    
    public PlayerHangState(PlayerStateMachine stateMachine, Vector3 ledgeForward) : base(stateMachine)
    {
        this.ledgeForward = ledgeForward;
        this.targetHangPosition = stateMachine.LedgeDetection.GetHangPosition();
    }

    public override void Enter()
    {
        stateMachine.transform.rotation = Quaternion.LookRotation(ledgeForward, Vector3.up);
        stateMachine.Animator.CrossFadeInFixedTime(HangRef, CrossFadeDuration);
        stateMachine.ForceReceiver.Reset();
        stateMachine.CharacterController.enabled = false;
        stateMachine.transform.position = targetHangPosition;
        hasSnapped = true;
    }

    public override void Exit()
    {
        stateMachine.CharacterController.enabled = true;
        stateMachine.LedgeDetection.ResetDetection();
        stateMachine.ForceReceiver.Reset();
    }

    public override void Tick(float deltaTime)
    {
        stateMachine.transform.position = targetHangPosition;
        
        if (stateMachine.InputReader.isJumping)
        {
            stateMachine.SwitchState(new PlayerHangJumpState(stateMachine));    
        }
        else if (stateMachine.InputReader.MovementValue.y < 0f)
        {
            stateMachine.SwitchState(new PlayerFallState(stateMachine));       
        }
    }
}