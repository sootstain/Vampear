using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{

    private const float SnapAngleThreshold = 120f;
    private const float RotationSpeed = 20f;
    
    private AnimationCurve jumpCurve;
    private Vector3 jumpMomentum;
    private Quaternion lockedFacingRotation;
    private float jumpTimer;
    private float speedMultiplier;

    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        jumpCurve = stateMachine.jumpCurve;
    }
    
    public override void Enter()
    { 
       //float normalizedTime = jumpTimer / stateMachine.JumpDuration;
        //float speedMultiplier = jumpCurve.Evaluate(normalizedTime);
        
        stateMachine.Animator.SetTrigger("Jump");
        //stateMachine.ForceReceiver.Jump(stateMachine.JumpForce, speedMultiplier);
        
        stateMachine.InputReader.DashEvent += OnDash;
        lockedFacingRotation = stateMachine.transform.rotation;
        
        // This calculates if the player is pressing any key of WASD
        //Vector3 inputDir = CalculateJump();
        //if (inputDir.sqrMagnitude > Mathf.Epsilon)
        //{
        //    jumpMomentum = inputDir.normalized;
        //    FaceJumpDirection(jumpMomentum, Time.deltaTime);
        //}
        //else
        //{
        //    jumpMomentum = Vector3.zero;
        //}
    }

    public override void Exit()
    {
        stateMachine.ForceReceiver.GravityEnabled = true;
        stateMachine.InputReader.DashEvent -= OnDash;
    }

    public override void Tick(float deltaTime)
    {
        jumpTimer += deltaTime;
        float normalizedTime = (jumpTimer / stateMachine.JumpDuration);
        float speedMultiplier = jumpCurve.Evaluate(normalizedTime);
        
        float jumpVel = stateMachine.JumpForce * speedMultiplier;
        stateMachine.ForceReceiver.SetJump(jumpVel);
        
        if (!stateMachine.InputReader.isJumpHeld && jumpVel > 0)
        {
            // Cut upward velocity when jump is released
            stateMachine.ForceReceiver.SetJump(
                stateMachine.ForceReceiver.GetVelocity() * 0.5f
            );
        }
        
        if (stateMachine.InputReader.isAttacking) 
        {
            stateMachine.Animator.SetBool("isAttacking", true);
        }
        //Changed from animation exit so it goes through the full thing before fall state
        //Not sure if the fall state meant to be managed in the jump animationcurve :)
        //oh well
        if (jumpTimer >= stateMachine.JumpDuration)
        {
            stateMachine.SwitchState(new PlayerFallState(stateMachine));
            return;
        }
        AirMovement(deltaTime, 1f);
        stateMachine.ForceReceiver.GravityEnabled = false;
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
