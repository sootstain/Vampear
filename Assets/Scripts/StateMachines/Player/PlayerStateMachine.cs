using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    [field: SerializeField] public Health Health { get; private set; }
    public Transform MainCameraPosition { get; private set; }
    [field: SerializeField] public float WhipLength;
    [field: SerializeField] public Transform WhipBase;
    [field: SerializeField] public LineRenderer WhipLine;
    [field: SerializeField] public Targeter Targeter { get; private set; }
    [field: SerializeField] public GameObject PlayerMesh { get; private set; }
    [field: SerializeField] public GameObject BatMesh { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }
    [field: SerializeField] public float RotationDamping { get; private set; }
    [field: SerializeField] public AnimationCurve jumpCurve;
    [field: SerializeField] public float JumpDuration { get; private set; } = 2f;
    [field: SerializeField] public AnimationCurve whipCurve { get; private set; }
    [field: SerializeField] public float DashCooldown { get; private set; } = 1f;

    [field: SerializeField] public LedgeDetection LedgeDetection;
    [field: SerializeField] public DealDamage DealDamage;
    [Header( "Air Control - Tweak speed of turning and speed of air movement" )]
    [field: SerializeField] public float AirControlSpeed { get; private set; } = 5f;
    [field: SerializeField] public float MaxAirSpeed { get; private set; } = 8f;
    [Header( "Dash - Tweak speed and duration of dash" )]
    [field: SerializeField] public float DashDuration { get; private set; } = 0.3f;

    [field: SerializeField] public float DashSpeed { get; private set; } = 20f;
    [field: SerializeField] public AnimationCurve dashCurve;
    public bool HasDashAvailable { get; private set; } = true;
    private float dashCooldownTimer;
    
    public bool IsInvincible { get; set; }
    
    [field: SerializeField] public GameObject visualSpherePrefab;
    
    [field: SerializeField] public Image visualTarget;

    [field: SerializeField] public GameObject BellGameObject;
    
    //[field: SerializeField] public Vector3 Offset = new Vector3(0f, 2.325f, 0.65f);

    [field: SerializeField] public float interactionRadius;

    [field: SerializeField] public GameObject[] chainSegments = new GameObject[7];
    
    private bool isVampire;
    private void Start()
    {
        if (PlayerMesh.activeInHierarchy) isVampire = true;
        MainCameraPosition = Camera.main.transform;
        BellGameObject.SetActive(true);

        SwitchState(new PlayerMoveState(this));
        InputReader.TransformEvent += OnTransform;
        IsInvincible = false;
    }
    
    private void OnEnable()
    {
        Health.OnTakeDamage += HandleTakeDamage;
        Health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        Health.OnTakeDamage -= HandleTakeDamage;
        Health.OnDeath -= HandleDeath;
    }

    private void HandleTakeDamage()
    {
        if (TryGetComponent<PlayerStateMachine>(out var player) && player.IsInvincible)
        {
            return;
        }
        
        SwitchState(new PlayerImpactState(this));
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

    private void HandleDeath()
    {
        SwitchState(new PlayerDeadState(this));
    }
    
    public void UpdateDashCooldown(float deltaTime)
    {
        if (!HasDashAvailable)
        {
            dashCooldownTimer -= deltaTime;
            if (dashCooldownTimer <= 0f)
            {
                HasDashAvailable = true;
            }
        }
    }

    public void StartDashCooldown()
    {
        dashCooldownTimer = DashCooldown;
        HasDashAvailable = false;
    }
}
