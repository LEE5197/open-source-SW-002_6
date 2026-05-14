using UnityEngine;

[CreateAssetMenu(fileName = "ScoreSO", menuName = "Scriptable Objects/ScoreSO")]
public class ScoreSO : ClampedIntVariableSO
{
    //점수 증가(0 이하 입력은 무시)
    public void AddScore(int amount)
    {
        if (amount <= 0) return;
        Value += amount;
    }
}
