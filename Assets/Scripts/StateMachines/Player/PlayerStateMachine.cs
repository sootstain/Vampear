using System;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    [field: SerializeField] public InputReader InputReader { get; private set; }
    [field: SerializeField] public CharacterController CharacterController { get; private set; }
    [field: SerializeField] public float FreeLookMovementSpeed { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField] public Attack[] Attacks { get; private set; }
    public Transform MainCameraPosition { get; private set; }
    
    [field: SerializeField] public float RotationDamping { get; private set; }
    private void Start()
    {
        MainCameraPosition = Camera.main.transform;
        SwitchState(new PlayerMoveState(this));
    }
}
