using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDashState : PlayerBaseState
{
    private readonly int DashHash = Animator.StringToHash("Dash");
    
    private const float DashDuration = 0.3f;
    private const float DashSpeed = 20f;
    private float dashTimer;
    private Vector3 dashDirection;

    public PlayerDashState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        dashTimer = DashDuration;
        dashDirection = GetDashDirection();
        stateMachine.Animator.CrossFadeInFixedTime(DashHash, 0.1f);
        stateMachine.IsInvincible = true;
        stateMachine.StartDashCooldown();
        stateMachine.ForceReceiver.enabled = false;
    }

    public override void Tick(float deltaTime)
    {
        Move(dashDirection * DashSpeed, deltaTime);
        dashTimer -= deltaTime;
        if (dashTimer <= 0f)
        {
            if (stateMachine.InputReader.isAttacking)
            {
                stateMachine.SwitchState(new PlayerAttackState(stateMachine, 0));
            }
            else if (stateMachine.InputReader.MovementValue != Vector2.zero)
            {
                stateMachine.SwitchState(new PlayerMoveState(stateMachine));
            }
            else
            {
                stateMachine.SwitchState(new PlayerMoveState(stateMachine));
            }
        }
    }

    public override void Exit()
    {
        stateMachine.IsInvincible = false;
        stateMachine.ForceReceiver.enabled = true;
    }

    private Vector3 GetDashDirection()
    {
        // Dashing in direction of movement value
        Vector2 input = stateMachine.InputReader.MovementValue;
        return stateMachine.transform.forward;
    }
}