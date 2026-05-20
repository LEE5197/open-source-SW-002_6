using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "Scriptable Objects/Events/GameEvent")]
public class GameEvent : ScriptableObject
{
    private event System.Action onEventRaised;
    private readonly List<GameEventListener> eventListeners = new List<GameEventListener>();

    public void Raise()
    {
        onEventRaised?.Invoke();

        for (int i = eventListeners.Count - 1; i >= 0; i--)
        {
            eventListeners[i].OnEventRaised();
        }
    }

    public void RegisterListener(System.Action listener)
    {
        onEventRaised += listener;
    }

    public void UnregisterListener(System.Action listener)
    {
        onEventRaised -= listener;
    }

    public void RegisterListener(GameEventListener listener)
    {
        if (!eventListeners.Contains(listener))
        {
            eventListeners.Add(listener);
        }
    }

    public void UnregisterListener(GameEventListener listener)
    {
        if (eventListeners.Contains(listener))
        {
            eventListeners.Remove(listener);
        }
    }
}
