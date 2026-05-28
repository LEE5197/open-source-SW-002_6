using UnityEngine;

public class UltCountPresenter : MonoBehaviour
{
    [SerializeField] private UltCountView ultCountView;
    [SerializeField] private UltCountSO ultCountSO;

    private void Awake()
    {
        if (!ultCountView)
        {
            Debug.LogError("[UltCountPresenter] ultCountView is not assigned");
            enabled = false;
            return;
        }
        if (!ultCountSO)
        {
            Debug.LogError("[UltCountPresenter] ultCountSO is not assigned");
            enabled = false;
            return;
        }
        ultCountView.Initialize(ultCountSO.Max);
    }

    private void OnEnable()
    {
        ultCountSO.OnValueChanged += OnUltCountChanged;
        OnUltCountChanged(ultCountSO.Value);
    }

    private void OnDisable()
    {
        ultCountSO.OnValueChanged -= OnUltCountChanged;
    }

    private void OnUltCountChanged(int value)
    {
        ultCountView.ChangeCount(value);
    }
}
