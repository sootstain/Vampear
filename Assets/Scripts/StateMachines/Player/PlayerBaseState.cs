using UnityEngine;

public abstract class PlayerBaseState : State
{
    protected PlayerStateMachine stateMachine;

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
    
}
