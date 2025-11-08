using UnityEngine;

public class EnemyBlockingState : EnemyBaseState
{
    private readonly int BlockHash = Animator.StringToHash("Block");
    private const float BlockDuration = 10f;
    private float blockTimer;

    public EnemyBlockingState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(BlockHash, 0.1f);
        blockTimer = BlockDuration;
        stateMachine.IsBlocking = true;
        stateMachine.Agent.velocity = Vector3.zero;
        stateMachine.Agent.isStopped = true;
    }

    public override void Tick(float deltaTime)
    {
        FacePlayer();
        
        // Count down block timer
        blockTimer -= deltaTime;
        if (blockTimer <= 0f)
        {
            stateMachine.SwitchState(new EnemyIdleState(stateMachine));
            return;
        }
        
        float distanceToPlayer = Vector3.Distance(
            stateMachine.transform.position, 
            stateMachine.Player.transform.position
        );
        
        if (distanceToPlayer > stateMachine.AttackRange * 2f)
        {
            stateMachine.SwitchState(new EnemyChasingState(stateMachine));
        }
    }

    public override void Exit()
    {
        stateMachine.IsBlocking = false;
        stateMachine.Agent.isStopped = false;
    }
    
}