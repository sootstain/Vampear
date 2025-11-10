using UnityEngine;

public abstract class PlayerBaseState : State
{
    protected PlayerStateMachine stateMachine;
    
    private const float SnapAngleThreshold = 120f;
    private const float RotationSpeed = 20f;
    public PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected void Move(float deltaTime)
    {
        //Move but not with input
        Move(Vector3.zero, deltaTime);
    }

    protected void Move(Vector3 motion, float deltaTime)
    {
        stateMachine.CharacterController.Move((motion + stateMachine.ForceReceiver.Movement) * deltaTime);
    }
    
    protected void FaceTarget()
    {
        if(stateMachine.Targeter.CurrentTarget == null) return;
        
        Vector3 lookPos = stateMachine.Targeter.CurrentTarget.transform.position - stateMachine.transform.position;
        lookPos.y = 0;
        //lookPos.Normalize();
        stateMachine.transform.rotation = Quaternion.LookRotation(lookPos); 
    }

    protected void ReturnToMoveStates()
    {
        if (stateMachine.Targeter.CurrentTarget != null)
        {
            stateMachine.SwitchState(new PlayerTargetState(stateMachine));  
        }
        else
        {
            stateMachine.SwitchState(new PlayerMoveState(stateMachine));
        }
        
    }

    protected Vector3 CalculateMovement()
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

    protected void FaceMoveDirection(Vector3 movement, float deltaTime)
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
    
}
