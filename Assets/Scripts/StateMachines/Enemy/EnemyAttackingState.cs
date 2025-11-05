using UnityEngine;

public class EnemyAttackingState : EnemyBaseState
{
    private readonly int AttackHash = Animator.StringToHash("Attack");
    private const float TransitionDuration = 0.1f;
    public EnemyAttackingState(EnemyStateMachine stateMachine) : base(stateMachine) { }
    public override void Enter()
    {
        stateMachine.Weapon.SetAttack(stateMachine.AttackDamage);
        stateMachine.Animator.CrossFadeInFixedTime(AttackHash, TransitionDuration);
    }

    public override void Exit() { }

    public override void Tick(float deltaTime)
    {
        Move(deltaTime);
        if (!IsInAttackRange())
        {
            stateMachine.SwitchState(new EnemyChasingState(stateMachine));
            return;
        }
        FacePlayer();
    }
    
}