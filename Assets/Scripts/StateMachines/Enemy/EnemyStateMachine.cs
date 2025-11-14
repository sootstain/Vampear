using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class EnemyStateMachine : StateMachine
{
    [field: SerializeField] public Animator Animator { get; private set; }
    
    [field: SerializeField] public CharacterController Controller { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [field: SerializeField] public WeaponDamage Weapon { get; private set; }
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public Target Target { get; private set; }
    [field: SerializeField] public float MovementSpeed { get; private set; } = 3f;
    [field: SerializeField] public float PlayerChasingRange { get; private set; } = 8f;
    [field: SerializeField] public float AttackingRange { get; private set; } = 1.2f;
    
    [field: SerializeField] public float RangedAttackRange { get; private set; } = 5f;
    [field: SerializeField] public int AttackDamage { get; private set; } = 10;
    [field: SerializeField] public int AttackKnockback { get; private set; } = 40;
    
    [field: SerializeField] public float BlockCooldown { get; private set; } = 5f;
    
    [field: SerializeField] public float BlockChance { get; private set; } = 0.2f;
    
    
    private float lastBlockTime = -999f;
    
    
    public GameObject Player { get; private set; }
    
    public bool IsBlocking { get; set; }
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, PlayerChasingRange);
    }

    private void HandleDeath()
    {
        SwitchState(new EnemyDeadState(this));
    }
}
