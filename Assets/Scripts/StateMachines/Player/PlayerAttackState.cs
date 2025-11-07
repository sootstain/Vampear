using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{

    private Attack attack;
    private float previousFrameTime;
    private bool forceApplied;
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
        
        float normalisedTime = GetNormalisedTime(stateMachine.Animator);

        if (normalisedTime >= previousFrameTime && normalisedTime < 1f)
        {
            if (normalisedTime >= attack.ForceTime)
            {
                ApplyForce();
            }
            
            else if (stateMachine.InputReader.isAttacking)
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

    //Move forwards
    private void ApplyForce()
    {
        if (forceApplied) return;
        stateMachine.ForceReceiver.AddForce(stateMachine.transform.forward * attack.ForceStrength);
        forceApplied = true;
    }
}
