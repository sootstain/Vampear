using System;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    //for scene triggers
    public GameEvent gameEvent;
    public bool isTriggered;

    public void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        gameEvent.TriggerEvent();
        isTriggered = true;
    }
}
