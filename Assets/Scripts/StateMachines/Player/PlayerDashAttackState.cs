using UnityEngine;

public class PlayerDashAttackState : PlayerBaseState
{
    private Attack attack;
    private float previousFrameTime;
    private bool forceApplied;
    private Vector3 dashDirection;
    private const float DashAttackSpeed = 15f;
    private float remainingDashTime;
    
    public PlayerDashAttackState(PlayerStateMachine stateMachine, int attackIndex, Vector3 dashDir, float remainingTime)
        : base(stateMachine)
    {
        attack = stateMachine.Attacks[attackIndex];
        dashDirection = dashDir;
        remainingDashTime = Mathf.Max(0f, remainingTime);
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(attack.AnimationName, attack.TransitionDuration);
        stateMachine.IsInvincible = true;
    }

    public override void Exit()
    {
        stateMachine.IsInvincible = false;
    }

    public override void Tick(float deltaTime)
    {
        // Continue moving only for the rest of the dash
        if (remainingDashTime > 0f)
        {
            Move(dashDirection * DashAttackSpeed, deltaTime);
            remainingDashTime -= deltaTime;
        }

        float normalisedTime = GetNormalisedTime(stateMachine.Animator);

        if (normalisedTime >= previousFrameTime && normalisedTime < 1f)
        {
            if (normalisedTime >= attack.ForceTime)
            {
                ApplyForce();
            }

            if (stateMachine.InputReader.isAttacking)
            {
                ComboAttack(normalisedTime);
            }
        }
        else
        {
            stateMachine.SwitchState(new PlayerMoveState(stateMachine));
        }

        previousFrameTime = normalisedTime;
    }

    private void ComboAttack(float normalisedTime)
    {
        if (attack.ComboIndex == -1) return;
        if (normalisedTime < attack.ComboAttackTime) return;
        
        stateMachine.SwitchState(new PlayerAttackState(stateMachine, attack.ComboIndex));
    }

    private void ApplyForce()
    {
        if (forceApplied) return;
        stateMachine.ForceReceiver.AddForce(dashDirection * attack.ForceStrength);
        forceApplied = true;
    }
}