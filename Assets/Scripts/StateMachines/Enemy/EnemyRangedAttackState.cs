using UnityEngine;
public class EnemyRangedAttackState : EnemyBaseState
{
    private readonly int AttackHash = Animator.StringToHash("enemyThrow");
    private const float TransitionDuration = 0.1f;
    
    public EnemyRangedAttackState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(AttackHash, TransitionDuration);

        if (stateMachine.Weapon != null)
        {
            stateMachine.Weapon.SetAttack(stateMachine.AttackDamage, stateMachine.AttackKnockback);
        }
    }

    public override void Exit()
    {
        stateMachine.IsAttacking = false;
    }

    public override void Tick(float deltaTime)
    {
        if (stateMachine.Player == null) return;
        
        FacePlayer();

        // Wait for the animation to complete playing
        bool animationComplete = stateMachine.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
        if (animationComplete)
        {
            // After ranged attack completes, the manager will handle retreat, just go to chase state after
            stateMachine.SwitchState(new EnemyChasingState(stateMachine));
        }
    }
    
}