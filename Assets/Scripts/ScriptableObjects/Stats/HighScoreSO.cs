using UnityEngine;

[CreateAssetMenu(fileName = "HighScoreSO", menuName = "Scriptable Objects/HighScoreSO")]
public class HighScoreSO : ScriptableObject
{
    private const string PrefsKey = "HighScore";

    public int Value { get; private set; }

    private void OnEnable()
    {
        Value = PlayerPrefs.GetInt(PrefsKey, 0);
    }

    // 현재 최고점보다 높으면 저장하고 true 반환
    public bool TrySave(int score)
    {
        if (score <= Value) return false;
        Value = score;
        PlayerPrefs.SetInt(PrefsKey, Value);
        PlayerPrefs.Save();
        return true;
    }
}
