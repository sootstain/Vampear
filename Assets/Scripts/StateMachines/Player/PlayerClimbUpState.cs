using UnityEngine;

public class PlayerClimbUpState : PlayerBaseState
{
    
    private readonly int ClimbUpRef = Animator.StringToHash("ClimbUp");
    private const float CrossFadeDuration = 0.1f;
    
    public Vector3 Offset; //Based on the chosen animation, could be fixed with root motion I think
    //but I'm dumb :)
    
    public PlayerClimbUpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        Offset = stateMachine.Offset;
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(ClimbUpRef, CrossFadeDuration);
    }

    public override void Exit()
    {
        //return control to player
        stateMachine.CharacterController.Move(Vector3.zero);
        stateMachine.ForceReceiver.Reset();
    }

    public override void Tick(float deltaTime)
    {
        if (stateMachine.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) return;

        stateMachine.CharacterController.enabled = false;
        stateMachine.transform.Translate(Offset, Space.Self);
        stateMachine.CharacterController.enabled = true;
        
        stateMachine.SwitchState(new PlayerMoveState(stateMachine, false));
    }
}
