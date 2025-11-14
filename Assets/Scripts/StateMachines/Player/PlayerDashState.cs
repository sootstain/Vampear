using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDashState : PlayerBaseState
{

    private float dashTimer;
    private Vector3 dashDirection;
    private bool hasAttacked = false;
    // It is just the 2nd attack of the attack slash
    private int slash2Index = 2;
    private AnimationCurve DashCurve;
    private Transform cameraTransform;
    public PlayerDashState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        DashCurve = stateMachine.dashCurve;
        cameraTransform = stateMachine.MainCameraPosition;
    }

    public override void Enter()
    {
        dashTimer = 0f;
        dashDirection = GetDashDirection();
        stateMachine.Animator.SetBool("Dash", true);
        stateMachine.IsInvincible = true;
        stateMachine.StartDashCooldown();
        stateMachine.ForceReceiver.enabled = false;
    }

    public override void Tick(float deltaTime)
    {
        dashTimer += deltaTime;
        float normalizedTime = dashTimer / stateMachine.DashDuration;
        float speedMultiplier = DashCurve.Evaluate(normalizedTime);
        
        Move(dashDirection * stateMachine.DashSpeed * speedMultiplier, deltaTime);
        
        if (stateMachine.InputReader.isAttacking && !hasAttacked)
        {
            hasAttacked = true;
            float remainingTime = stateMachine.DashDuration - dashTimer;
            
            // Switch to attack state with Slash 2
            stateMachine.SwitchState(new PlayerDashAttackState(stateMachine, slash2Index, dashDirection, remainingTime));
            return;
        }
        
        if (dashTimer >= stateMachine.DashDuration)
        {
            stateMachine.SwitchState(new PlayerMoveState(stateMachine));
        }
    }

    public override void Exit()
    {
        stateMachine.Animator.SetBool("Dash", false);
        stateMachine.IsInvincible = false;
        stateMachine.ForceReceiver.enabled = true;
    }

    private Vector3 GetDashDirection()
    {
        Vector2 input = stateMachine.InputReader.MovementValue;
        if (input.magnitude < 0.1f)
        {
            return stateMachine.transform.forward;
        }
        Vector3 moveDir = new Vector3(input.x, 0f, input.y).normalized;
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        
        return (forward * moveDir.z + right * moveDir.x).normalized;
    }
    
}