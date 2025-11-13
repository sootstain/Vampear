using UnityEngine;

public class PlayerImpactState : PlayerBaseState
{
    private float duration = 1f;
    
    public PlayerImpactState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }
    
    public override void Enter()
    {
        stateMachine.Animator.SetTrigger("Hurt");
    }

    public override void Exit()
    {
    }

    public override void Tick(float deltaTime)
    {
        Move(deltaTime);
        
        duration -= deltaTime;

        if (duration <= 0f)
        {
            ReturnToMoveStates();
        }
    }
}
