using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class EnemyStateMachine : StateMachine
{
    [Header("The most important stuff goes here")]
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public CharacterController Controller { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [field: SerializeField] public WeaponDamage Weapon { get; private set; }
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public Target Target { get; private set; }
    [Header("Speed related stuff")]
    [field: SerializeField] public float MovementSpeed { get; private set; } = 3f;
    [field: SerializeField] public float StrafeSpeed { get; private set; } = 2f;
    [field: SerializeField] public float ChargeSpeed { get; private set; } = 5f;
    [field: SerializeField] public float RetreatSpeed { get; private set; } = 2f;
    
    [Header("Combat related things go in here")]
    [field: SerializeField] public float PlayerChasingRange { get; private set; } = 8f;
    [field: SerializeField] public float AttackingRange { get; private set; } = 1.2f;
    [field: SerializeField] public float RangedAttackRange { get; private set; } = 5f;
    [field: SerializeField] public float RetreatDistance { get; private set; } = 4f;
    
    [field: SerializeField] public int AttackDamage { get; private set; } = 10;
    [field: SerializeField] public int AttackKnockback { get; private set; } = 40;
    [field: SerializeField] public float BlockCooldown { get; private set; } = 5f;
    [field: SerializeField] public float BlockChance { get; private set; } = 0.2f;
    
    [Header("Particles effects")]
    [field: SerializeField] public ParticleSystem CounterParticle { get; private set; }
    
    // State flags
    public bool IsPreparingAttack { get; set; }
    public bool IsBlocking { get; set; }
    public bool IsLockedTarget { get; set; }
    
    private float lastBlockTime = -999f;
    //private EnemyManager enemyManager;
    
    public GameObject Player { get; private set; }
    //public EnemyManager EnemyManager => enemyManager;

    public float AttackRange => AttackingRange;
    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Agent.updatePosition = false;
        Agent.updateRotation = false;
        SwitchState(new EnemyIdleState(this));
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
    
    public bool CanBlock() => Time.time - lastBlockTime >= BlockCooldown;

    public void ResetBlockCooldown() => lastBlockTime = Time.time;

    private void HandleTakeDamage()
    {
        if (CanBlock() && Random.value < BlockChance)
        {
            ResetBlockCooldown();
            SwitchState(new EnemyBlockingState(this));
            return;
        }
        SwitchState(new EnemyImpactState(this));
    }
    
    public float GetDistanceToPlayer()
    {
        if (Player == null) return Mathf.Infinity;
        return Vector3.Distance(transform.position, Player.transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, PlayerChasingRange);
    }

    private void HandleDeath()
    {
        if (CounterParticle != null)
        {
            CounterParticle.Clear();
            CounterParticle.Stop();
        }
        SwitchState(new EnemyDeadState(this));
    }
    
}
