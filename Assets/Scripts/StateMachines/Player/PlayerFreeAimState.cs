
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFreeAimState : PlayerBaseState
{
    public PlayerFreeAimState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        GameObject sphere = stateMachine.visualSpherePrefab;
    }

    public override void Enter()
    {
        stateMachine.InputReader.AimEvent += OnAimCancel;
    }

    public override void Exit()
    {
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
                    var sphere = Object.Instantiate(stateMachine.visualSpherePrefab, hitInfo.point, Quaternion.identity);
                    stateMachine.SwitchState(new PlayerMoveState(stateMachine));
                }
            }
            //else sphere time baby
        }
    }
}

//IF WE WANT TO USE RAYCAST / NOT JUST POINT CLICK
/**/

/*private void Pull(RaycastHit ray)
{
    //non-target method
    ray.rigidbody.gameObject.TryGetComponent(out Target target);
    if(target != null)
    {
        Vector3 offset = new Vector3(0f, 0f, 2f); //TODO: lol fix for different directions
        target.GetPulled(stateMachine.WhipBase.position + offset, 1);
    }

}*/