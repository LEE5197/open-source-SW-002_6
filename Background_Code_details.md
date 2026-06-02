# Background.cs 상세 설명
코드 상세 설명<br/>
[Player](./Player_Code_Details.md)<br/>
[Item](./Item_Code_Details.md)<br/>
[Enemy](./Enemy_Code_details.md)<br/>
[GameManager](./GameManager_Code_details.md)<br/>
[UI](./UI_Code_details.md)<br/>
[README](./README.md)<br/>

`Background.cs`는 슈팅 게임에서 화면이 아래로 끊임없이 스크롤되는 무한 루프 배경을 제어하는 클래스입니다. 레이어별로 이동 속도를 다르게 설정하여 2D 게임에서 깊이감과 공간감을 만들어내고 배경을 재배치하여 무한한 배경을 구성하는 **Infinite Parallax Scrolling**를 적용했습니다.

~~~csharp
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
}
~~~
Background.cs 클래스의 상단에서 초기화한 변수들로 무한 스크롤링 구조를 만들기 위한 배경 레이어별 배열 변수들을 선언합니다. 배경은 카메라와 가까운 순서대로 **top(상단)**, **middle(중단)**, **bottom(하단)**으로 분리하여 관리하며, 입체적인 원근감을 연출하기 위해 레이어마다 각기 다른 이동 속도 (3f, 4f, 5f)를 설정합니다.


<br/>


~~~csharp
private void FixedUpdate()
{
    Move();
}
~~~
프레임률이 변하더라도 배경이 밀리거나 찢어지는 현상 없이 일정한 속도로 부드럽게 스크롤되도록 보장하기 위해, 배경 이동 로직인 `Move()` 함수를 유니티의 독립 물리 루프인 FixedUpdate에서 실행합니다.


<br/>


~~~csharp
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
~~~
배경 오브젝트들을 아래로 이동시키고, 화면 경계선을 벗어나면 다시 최상단으로 리포지셔닝하여 무한 루프를 형성하는 핵심 연산 함수입니다.<br/>
**for** 루프를 순회하며 **bottom**, **middle**, **top** 배열에 등록된 모든 배경 오브젝트의 현재 위치 `curPos`를 가져와 아래쪽 방향(Vector2.down)으로 각각 지정된 속도와 물리 델타 타임(Time.fixedDeltaTime)을 연산해 실시간으로 하강시킵니다. 플레이어 시점에서는 가장 빠르게 움직이는 `top` 레이어가 가장 가깝게 느껴지고, 느리게 스크롤되는 `bottom` 레이어가 멀리 있는 것처럼 보이는 2D 시차 효과가 발생합니다.<br/>
기존 Player.cs, Enemy.cs, ~Bullet.cs와 다르게 Rigidbody2D 컴포넌트를 사용하지 않고 transform 컴포넌트를 이용하여 이동을 구현했기 때문에, Time.fixedDeltaTime을 이용했습니다.<br/>
이후 각 배경 오브젝트의 Y축 좌표가 하단 임계점인 **-40f**보다 작아졌는지 검사합니다. 화면 밖 영역으로 완전히 빠져나간 배경 스프라이트는 플레이어 몰래 위쪽 대기 영역인 **(0, 80)** 위치로 재배치하여, 배경의 끊김 없이 지속되는 무한 스크롤 스태이지 환경을 최소한의 리소스로 구현했습니다.<br/>
