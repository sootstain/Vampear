using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    private Attack attack;
    private float previousFrameTime;
    public PlayerAttackState(PlayerStateMachine stateMachine, int comboIndex) : base(stateMachine)
    {
        attack = stateMachine.Attacks[comboIndex];
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(attack.AnimationName, attack.TransitionDuration);
    }

    public override void Exit()
    {
    }

    public override void Tick(float deltaTime)
    {
        //TODO: Face target method
        
        Move(deltaTime);
        
        float normalisedTime = GetNormalisedTime();

        if (normalisedTime >= previousFrameTime && normalisedTime < 1f)
        {
            if (stateMachine.InputReader.isAttacking)
            {
                ComboAttack(normalisedTime);
            }        
        }
        else
        {
            if (stateMachine.Targeter.CurrentTarget != null)
            {
                stateMachine.SwitchState(new PlayerTargetState(stateMachine));;
            }
            else
            {
                stateMachine.SwitchState(new PlayerMoveState(stateMachine));    
            }
        }
        
        previousFrameTime = normalisedTime;
    }

    private void ComboAttack(float normalisedTime)
    {
        
        if (attack.ComboIndex == -1) return;
        if (normalisedTime < attack.ComboAttackTime) return; //hasn't finished previous anim / previous attack
        
        stateMachine.SwitchState(new PlayerAttackState(stateMachine,attack.ComboIndex));
    }

    private float GetNormalisedTime()
    {
        AnimatorStateInfo currentStateInfo = stateMachine.Animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextStateInfo = stateMachine.Animator.GetNextAnimatorStateInfo(0);

        if (stateMachine.Animator.IsInTransition(0) && nextStateInfo.IsTag("Attack"))
        {
            return nextStateInfo.normalizedTime;
        }
        
        if (!stateMachine.Animator.IsInTransition(0) && currentStateInfo.IsTag("Attack"))
        {
            return currentStateInfo.normalizedTime;
        }
    
        return 0f;            
        
    }
}
