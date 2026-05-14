using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ClampedIntVariableSO", menuName = "Scriptable Objects/ClampedIntVariableSO")]
public class ClampedIntVariableSO : ScriptableObject
{
    [Header("Values")]
    [Tooltip("초기값(게임 시작 시 이 값으로 초기화됨)")]
    [SerializeField] protected int initialValue;
    [Tooltip("최소값")]
    [SerializeField] protected int minValue;
    [Tooltip("최대값")]
    [SerializeField] protected int maxValue;
    [Tooltip("런타임 값")]
    [SerializeField] protected int runtimeValue;

    //runtimeValue 수정 시 호출되는 이벤트
    public event Action<int> OnValueChanged;

    public int Min => minValue;
    public int Max => maxValue;

    //정규화된 비율(0~1). UI Fill 등에 사용.
    public float Normalized =>
        maxValue > minValue ? (float)(runtimeValue - minValue) / (maxValue - minValue) : 0f;

    //외부에서 값을 읽고 쓸 때 사용하는 프로퍼티(자동 클램프)
    public int Value
    {
        get => runtimeValue;
        set
        {
            int clamped = Mathf.Clamp(value, minValue, maxValue);
            if (runtimeValue != clamped)
            {
                int previous = runtimeValue;
                runtimeValue = clamped;
                OnValueChanged?.Invoke(runtimeValue);
                OnValueChangedHook(previous, runtimeValue);
            }
        }
    }

    protected virtual void OnEnable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        ResetValue();
    }

    //초기값으로 되돌림. 씬 재시작 등에서 수동 호출도 가능.
    public virtual void ResetValue()
    {
        runtimeValue = Mathf.Clamp(initialValue, minValue, maxValue);
        OnValueChanged?.Invoke(runtimeValue);
    }

    //파생 클래스에서 값 변경 직후 추가 처리를 하고 싶을 때 오버라이드.
    protected virtual void OnValueChangedHook(int previous, int current) { }
}
