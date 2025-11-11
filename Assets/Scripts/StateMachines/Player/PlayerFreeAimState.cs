
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerFreeAimState : PlayerBaseState
{
    
    private GameObject sphere;
    private readonly int Shoot = Animator.StringToHash("ChainThrow");
    
    
    
    public PlayerFreeAimState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        sphere = stateMachine.visualSpherePrefab;
    }
    
    public override void Enter()
    {
        stateMachine.InputReader.AimEvent += OnAimCancel;
        stateMachine.Animator.Play(Shoot);
    }

    public override void Exit()
    {
        //stateMachine.visualTarget.enabled = false;
        stateMachine.InputReader.AimEvent -= OnAimCancel;
    }

    private void OnAimCancel()
    {
        stateMachine.SwitchState(new PlayerMoveState(stateMachine));
    }

    public override void Tick(float deltaTime)
    {
        
        //Get the mouse pos
        Ray rayOrigin = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hitInfo;
        
        if (Physics.Raycast(rayOrigin, out hitInfo))
        {
            stateMachine.visualTarget.enabled = true;
            stateMachine.visualTarget.transform.LookAt(stateMachine.MainCameraPosition);
            stateMachine.visualTarget.transform.position = hitInfo.point;
            
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
                    
                    stateMachine.SwitchState(new PlayerThrowBellState(stateMachine, hitInfo.point));
                
                }
            }
            //else sphere time baby
        }
    }
}