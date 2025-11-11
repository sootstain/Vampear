using System;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    //for scene triggers
    public GameEvent gameEvent;
    public bool isTriggered;

    public void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent(out PlayerStateMachine stateMachine);
        if (stateMachine == null) return;
        if (isTriggered) return;
        
        gameEvent.TriggerEvent();
        isTriggered = true;
    }
}
