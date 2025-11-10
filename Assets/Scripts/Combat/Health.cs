using System.Collections;
using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;

    public event Action OnTakeDamage;
    public event Action OnDeath;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;        
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0)
        {
            Die();
            OnDeath?.Invoke();
        }

        if (TryGetComponent<EnemyStateMachine>(out var enemy) && enemy.IsBlocking)
        {
            damage = Mathf.RoundToInt(damage * 0.5f);
        }
        Debug.Log(currentHealth);
        currentHealth = Mathf.Max(0, currentHealth - damage);
        
        OnTakeDamage?.Invoke();
        
    }

    private void Die()
    {
        //FOR NOW, JUST TESTING
        //Add death anim

        TryGetComponent(out PlayerStateMachine stateMachine);
        if (stateMachine != null) return;
        
        Destroy(gameObject);
    }
}
