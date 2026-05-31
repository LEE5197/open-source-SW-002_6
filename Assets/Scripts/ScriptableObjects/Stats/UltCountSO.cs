using UnityEngine;
[CreateAssetMenu(fileName = "UltCountSO", menuName = "Scriptable Objects/UltCountSO")]

public class UltCountSO : ClampedIntVariableSO
{
    public bool UseUlt()
    {
        /*
        int before = Value;
        Value--;
        return before==Value;
        */
        if (Value < 1) return false;

        Value--;
        return true;
    }

    public void GetUlt()
    {
        Value++;
    }
}
