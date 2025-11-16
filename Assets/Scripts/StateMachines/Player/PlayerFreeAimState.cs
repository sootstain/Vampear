using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFreeAimState : PlayerBaseState
{
    private GameObject sphere;
    private Transform whipBase;
    private LineRenderer lr;
    private GameObject bellObject;
    private RectTransform crosshair;
    private PlayerAimController aimController;
    
    private readonly int MoveSpeedAnimRef = Animator.StringToHash("MoveSpeed");
    
    public PlayerFreeAimState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        crosshair = stateMachine.visualTarget.rectTransform;
        sphere = stateMachine.visualSpherePrefab;
        lr = stateMachine.WhipLine;
        whipBase = stateMachine.WhipBase;
        bellObject = stateMachine.BellGameObject;
        aimController = stateMachine.AimController;
    }
    
    public override void Enter()
    {
        stateMachine.Animator.SetBool("isAiming", true);
        crosshair.gameObject.SetActive(true);
        
        // Setup whip line renderer
        lr.positionCount = 2; 
        lr.useWorldSpace = true;
        lr.SetPosition(0, whipBase.position);
        lr.SetPosition(1, bellObject.transform.position);
        
        stateMachine.InputReader.AimEvent += OnAimCancel;
    }

    public override void Exit()
    {
        stateMachine.Animator.SetBool("isAiming", false);
        crosshair.gameObject.SetActive(false);
        lr.positionCount = 0;
        stateMachine.InputReader.AimEvent -= OnAimCancel;
    }

    private void OnAimCancel()
    {
        stateMachine.SwitchState(new PlayerMoveState(stateMachine));
    }

    public override void Tick(float deltaTime)
    {
        lr.SetPosition(0, whipBase.position);
        lr.SetPosition(1, bellObject.transform.position);
        
        aimController.UpdateAimDirection();
        aimController.RotatePlayerToAimDirection(deltaTime);
    
        
        if (stateMachine.InputReader.isAttacking)
        {
            ThrowWhip();
        }

        Vector3 movement = CalculateMovement();
        
        Move(movement * stateMachine.StandardMovementSpeed, deltaTime);

        if (stateMachine.InputReader.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(MoveSpeedAnimRef, 0, 0.1f, deltaTime);
        }
        else
        {
            stateMachine.Animator.SetFloat(MoveSpeedAnimRef, 1, 0.1f, deltaTime);
        }
    }
    
    private void ThrowWhip()
    {
        Ray rayOrigin = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (Physics.Raycast(rayOrigin, out RaycastHit hitInfo))
        {
            if (hitInfo.collider.TryGetComponent(out Target target))
            {
                stateMachine.Targeter.CurrentTarget = target;
                stateMachine.SwitchState(new PlayerPullTargetState(stateMachine));
            }
            else
            {
                stateMachine.SwitchState(new PlayerThrowBellState(stateMachine, crosshair));
            }
        }
    }
}