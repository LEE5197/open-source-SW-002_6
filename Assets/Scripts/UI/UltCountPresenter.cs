using System;
using UnityEngine;

public class UltCountPresenter : MonoBehaviour
{
    [SerializeField]private UltCountView ultCountView;
    [SerializeField]private UltCountSO ultCountSO;

    private void Awake()
    {
        if (!ultCountView||!ultCountSO)
        {
            Debug.LogError("[UltCountPresenter] Var not found");
            enabled = false;
            return;
        }
        ultCountView.Initialize(ultCountSO.Max);
    }

    private void OnEnable()
    {
        ultCountSO.OnValueChanged += UIUpdate;
    }

    private void OnDisable()
    {
        ultCountSO.OnValueChanged -= UIUpdate;
    }

    private void UIUpdate(int value)
    {
        ultCountView.ChangeCount(value);
    }
}
