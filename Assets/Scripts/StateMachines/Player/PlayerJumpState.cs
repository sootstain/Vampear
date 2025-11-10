using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private readonly int Uppercut = Animator.StringToHash("Uppercut"); //for camera change

    private readonly int JumpRef = Animator.StringToHash("Jump");
    private const float CrossFadeDuration = 0.1f;
    private Vector3 momentum;
    
    private const float SnapAngleThreshold = 120f;
    private const float RotationSpeed = 20f;

    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine) 
    {
        
    }
    

    public override void Enter()
    {
        stateMachine.ForceReceiver.Jump(stateMachine.JumpForce);
        momentum = stateMachine.CharacterController.velocity;
        momentum.y = 0f; //only jump determines y movement
        stateMachine.Animator.CrossFadeInFixedTime(JumpRef, CrossFadeDuration);
        stateMachine.LedgeDetection.OnLedgeDetected += HandleLedgeDetection;
        
        stateMachine.InputReader.DashEvent += OnDash;
        if (stateMachine.InputReader.MovementValue.sqrMagnitude > Mathf.Epsilon)
        {
            Vector3 jumpDirection = CalculateJump();
            FaceJumpDirection(jumpDirection, Time.deltaTime);
        }
    }

    public override void Exit()
    {
        stateMachine.LedgeDetection.OnLedgeDetected -= HandleLedgeDetection;
        stateMachine.InputReader.DashEvent += OnDash;
    }

    public override void Tick(float deltaTime)
    {
        Move(momentum, deltaTime);
        if (stateMachine.InputReader.isAttacking) 
        {
            stateMachine.Animator.SetBool("isAttacking", true);
        }
        else if (stateMachine.CharacterController.velocity.y <= 0f)
        {
            stateMachine.SwitchState(new PlayerFallState(stateMachine));
            return;
        }


        
        FaceTarget();
    }
    
    private void HandleLedgeDetection(Vector3 ledgeForward)
    {
        stateMachine.SwitchState(new PlayerHangState(stateMachine, ledgeForward));
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

        return forward * stateMachine.InputReader.MovementValue.y + 
               right * stateMachine.InputReader.MovementValue.x;
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
