using UnityEngine;

public class EnemyChasingState : EnemyBaseState
{
    public EnemyChasingState(EnemyStateMachine stateMachine) : base(stateMachine) { }
    
    private readonly int IdleHash = Animator.StringToHash("Idle");
    private readonly int SpeedHash = Animator.StringToHash("Speed");
    private const float CrossFadeDuration = 0.1f;
    private const float AnimatorDampTime = 0.1f;
    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime("IdleHash", CrossFadeDuration);
    }

    public override void Exit()
    {
        stateMachine.Agent.ResetPath();
        stateMachine.Agent.velocity = Vector3.zero;
    }

    public override void Tick(float deltaTime)
    {
        if (!IsInChaseRange())
        {
            stateMachine.SwitchState(new EnemyIdleState(stateMachine));
            return;
        }
        else if (IsInAttackRange())
        {
            stateMachine.SwitchState(new EnemyAttackingState(stateMachine));
            return;
        }
        stateMachine.Animator.SetFloat(SpeedHash, 1f, AnimatorDampTime, deltaTime);
        MoveToPlayer(deltaTime);
        FacePlayer();
    }

    private void MoveToPlayer(float deltaTime)
    {
        stateMachine.Agent.destination = stateMachine.Player.transform.position;
        Move(stateMachine.Agent.desiredVelocity.normalized * stateMachine.MovementSpeed, deltaTime);
        stateMachine.Agent.velocity = stateMachine.Controller.velocity;
    }
}