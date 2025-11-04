using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BatInputReader : MonoBehaviour, Controls.IBatActions
{
    public Vector2 MovementValue { get; private set; }
    public event Action BatAttackEvent;
    public event Action BatDiveEvent;
    public event Action BatScreechEvent;
    public event Action BatTransformEvent;
    public void OnMove(InputAction.CallbackContext context)
    {
        MovementValue = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        //Just needed for cinemachine
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        BatAttackEvent?.Invoke();
    }

    public void OnSoar(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnDive(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnScreech(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnTransform(InputAction.CallbackContext context)
    {

    }
}
