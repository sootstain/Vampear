using UnityEngine;

public class EnemyAttackingState : EnemyBaseState
{
    private readonly int AttackHash = Animator.StringToHash("Attack");
    private const float TransitionDuration = 0.1f;
    public EnemyAttackingState(EnemyStateMachine stateMachine) : base(stateMachine) { }
    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(AttackHash, TransitionDuration);
        if (stateMachine.Weapon != null)
        {
            stateMachine.Weapon.SetAttack(stateMachine.AttackDamage, stateMachine.AttackKnockback);   
        }
    }

    public override void Exit() { }

    public override void Tick(float deltaTime)
    {
        if (stateMachine.Player == null) return;
        FacePlayer();
        
        if (GetNormalisedTime(stateMachine.Animator) >= 1f)
        {
            if (IsInAttackRange())
            {
                stateMachine.SwitchState(new EnemyAttackingState(stateMachine));
            }
            else
            {
                stateMachine.SwitchState(new EnemyChasingState(stateMachine));    
            }
        }
    }
}