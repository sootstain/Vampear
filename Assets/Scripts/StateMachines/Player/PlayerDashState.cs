using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDashState : PlayerBaseState
{
    private readonly int DashHash = Animator.StringToHash("Dash");
    
    private const float DashDuration = 0.3f;
    private const float DashSpeed = 20f;
    private float dashTimer;
    private Vector3 dashDirection;
    private bool hasAttacked = false;
    // It is just the 3rd attack of the attack slash
    private int slash3Index = 2;
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
        if (stateMachine.InputReader.isAttacking && !hasAttacked)
        {
            hasAttacked = true;
            
            // Switch to attack state with Slash 3
            stateMachine.SwitchState(new PlayerDashAttackState(stateMachine, slash3Index, dashDirection, dashTimer));;
            return;
        }
        
        if (dashTimer <= 0f)
        {
            stateMachine.SwitchState(new PlayerMoveState(stateMachine));
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