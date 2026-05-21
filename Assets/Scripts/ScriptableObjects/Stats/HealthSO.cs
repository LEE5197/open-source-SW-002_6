using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthSO", menuName = "Scriptable Objects/HealthSO")]
public class HealthSO : ClampedIntVariableSO
{
    [Header("Events")]
    [SerializeField] private GameEvent deathEvent;
    
    //체력이 0(최소값) 이하로 떨어지는 순간 1회 호출됨.
    public event Action OnDeath;

    public bool IsDead => runtimeValue <= minValue;
    public bool IsFull => runtimeValue >= maxValue;

    public void Damage(int amount)
    {
        if (amount <= 0) return;
        Value -= amount;
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        Value += amount;
    }

    protected override void OnValueChangedHook(int previous, int current)
    {
        if (previous > minValue && current <= minValue)
        {
            OnDeath?.Invoke();
            if (deathEvent != null)
            {
                deathEvent.Raise();
            }
        }
    }
}
