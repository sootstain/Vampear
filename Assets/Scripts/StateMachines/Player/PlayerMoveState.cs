using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    private readonly int MoveSpeedAnimRef = Animator.StringToHash("MoveSpeed"); //readonly for anim
    
    private readonly int MoveBlendTree = Animator.StringToHash("MoveBlendTree"); //for camera change

    private bool shouldFade;
    private const float CrossFadeDuration = 0.1f;
    private float timer;
    private const float SnapAngleThreshold = 120f;
    private const float RotationSpeed = 20f;
    public PlayerMoveState(PlayerStateMachine stateMachine, bool shouldFade = true) : base(stateMachine)
    {
        this.shouldFade = shouldFade;
    }

    public override void Enter()
    {
        stateMachine.InputReader.AimEvent += OnAim;
        stateMachine.InputReader.TargetEvent += OnTarget;
        stateMachine.InputReader.JumpEvent += OnJump;
        stateMachine.InputReader.DashEvent += OnDash;
        
        stateMachine.Animator.SetFloat(MoveSpeedAnimRef, 0f); //reset
        
        if (shouldFade)
        {
            stateMachine.Animator.CrossFadeInFixedTime(MoveBlendTree, CrossFadeDuration);
        }
        else
        {
            stateMachine.Animator.Play(MoveBlendTree);
        }
        
    }

    public override void Exit()
    {
        stateMachine.InputReader.AimEvent -= OnAim;
        stateMachine.InputReader.TargetEvent -= OnTarget;
        stateMachine.InputReader.JumpEvent -= OnJump;
        stateMachine.InputReader.DashEvent -= OnDash;
    }
    
    private void OnTarget()
    {
        if (!stateMachine.Targeter.SelectTarget()) return;
        stateMachine.SwitchState(new PlayerTargetState(stateMachine));
    }

    private void OnJump()
    {
        stateMachine.SwitchState(new PlayerJumpState(stateMachine));
    }

    private void OnAim()
    {
        stateMachine.SwitchState(new PlayerFreeAimState(stateMachine));
    }
    
    public override void Tick(float deltaTime)
    {
        stateMachine.UpdateDashCooldown(deltaTime);
        
        if (stateMachine.InputReader.isAttacking)
        {
            stateMachine.SwitchState(new PlayerAttackState(stateMachine, 0));
            return;
        }

        Vector3 movement = CalculateMovement();
        
        Move(movement * stateMachine.StandardMovementSpeed, deltaTime);

        if (stateMachine.InputReader.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(MoveSpeedAnimRef, 0, 0.1f, Time.deltaTime);
            return;
        }
        stateMachine.Animator.SetFloat(MoveSpeedAnimRef, 1, 0.1f, Time.deltaTime);
        
        FaceMoveDirection(movement, deltaTime);
    }

    private void OnDash()
    {
        if (stateMachine.HasDashAvailable)
        {
            stateMachine.SwitchState(new PlayerDashState(stateMachine));
        }
    }
}
