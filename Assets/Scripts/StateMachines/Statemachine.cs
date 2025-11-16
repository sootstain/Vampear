using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    public State currentState { get; private set; }
    private void Update()
    {
        // null condition operator
        currentState?.Tick(Time.deltaTime);
    }

    public void SwitchState(State newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }
    
}
