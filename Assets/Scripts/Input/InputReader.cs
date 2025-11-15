using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, Controls.IPlayerActions
{
    public bool isAttacking { get; private set; }
    public bool isTargeting { get; set; }
    
    public bool isJumping { get; private set; }
    public bool isJumpHeld { get; private set; }
    public bool isAiming { get; set; }
    
    public bool isReadyToInteract { get; private set; }

    public Vector2 MovementValue { get; private set; }
    public event Action JumpEvent;
    public event Action DashEvent;
    public event Action TargetEvent;
    public event Action TransformEvent;
    
    public event Action InteractEvent;
    public event Action AimEvent;
    
    private Controls controls;
    private void Start()
    {
        controls = new Controls();
        controls.Player.SetCallbacks(this);
        controls.Player.Enable();
    }
    
    private void Update()
    {
        if (controls != null && controls.Player.enabled)
        {
            Vector2 readValue = controls.Player.Move.ReadValue<Vector2>();
            MovementValue = readValue;
        }
    }

    private void OnDestroy()
    {
        controls.Player.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MovementValue = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        //Func for cinemachine component
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        //Doing this way instead of event so that we can hold down attack to continue the combo
        if (context.performed) isAttacking = true;
        else if (context.canceled) isAttacking = false;
        
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started) isReadyToInteract = true;
        else if (context.canceled) isReadyToInteract = false;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isJumping = true;    // for jump start
            isJumpHeld = true;   // for variable gravity
            JumpEvent?.Invoke();
        }
        else if (context.canceled)
        {
            isJumping = false;
            isJumpHeld = false;
        }
    }

    //For potential dialogue?
    public void OnPrevious(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!context.performed) return; 
        DashEvent?.Invoke();
    }
    

    public void OnTarget(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        TargetEvent?.Invoke();
    }
    

    public void OnTransform(InputAction.CallbackContext context)
    {
        Debug.Log("Changing into Bat");
        if(!context.performed) return;
        TransformEvent?.Invoke();
    }

    public void OnEnterFreeAim(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        AimEvent?.Invoke();
    }
}
