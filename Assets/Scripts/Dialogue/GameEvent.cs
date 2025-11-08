using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Vampear/Game Event")]
public class GameEvent : ScriptableObject
{
    List<GameEventListener> listeners = new List<GameEventListener>();
    public bool triggered = false;

    public void TriggerEvent()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventTriggered();
    }

    public void AddListener(GameEventListener listener)
    {
        triggered = false;
        listeners.Add(listener);
    }

    public void RemoveListener(GameEventListener listener)
    {
        listeners.Remove(listener);
    }
}