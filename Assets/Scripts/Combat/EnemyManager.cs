using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private EnemyStateMachine[] enemies;
    public EnemyStruct[] allEnemies;
    private List<int> enemyIndexes;

    [Header("Main AI Loop - Settings")]
    private Coroutine AI_Loop_Coroutine;

    public int aliveEnemyCount;
    
    void Start()
    {
        enemies = GetComponentsInChildren<EnemyStateMachine>();

        allEnemies = new EnemyStruct[enemies.Length];

        for (int i = 0; i < allEnemies.Length; i++)
        {
            allEnemies[i].enemyStateMachine = enemies[i];
            allEnemies[i].enemyAvailability = true;
        }

        StartAI();
    }

    public void StartAI()
    {
        AI_Loop_Coroutine = StartCoroutine(AI_Loop(null));
    }

    IEnumerator AI_Loop(EnemyStateMachine enemy)
    {
        if (AliveEnemyCount() == 0)
        {
            StopCoroutine(AI_Loop(null));
            yield break;
        }

        yield return new WaitForSeconds(Random.Range(.5f, 1.5f));

        EnemyStateMachine attackingEnemy = RandomEnemyExcludingOne(enemy);

        if (attackingEnemy == null)
            attackingEnemy = RandomEnemy();

        if (attackingEnemy == null)
            yield break;
            
        // Wait until enemy is not retreating
        yield return new WaitUntil(() => !(attackingEnemy.currentState is EnemyRetreatState));
        
        // Wait until enemy is not locked as a target
        yield return new WaitUntil(() => attackingEnemy.IsLockedTarget == false);
        
        // Wait until enemy is not stunned (in impact state)
        yield return new WaitUntil(() => !(attackingEnemy.currentState is EnemyImpactState));

        // Start the attack
        attackingEnemy.StartPrepareAttack();

        // Wait until attack preparation is complete
        yield return new WaitUntil(() => attackingEnemy.IsPreparingAttack == false);

        // Make enemy retreat
        attackingEnemy.StartRetreat();

        yield return new WaitForSeconds(Random.Range(0, .5f));

        if (AliveEnemyCount() > 0)
            AI_Loop_Coroutine = StartCoroutine(AI_Loop(attackingEnemy));
    }

    public EnemyStateMachine RandomEnemy()
    {
        enemyIndexes = new List<int>();

        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyAvailability)
                enemyIndexes.Add(i);
        }

        if (enemyIndexes.Count == 0)
            return null;

        EnemyStateMachine randomEnemy;
        int randomIndex = Random.Range(0, enemyIndexes.Count);
        randomEnemy = allEnemies[enemyIndexes[randomIndex]].enemyStateMachine;

        return randomEnemy;
    }

    public EnemyStateMachine RandomEnemyExcludingOne(EnemyStateMachine exclude)
    {
        enemyIndexes = new List<int>();

        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyAvailability && allEnemies[i].enemyStateMachine != exclude)
                enemyIndexes.Add(i);
        }

        if (enemyIndexes.Count == 0)
            return null;

        EnemyStateMachine randomEnemy;
        int randomIndex = Random.Range(0, enemyIndexes.Count);
        randomEnemy = allEnemies[enemyIndexes[randomIndex]].enemyStateMachine;

        return randomEnemy;
    }

    public int AvailableEnemyCount()
    {
        int count = 0;
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyAvailability)
                count++;
        }
        return count;
    }

    public bool AnEnemyIsPreparingAttack()
    {
        foreach (EnemyStruct enemyStruct in allEnemies)
        {
            if (enemyStruct.enemyStateMachine.IsPreparingAttack)
            {
                return true;
            }
        }
        return false;
    }

    public int AliveEnemyCount()
    {
        int count = 0;
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyStateMachine.isActiveAndEnabled)
                count++;
        }
        aliveEnemyCount = count;
        return count;
    }

    public void SetEnemyAvailability(EnemyStateMachine enemy, bool state)
    {
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyStateMachine == enemy)
                allEnemies[i].enemyAvailability = state;
        }

        // Optional: Handle current target if you have an EnemyDetection system
        // if (FindObjectOfType<EnemyDetection>().CurrentTarget() == enemy)
        //     FindObjectOfType<EnemyDetection>().SetCurrentTarget(null);
    }
}

[System.Serializable]
public struct EnemyStruct
{
    public EnemyStateMachine enemyStateMachine;
    public bool enemyAvailability;
}