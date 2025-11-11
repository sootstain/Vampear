using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{

    private Attack attack;
    private float previousFrameTime;
    private bool forceApplied;
    
    private const float ReorientSpeed = 10f;
    private const float MaxReorientAngle = 45f;
    public PlayerAttackState(PlayerStateMachine stateMachine, int comboIndex) : base(stateMachine)
    {
        attack = stateMachine.Attacks[comboIndex];
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(attack.AnimationName, attack.TransitionDuration);
        forceApplied = false;
    }

    public override void Exit() { }

    public override void Tick(float deltaTime)
    {
        FaceAttackDirection(deltaTime);
        
        Move(deltaTime);
        
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
            if (stateMachine.Targeter.CurrentTarget != null)
            {
                stateMachine.SwitchState(new PlayerTargetState(stateMachine));;
            }
            else
            {
                stateMachine.SwitchState(new PlayerMoveState(stateMachine));    
            }
        }
        
        previousFrameTime = normalisedTime;
    }

    private void ComboAttack(float normalisedTime)
    {
        
        if (attack.ComboIndex == -1) return;
        if (normalisedTime < attack.ComboAttackTime) return; //hasn't finished previous anim / previous attack
        
        stateMachine.SwitchState(new PlayerAttackState(stateMachine,attack.ComboIndex));
    }

    //Move forwards
    private void ApplyForce()
    {
        if (forceApplied) return;
        stateMachine.ForceReceiver.AddForce(stateMachine.transform.forward * attack.ForceStrength);
        forceApplied = true;
    }
    
    // Comments for SexyCookieEater to understand this big brain solutions
    private void FaceAttackDirection(float deltaTime)
    {
        // GET THE INPUT 
        Vector3 input = stateMachine.InputReader.MovementValue;
        if (input.sqrMagnitude <= Mathf.Epsilon) return;

        // Camera relative movement. Setting to 0 removes vertical and normalise makes it unit vectors
        Vector3 forward = stateMachine.MainCameraPosition.forward;
        Vector3 right = stateMachine.MainCameraPosition.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        
        // This gets the world direction the player wants to face
        Vector3 desiredDirection = (forward * input.y + right * input.x).normalized;
        // if this is zero return
        if (desiredDirection.sqrMagnitude <= Mathf.Epsilon) return;

        // Understand which way to rotate for smallest rotation possible
        Quaternion currentRot = stateMachine.transform.rotation;
        Quaternion desiredRot = Quaternion.LookRotation(desiredDirection);
        float angle = Quaternion.Angle(currentRot, desiredRot);

        if (angle > MaxReorientAngle)
        {
            // This makes it so we can't do full 180 degree turns by checking for maxreorientangle
            desiredRot = Quaternion.RotateTowards(currentRot, desiredRot, MaxReorientAngle);
        }

        // Simple LERP rotation towards desired rotation
        stateMachine.transform.rotation = Quaternion.RotateTowards(
            currentRot,
            desiredRot,
            ReorientSpeed * deltaTime * 100f // scale speed for feels
        );
    }
}
