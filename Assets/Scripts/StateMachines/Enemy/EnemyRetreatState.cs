using UnityEngine;

public class EnemyRetreatState : EnemyBaseState
{
    private readonly int IdleHash = Animator.StringToHash("Idle");
    private readonly int SpeedHash = Animator.StringToHash("Speed");
    
    private const float AnimatorDampTime = 0.1f;
    private const float PrepDelay = 1.4f;
    
    private float prepTimer;
    private bool isRetreating;
    
    public EnemyRetreatState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        prepTimer = 0f;
        isRetreating = false;
        
        stateMachine.Animator.CrossFadeInFixedTime(IdleHash, 0.1f);
    }

    public override void Tick(float deltaTime)
    {
        // Wait for prep delay before starting retreat
        if (!isRetreating)
        {
            prepTimer += deltaTime;
            
            // Face the player during prep phase
            FacePlayer();
            
            if (prepTimer >= PrepDelay)
            {
                isRetreating = true;
            }
            
            return;
        }
        
        // Retreat movement
        float distanceToPlayer = stateMachine.GetDistanceToPlayer();
        
        // Check if we've retreated far enough
        if (distanceToPlayer >= stateMachine.RetreatDistance)
        {
            // Return to idle state after successful retreat
            stateMachine.SwitchState(new EnemyIdleState(stateMachine));
            return;
        }
        
        // Move away from player
        RetreatFromPlayer(deltaTime);
    }

    public override void Exit()
    {
        stateMachine.Animator.SetFloat(SpeedHash, 0f);
    }
    
    private void RetreatFromPlayer(float deltaTime)
    {
        if (stateMachine.Player == null) return;
        
        // Calculate retreat direction (away from player)
        Vector3 directionAwayFromPlayer = (stateMachine.transform.position - stateMachine.Player.transform.position).normalized;
        directionAwayFromPlayer.y = 0f;
        
        // Face the player while retreating (walking backwards)
        FacePlayer();
        
        // Set NavMesh destination behind the enemy
        Vector3 retreatDestination = stateMachine.transform.position + directionAwayFromPlayer * 2f;
        
        if (stateMachine.Agent.isOnNavMesh)
        {
            stateMachine.Agent.destination = retreatDestination;
        }
        
        // Move backwards
        Move(-stateMachine.transform.forward * stateMachine.RetreatSpeed, deltaTime);
        
        stateMachine.Agent.velocity = stateMachine.Controller.velocity;
        
        // Set animator to move backwards (negative speed)
        stateMachine.Animator.SetFloat(SpeedHash, -0.5f, AnimatorDampTime, deltaTime);
    }
}