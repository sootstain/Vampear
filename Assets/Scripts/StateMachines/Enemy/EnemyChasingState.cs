using UnityEngine;

public class EnemyChasingState : EnemyBaseState
{
    public EnemyChasingState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }
    
    private readonly int IdleHash = Animator.StringToHash("Idle");
    private readonly int SpeedHash = Animator.StringToHash("Speed");
    private const float CrossFadeDuration = 0.1f;
    private const float AnimatorDampTime = 0.1f;
    private readonly float rangedStopDistance = 8f;
    private readonly float rangedMinDistance = 5f;
    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(IdleHash, CrossFadeDuration);
    }

    public override void Exit()
    {
        if (stateMachine.Agent.hasPath)
        {
            stateMachine.Agent.ResetPath();    
        }
        stateMachine.Agent.velocity = Vector3.zero;
    }

    public override void Tick(float deltaTime)
    {
        if (stateMachine.Player == null) return;
        
        if (!IsInChaseRange())
        {
            stateMachine.SwitchState(new EnemyIdleState(stateMachine));
            return;
        }
        // Should be handled by the enemy manager now
        /*
        else if (IsInAttackRange()) //leaving this here but think it'll always go from chase -> ranged attack -> melee
        {
            stateMachine.SwitchState(new EnemyAttackingState(stateMachine));
            return;
        }
        else if (IsInRangedAttackRange())
        {
            stateMachine.SwitchState(new EnemyRangedAttackState(stateMachine));
            return;
        }
        */
 

        stateMachine.Animator.SetFloat(SpeedHash, 1f, AnimatorDampTime, deltaTime);
        MoveToPlayer(deltaTime);
        FacePlayer();
    }

    private void MoveToPlayer(float deltaTime)
    {
        if (!stateMachine.Agent.isOnNavMesh)
            return;

        float distance = Vector3.Distance(stateMachine.Player.transform.position, stateMachine.transform.position);
        
        if (stateMachine.IsRangedEnemy)
        {
            if (distance > rangedStopDistance)
            {
                stateMachine.Agent.stoppingDistance = rangedStopDistance;
                stateMachine.Agent.SetDestination(stateMachine.Player.transform.position);
            }
            else if (distance < rangedMinDistance)
            {
                Vector3 dirAway = (stateMachine.transform.position - stateMachine.Player.transform.position).normalized;
                Vector3 newPos = stateMachine.transform.position + dirAway * 2f;
            
                stateMachine.Agent.stoppingDistance = 0f;
                stateMachine.Agent.SetDestination(newPos);
            }
            else
            {
                if (stateMachine.Agent.hasPath)
                    stateMachine.Agent.ResetPath();
            }

            return;
        }
        
        stateMachine.Agent.stoppingDistance = 0f;
        stateMachine.Agent.SetDestination(stateMachine.Player.transform.position);

        Move(stateMachine.Agent.desiredVelocity.normalized * stateMachine.MovementSpeed, deltaTime);
    }
}