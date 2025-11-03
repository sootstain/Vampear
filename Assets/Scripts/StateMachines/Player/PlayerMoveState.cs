using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    private readonly int MoveSpeedAnimRef = Animator.StringToHash("MoveSpeed"); //readonly for anim
    
    private readonly int MoveBlendTree = Animator.StringToHash("MoveBlendTree"); //for camera change

    
    private float timer;
    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.InputReader.TargetEvent += OnTarget;
        stateMachine.Animator.Play(MoveBlendTree);
    }

    public override void Exit()
    {
        stateMachine.InputReader.TargetEvent -= OnTarget;
    }
    
    private void OnTarget()
    {
        if (!stateMachine.Targeter.SelectTarget()) return; //no targets in range to target
        
        stateMachine.SwitchState(new PlayerTargetState(stateMachine));
    }

    public override void Tick(float deltaTime)
    {
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

    private Vector3 CalculateMovement()
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

    private void FaceMoveDirection(Vector3 movement, float deltaTime)
    {
        stateMachine.transform.rotation = Quaternion.Lerp
            (stateMachine.transform.rotation, 
            Quaternion.LookRotation(movement), 
            deltaTime * stateMachine.RotationDamping);
    }
}
