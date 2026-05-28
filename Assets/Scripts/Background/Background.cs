using UnityEngine;

public class Background : MonoBehaviour
{
    // 배경 스프라이트 위치 담을 변수
    public Transform[] bottom;
    public Transform[] middle;
    public Transform[] top;

    // 원근감을 주기 위한 각 배경의 속도 
    [Space]
    public float bottomSpeed = 3f;
    public float middleSpeed = 4f;
    public float topSpeed = 5f;

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        for(int i = 0; i < bottom.Length; i++)
        {
            Vector2 curPos = bottom[i].transform.position;
            curPos += Vector2.down * bottomSpeed * Time.fixedDeltaTime;
            bottom[i].transform.position = curPos;

            curPos = middle[i].transform.position;
            curPos += Vector2.down * middleSpeed * Time.fixedDeltaTime;
            middle[i].transform.position = curPos;

            curPos = top[i].transform.position;
            curPos += Vector2.down * topSpeed * Time.fixedDeltaTime;
            top[i].transform.position = curPos;

            if (bottom[i].transform.position.y < -40f)
            {
                bottom[i].transform.position = new Vector2(0, 80);
            }
            if (middle[i].transform.position.y < -40f)
            {
                middle[i].transform.position = new Vector2(0, 80);
            }
            if (top[i].transform.position.y < -40f)
            {
                top[i].transform.position = new Vector2(0, 80);
            }
        }
    }
}
