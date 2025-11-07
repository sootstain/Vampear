using UnityEngine;

public class PlayerHangState : PlayerBaseState
{
    private readonly int HangRef = Animator.StringToHash("Hang");
    private const float CrossFadeDuration = 0.1f;
    
    Vector3 closestPoint;
    Vector3 ledgeForward;
    
    public PlayerHangState(PlayerStateMachine stateMachine, Vector3 ledgeForward) : base(stateMachine)
    {
        this.ledgeForward = ledgeForward;
    }

    public override void Enter()
    {
        stateMachine.transform.rotation = Quaternion.LookRotation(ledgeForward, Vector3.up);
        stateMachine.Animator.CrossFadeInFixedTime(HangRef, CrossFadeDuration);
    }

    public override void Exit()
    {
        
    }

    public override void Tick(float deltaTime)
    {
        if (stateMachine.InputReader.MovementValue.y > 0f)
        {
            stateMachine.SwitchState(new PlayerClimbUpState(stateMachine));    
        }
        else if (stateMachine.InputReader.MovementValue.y < 0f)
        {
            stateMachine.CharacterController.Move(Vector3.zero);
            stateMachine.ForceReceiver.Reset();
            stateMachine.SwitchState(new PlayerFallState(stateMachine));       
        }
    }
}
