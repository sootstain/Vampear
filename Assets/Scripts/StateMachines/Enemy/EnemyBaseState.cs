using UnityEngine;

public abstract class EnemyBaseState : State
{
    protected EnemyStateMachine stateMachine;
    
    public EnemyBaseState(EnemyStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected bool IsInChaseRange()
    {
        float playerDistanceSqr = (stateMachine.Player.transform.position - stateMachine.transform.position).sqrMagnitude;
        return playerDistanceSqr <= stateMachine.PlayerChasingRange * stateMachine.PlayerChasingRange;
    }

    protected bool IsInAttackRange()
    {
        float playerDistanceSqr = (stateMachine.Player.transform.position - stateMachine.transform.position).sqrMagnitude;
        return playerDistanceSqr <= stateMachine.AttackingRange * stateMachine.AttackingRange;
    }

    protected bool IsInRangedAttackRange()
    {
        float playerDistanceSqr = (stateMachine.Player.transform.position - stateMachine.transform.position).sqrMagnitude;
        return (playerDistanceSqr <= stateMachine.RangedAttackRange * stateMachine.RangedAttackRange) && (playerDistanceSqr > stateMachine.AttackingRange * stateMachine.AttackingRange);
    }
    
    protected void Move(float deltaTime)
    {
        //Move but not with input
        Move(Vector3.zero, deltaTime);
    }

    protected void Move(Vector3 motion, float deltaTime)
    {
        stateMachine.Controller.Move((motion + stateMachine.ForceReceiver.Movement) * deltaTime);
    }

    protected void FacePlayer()
    {
        if(stateMachine.Player == null) return;
        
        Vector3 lookPos = stateMachine.Player.transform.position - stateMachine.transform.position;
        lookPos.y = 0;
        //lookPos.Normalize();
        stateMachine.transform.rotation = Quaternion.LookRotation(lookPos);
    }
}
