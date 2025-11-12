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
        
    }

    public override void Tick(float deltaTime)
    {
        if (stateMachine.Player == null) return;

        FacePlayer();

        bool animationComplete = stateMachine.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
        if (animationComplete)
        {
            /*if (IsInRangedAttackRange())
            {
                Debug.Log("Repeat this state!");
                stateMachine.SwitchState(this);
            }*/
            if (IsInAttackRange())
            {
                Debug.Log("Switching to melee");
                stateMachine.SwitchState(new EnemyAttackingState(stateMachine));
            }
            else
            {
                Debug.Log("Switching to chase");
                stateMachine.SwitchState(new EnemyChasingState(stateMachine));
            }
            
        }
    }
    
}