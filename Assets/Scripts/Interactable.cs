using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameEvent gameEvent;
    public bool hasBeenTriggered;

    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent(out PlayerStateMachine stateMachine);
        if (stateMachine == null) return;
        if (hasBeenTriggered) return;
        GameManager.Instance.interactDisplay.SetActive(true);
    }
    
    private void OnTriggerStay(Collider other)
    {
        other.TryGetComponent(out PlayerStateMachine stateMachine);
        if (stateMachine == null) return;
        if (stateMachine.InputReader.isReadyToInteract && !hasBeenTriggered)
        {
            GameManager.Instance.interactDisplay.SetActive(false);
            gameEvent.TriggerEvent();
            hasBeenTriggered = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        GameManager.Instance.interactDisplay.SetActive(false);
    }
}
