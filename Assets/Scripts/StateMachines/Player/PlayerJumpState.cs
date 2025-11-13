using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{

    private const float SnapAngleThreshold = 120f;
    private const float RotationSpeed = 20f;
    
    private AnimationCurve jumpCurve;
    private Vector3 jumpMomentum;
    private Quaternion lockedFacingRotation;
    private float jumpTimer;
    private float jumpDuration = 2f;

    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        jumpCurve = stateMachine.jumpCurve;
    }
    
    public override void Enter()
    { 
        stateMachine.Animator.SetTrigger("Jump");
        stateMachine.ForceReceiver.Jump(stateMachine.JumpForce);
        
        stateMachine.InputReader.DashEvent += OnDash;
        lockedFacingRotation = stateMachine.transform.rotation;
        
        // This calculates if the player is pressing any key of WASD
        Vector3 inputDir = CalculateJump();
        if (inputDir.sqrMagnitude > Mathf.Epsilon)
        {
            jumpMomentum = inputDir.normalized;
            FaceJumpDirection(jumpMomentum, Time.deltaTime);
        }
        else
        {
            jumpMomentum = Vector3.zero;
        }
    }

    public override void Exit()
    {

        stateMachine.InputReader.DashEvent -= OnDash;
    }

    public override void Tick(float deltaTime)
    {
        jumpTimer += deltaTime;
        float normalizedTime = Mathf.Clamp01(jumpTimer / jumpDuration);
        float speedMultiplier = jumpCurve.Evaluate(normalizedTime);
        
        AirMovement(deltaTime, speedMultiplier);
        if (stateMachine.InputReader.isAttacking) 
        {
            stateMachine.Animator.SetBool("isAttacking", true);
        }
        else if (stateMachine.CharacterController.velocity.y <= 0f)
        {
            stateMachine.SwitchState(new PlayerFallState(stateMachine));
            return;
        }
        stateMachine.transform.rotation = lockedFacingRotation;
    }

    private void AirMovement(float deltaTime, float speedMultiplier)
    {
        Vector3 inputDir = CalculateJump();
        if (inputDir.sqrMagnitude > Mathf.Epsilon)
        {
            jumpMomentum = Vector3.Lerp(
                jumpMomentum,
                inputDir.normalized * stateMachine.MaxAirSpeed,
                stateMachine.AirControlSpeed * deltaTime
            );
        }
        
        Move(jumpMomentum * speedMultiplier, deltaTime);
        
    }
    
    private Vector3 CalculateJump()
    {
        Vector3 forward = stateMachine.MainCameraPosition.forward;
        Vector3 right = stateMachine.MainCameraPosition.right;

        //don't care about vertical pos
        forward.y = 0;
        right.y = 0;
        
        forward.Normalize();
        right.Normalize();
        
        Vector3 result = forward * stateMachine.InputReader.MovementValue.y + 
                         right * stateMachine.InputReader.MovementValue.x;
        
        return result;
    }
    
    private void FaceJumpDirection(Vector3 movement, float deltaTime)
    {
        if (movement.sqrMagnitude > Mathf.Epsilon)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            float angleDifference = Quaternion.Angle(stateMachine.transform.rotation, targetRotation);
            if (angleDifference > SnapAngleThreshold)
            {
                stateMachine.transform.rotation = targetRotation;
            }
            else
            {
                // Slerp is used
                // BUT Quaternion.RotateTowards can be used for framerate independent fast rotation
                float rotationAmount = Mathf.Min(1f, RotationSpeed * deltaTime);
                stateMachine.transform.rotation = Quaternion.Slerp(
                    stateMachine.transform.rotation, 
                    targetRotation, 
                    rotationAmount
                );
            }
        }
    }
    
    private void OnDash()
    {
        if (stateMachine.HasDashAvailable)
        {
            stateMachine.SwitchState(new PlayerDashState(stateMachine));
        }
    }
}
