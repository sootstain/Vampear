using UnityEngine;

public class EnemyPrepareAttackState : EnemyBaseState
{
    private readonly int LocomotionBlendTreeHash = Animator.StringToHash("Locomotion");
    private readonly int SpeedHash = Animator.StringToHash("Speed");
    
    private const float AnimatorDampTime = 0.1f;
    private const float PrepareTime = 0.2f;
    
    private float prepareTimer;
    
    public EnemyPrepareAttackState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.IsPreparingAttack = true;
        prepareTimer = 0f;
        
        // Play counter particle effect
        if (stateMachine.CounterParticle != null)
        {
            stateMachine.CounterParticle.Play();
        }
        
        stateMachine.Animator.CrossFadeInFixedTime(LocomotionBlendTreeHash, 0.1f);
    }

    public override void Tick(float deltaTime)
    {
        prepareTimer += deltaTime;
        
        FacePlayer();
        
        if (prepareTimer < PrepareTime)
        {
            return;
        }
        
        // After prepare time, charge towards player
        MoveTowardsPlayer(deltaTime);
        
        float distanceToPlayer = stateMachine.GetDistanceToPlayer();
        
        // Check if we're close enough to attack
        if (distanceToPlayer <= stateMachine.AttackingRange)
        {
            stateMachine.SwitchState(new EnemyAttackingState(stateMachine));
            return;
        }
        
        // If player is too far, cancel attack preparation
        if (distanceToPlayer > stateMachine.PlayerChasingRange)
        {
            stateMachine.SwitchState(new EnemyIdleState(stateMachine));
        }
    }

    public override void Exit()
    {
        stateMachine.IsPreparingAttack = false;
        
        if (stateMachine.CounterParticle != null)
        {
            stateMachine.CounterParticle.Clear();
            stateMachine.CounterParticle.Stop();
        }
    }
    
    private void MoveTowardsPlayer(float deltaTime)
    {
        if (stateMachine.Agent.isOnNavMesh)
        {
            stateMachine.Agent.destination = stateMachine.Player.transform.position;
            
            Move(stateMachine.Agent.desiredVelocity.normalized * stateMachine.ChargeSpeed, deltaTime);
        }
        
        stateMachine.Agent.velocity = stateMachine.Controller.velocity;
        
        stateMachine.Animator.SetFloat(SpeedHash, 1f, AnimatorDampTime, deltaTime);
    }
}