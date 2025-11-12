using UnityEngine;

public class EnemyDeadState : EnemyBaseState
{
    private readonly int Dead = Animator.StringToHash("enemyDie");
    public EnemyDeadState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        if (stateMachine.Weapon != null)
        {
            stateMachine.Weapon.gameObject.SetActive(false);    
        }
        
        GameObject.Destroy(stateMachine.Target); //so you cannot lock on anymore
        stateMachine.Animator.Play(Dead); //SCE can crossfade this uwu :)
    }

    public override void Exit()
    {
        
    }

    public override void Tick(float deltaTime)
    {
        
    }
}
