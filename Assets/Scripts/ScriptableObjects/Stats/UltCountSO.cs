using UnityEngine;
[CreateAssetMenu(fileName = "UltCountSO", menuName = "Scriptable Objects/UltCountSO")]

public class UltCountSO : ClampedIntVariableSO
{
    bool UseUlt()
    {
        int before = Value;
        Value--;
        return before==Value;
    }

    void GainUlt()
    {
        Value++;
    }
}
