using UnityEngine;

public abstract class State
{
    public abstract void Enter();
    
    public abstract void Exit();
    
    public abstract void Tick(float deltaTime);
    
    protected float GetNormalisedTime(Animator animator)
    {
        AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextStateInfo = animator.GetNextAnimatorStateInfo(0);

        if (animator.IsInTransition(0) && nextStateInfo.IsTag("Attack"))
        {
            return nextStateInfo.normalizedTime;
        }
        
        if (!animator.IsInTransition(0) && currentStateInfo.IsTag("Attack"))
        {
            return currentStateInfo.normalizedTime;
        }
    
        return 0f;            
        
    }
}
