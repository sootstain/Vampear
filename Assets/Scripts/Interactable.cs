using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameEvent gameEvent;
    public bool hasBeenTriggered;

    public void TriggerInteract()
    {
        if (hasBeenTriggered) return;
        gameEvent.TriggerEvent();
        hasBeenTriggered = true;
    }
}
