using TMPro;
using UnityEngine;

//추후 비슷한 방식으로 int->tmp로 컨트롤러 만들 필요 있으면 추상화
public class ScoreTextController : MonoBehaviour
{
    [SerializeField] private ScoreSO score;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    void Awake()
    {
        if (!scoreText)
        {
            Debug.LogError("Score Text is null!");
            enabled = false;
            return;
        }
    }
    void OnEnable()
    {
        if (!score)
        {
            Debug.LogError("Score is null!");
            enabled = false;
            return;
        }
        score.OnValueChanged += UIUpdate;
        UIUpdate(score.Value);
    }

    void OnDisable()
    {
        if(score) score.OnValueChanged -= UIUpdate;
    }

    void UIUpdate(int value)
    {
        //ToString은 무거우므로 점수 변경 빈도 증가 시 수정 필요
        scoreText.text = value.ToString();
    }
}  
