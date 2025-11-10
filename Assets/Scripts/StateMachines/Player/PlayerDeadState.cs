using UnityEngine;

public class PlayerDeadState : PlayerBaseState
{
    private readonly int Die = Animator.StringToHash("Die");
    public PlayerDeadState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.Play(Die);
    }

    public override void Exit()
    {
        
    }

    public override void Tick(float deltaTime)
    {
        bool animationComplete = stateMachine.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
        if (animationComplete)
        {
            GameManager.Instance.ShowDeathScreen();
        }
    }
}
