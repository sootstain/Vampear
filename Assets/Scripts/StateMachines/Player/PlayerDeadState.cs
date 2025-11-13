using UnityEngine;

public class PlayerDeadState : PlayerBaseState
{
    
    public PlayerDeadState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.SetTrigger("Die");
        stateMachine.CharacterController.enabled = false;
    }

    public override void Exit()
    {
        stateMachine.CharacterController.enabled = true;
    }

    public override void Tick(float deltaTime)
    {
        Debug.Log("You dead");
        bool animationComplete = stateMachine.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.5f;
        if (animationComplete)
        {
            GameManager.Instance.ShowDeathScreen();
        }
    }
}
