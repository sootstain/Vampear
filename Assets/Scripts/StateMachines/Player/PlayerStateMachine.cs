using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    [field: SerializeField] public InputReader InputReader { get; private set; }
    [field: SerializeField] public BatInputReader BatInputReader { get; private set; }

    [field: SerializeField] public CharacterController CharacterController { get; private set; }
    [field: SerializeField] public float StandardMovementSpeed { get; private set; }
    [field: SerializeField] public float TargetingMovementSpeed { get; private set; }

    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField] public Attack[] Attacks { get; private set; }
    public Transform MainCameraPosition { get; private set; }
    
    [field: SerializeField] public float WhipLength;
    [field: SerializeField] public Transform WhipBase;
    [field: SerializeField] public LineRenderer WhipLine;
    
    [field: SerializeField] public Targeter Targeter { get; private set; }

    [field: SerializeField] public GameObject PlayerMesh { get; private set; }
    [field: SerializeField] public GameObject BatMesh { get; private set; }

    [field: SerializeField] public float JumpForce { get; private set; }
    [field: SerializeField] public float RotationDamping { get; private set; }

    [field: SerializeField] public AnimationCurve whipCurve { get; private set; }
    [field: SerializeField] public LedgeDetection LedgeDetection { get; private set; }
    
    [field: SerializeField] public float interactionDistance;
    
    [field: SerializeField] public GameObject visualSpherePrefab;
    
    private bool isVampire;
    private void Start()
    {
        if (PlayerMesh.activeInHierarchy) isVampire = true;
        
        MainCameraPosition = Camera.main.transform;
        SwitchState(new PlayerMoveState(this));
        InputReader.TransformEvent += OnTransform;
    }

    private void OnTransform()
    {
        if (isVampire)
        {
            PlayerMesh.SetActive(false);
            BatMesh.SetActive(true);
            isVampire = false;
        }
        else
        {
            PlayerMesh.SetActive(true);
            BatMesh.SetActive(false);
            isVampire = true;
        }
    }
}
