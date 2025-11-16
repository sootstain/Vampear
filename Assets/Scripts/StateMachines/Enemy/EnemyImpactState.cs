using UnityEngine;

public class EnemyImpactState : EnemyBaseState
{
    private readonly int ImpactHash = Animator.StringToHash("Impact");
    private const float CrossFadeDuration = 0.1f;
    private float duration = 1f;
    
    private const float KnockbackDistance = 0.5f;
    private const float KnockbackDuration = 0.3f;
    private float stunTimer;
    private bool knockbackApplied;
    public EnemyImpactState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }
    
    public override void Enter()
    {
        stunTimer = 0f;
        knockbackApplied = false;
        
        // Apply knockback force
        ApplyKnockback();
        stateMachine.Animator.CrossFadeInFixedTime(ImpactHash, CrossFadeDuration);
    }

    public override void Exit()
    {
    }

    public override void Tick(float deltaTime)
    {
        Move(deltaTime);
        duration -= deltaTime;
        FacePlayer();
        if (duration <= 0f)
        {
            stateMachine.SwitchState(new EnemyIdleState(stateMachine));
        }
    }
    private void ApplyKnockback()
    {
        if (knockbackApplied) return;
        
        Vector3 knockbackDirection = (stateMachine.transform.position - stateMachine.Player.transform.position).normalized;
        knockbackDirection.y = 0f;
        stateMachine.ForceReceiver.AddForce(knockbackDirection * KnockbackDistance / KnockbackDuration);
        
        knockbackApplied = true;
    }
}