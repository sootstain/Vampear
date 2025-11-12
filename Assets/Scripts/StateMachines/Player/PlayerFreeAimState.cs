
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerFreeAimState : PlayerBaseState
{
    
    private GameObject sphere;
    private readonly int Shoot = Animator.StringToHash("ChainThrow");
    private readonly int MoveSpeedAnimRef = Animator.StringToHash("MoveSpeed"); //readonly for anim
    
    private readonly int MoveBlendTree = Animator.StringToHash("ChainMovement"); //for camera change
    private RectTransform crosshair;
    
    
    public PlayerFreeAimState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        crosshair = stateMachine.visualTarget.rectTransform;
        sphere = stateMachine.visualSpherePrefab;
    }
    
    public override void Enter()
    {
        crosshair.gameObject.SetActive(true);
        stateMachine.InputReader.AimEvent += OnAimCancel;
        stateMachine.Animator.Play(Shoot);
    }

    public override void Exit()
    {
        crosshair.gameObject.SetActive(false);
        stateMachine.InputReader.AimEvent -= OnAimCancel;
    }

    private void OnAimCancel()
    {
        stateMachine.SwitchState(new PlayerMoveState(stateMachine));
    }

    public override void Tick(float deltaTime)
    {
        
        Ray rayOrigin = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hitInfo;
        
        if (Physics.Raycast(rayOrigin, out hitInfo))
        {
            //Instantiate target
            if (stateMachine.InputReader.isAttacking)
            {
                hitInfo.collider.gameObject.TryGetComponent(out Target target);
                if (target != null)
                {
                    stateMachine.Targeter.CurrentTarget = target;
                    stateMachine.SwitchState(new PlayerPullTargetState(stateMachine));
                }
                else
                {
                    stateMachine.SwitchState(new PlayerThrowBellState(stateMachine, crosshair));
                
                }
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
    }
}