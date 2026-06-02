## Enemy.cs 관련 코드 상세 설명
코드 상세 설명<br/>
[Player](./Player_Code_Details.md)<br/>
[Item](./Item_Code_Details.md)<br/>
[Enemy](./Enemy_Code_details.md)<br/>
[GameManager](./GameManager_Code_details.md)<br/>
[UI](./UI_Code_details.md)<br/>
[README](./README.md)<br/>
## Enemy.cs 상세 설명

`Enemy.cs`는 일반 적(Enemy) 오브젝트의 이동, 플레이어를 향한 공격, 피격 및 파괴 연출(이펙트/사운드), 그리고 전과 점수 가산 및 아이템 드롭 메커니즘을 담당하는 핵심 클래스입니다.

~~~csharp
public class Enemy : MonoBehaviour
{
    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;

    public float moveSpeed = 3.0f;
    public float fireDelay = 1f; //공격 속도
    public int bulletNum = 1; //연속으로 발사하는 총알 숫자 
    public int score = 10;  //처치 시 얻는 점수

    [SerializeField] private ScoreSO scoreSO;

    [Header("Audio Clips")]
    public AudioClip enemyShootClip;
    public AudioClip enemyExplosionClip;

    private Transform playerTransform;
}
~~~
`Enemy.cs` 클래스의 상단에서 초기화한 변수들로 적 캐릭터의 이동 및 피격 이팩트 제어를 위한 컴포넌트 변수, 기본 이동 속도와 연사 속도`fireDelay`, 연속 사격 탄수 `bulletNum`, 처치 시 획득할 점수 상수를 선언합니다. 또한 실시간 가산 점수 연동을 위한 스크립터블 오브젝트 에셋 `scoreSO`, 상황별 사운드 에셋, 그리고 추적 조준의 대상이 될 플레이어의 위치 정보 참조 변수 `playerTransform`를 설정합니다.

<br/>

~~~csharp
private void Awake()
{
    rigid = GetComponent<Rigidbody2D>();
    spriteRenderer = GetComponent<SpriteRenderer>();
}
~~~
런타임 최적화를 위해 주요 컴포넌트(Rigidbody2D, SpriteRenderer)를 런타임 전에 미리 변수에 할당하여 초기화합니다.

<br/>

~~~csharp
void Start()
{
    playerTransform = GameManager.Instance.playerTransform;
    if (playerTransform == null)
        playerTransform = GameObject.FindWithTag("Player").transform;

    StartCoroutine(Fire(bulletNum)); //발사 시작
}
~~~
적 오브젝트가 생성되는 시점에 최초 1회 실행되는 초기화 루틴입니다.<br/>
기본적으로 `GameManager.cs`에서 관리하는 플레이어의 위치를 참조하지만, 만약 참조할 데이터가 없다면 `FindWithTag()`를 호출하여 플레이어 오브젝트의 위치를 얻습니다.<br/>
이후 `Fire()`를 실행하여, 플레이어 위치를 조준하여 총알을 발사합니다.

## Boss.cs 상세 설명

`Boss.cs`는 스테이지 진입 60초 후 등장하는 보스 오브젝트를 담당하는 클래스입니다. 보스의 이동, 슬라이더 UI 를 통한 HP 바, 3 가지 기본 공격 패턴(원형 공격, 부채꼴 공격, 플레이어 방향 연사), 그리고 체력이 50% 미만으로 떨어졌을 때 발동하는 광폭화(Rage) 상태와 게임 클리어 이벤트 송출까지의 거대한 보스전을 제어합니다.

~~~csharp
public class Boss : MonoBehaviour
{
    private Rigidbody2D rigid;
    private SpriteRenderer render;
    [SerializeField] private ScoreSO scoreSO;
    [SerializeField] private Slider hpBar;

    public Transform playerTransform;

    [Header("Default setting")]
    public bool isRage = false;         //최대 체력이 50% 미만인지 확인
    public float maxHealth = 10000f;    //최대 체력
    private float Health = 10000f;      //현재 체력
    public float moveSpeed = 1.0f;      //이동 속도
    public float minDistX = 0.2f;       //최소 x축 거리
    public float minDistY = 6f;         //최소 y축 거리

    public int score = 1000;  //처치 시 얻는 점수

    [Space]
    [Header("Default attack parameter")]
    public float fireDelay = 2f; //공격 속도
    public float fireGap = 0.1f; //탄환 발사 간격
    private bool canAttack = true;

    [Header("Audio Clips")]
    public AudioClip BossBGMClip;

    [SerializeField] private GameEvent gameClearEvent;

    [Space]
    [Header("Fire Forward")]
    #region Fire Forward
    public int bulletNum = 10; //연속으로 발사하는 총알 숫자 
    public int fireForwardCount = 5;    //패턴 수행 횟수
    #endregion

    [Header("Fire Circle Sector")]
    #region Fire Shot
    public int fireShotBulletNum = 30;
    public float angle = 45f;
    public int fireSectorCount = 5;      //패턴 수행 횟수
    #endregion

    [Header("Fire Spree")]
    #region Fire Rapid
    public int fireRapidBulletNum = 90;
    #endregion

    [Header("Fire Circle")]
    public int fireCircleBulletNum = 359;
    public float fireCircleDelay = 1f;
    public int fireAroundCount = 5;
}
~~~
Boss.cs 클래스의 상단에서 초기화한 변수들로 컴포넌트 변수, UI 슬라이더 컴포넌트(`hpBar`), 타겟팅을 위한 플레이어 위치 참조값 `playerTransform`을 선언합니다. 보스의 밸런스를 조율하는 기본 세팅값(최대/현재 체력, 이동 속도, 플레이어와의 거리 유지를 위한 변수값)과 처치 보상 점수, 그리고 공격 주기를 규정하는 공통 파라미터가 포함됩니다. 마지막으로 보스전 전용 BGM 에셋, 스크립터블 오브젝트 기반 게임 클리어 이벤트 인스턴스 `gameClearEvent`, 그리고 전후방 및 원형 탄막 연산에 활용될 패턴별 탄수, 각도, 수행 횟수 상수들을 세부 그룹화하여 정의합니다.

<br/>

~~~csharp
private void Awake()
{
    rigid = GetComponent<Rigidbody2D>();
    render = GetComponent<SpriteRenderer>();

    if (playerTransform == null)
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    Health = maxHealth;
    hpBar.value = Health / maxHealth;
}
~~~
런타임 최적화를 위해 주요 컴포넌트(Rigidbody2D, SpriteRenderer)를 런타임 전에 미리 할당합니다.<br/>
초기 구동 시 플레이어 타겟 누락 방지를 위한 태그 탐색 백업 코드 `FindWithTag()`가 동작하여 플레이어 위치를 받아오고, 최대 체력 수치를 현재 체력에 대입해 게임을 시작합니다. 또한 체력 잔량 비율을 연산 **Health / maxHealth**하여 화면 상단 보스 UI의 HP 바 게이지를 100% 충전 상태로 동기화합니다.

<br/>

~~~csharp
private void Start()
{
    playerTransform = GameManager.Instance.playerTransform;
    if (playerTransform == null)
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    SoundManager.Instance.PlayBossBGM(BossBGMClip);
}
~~~
보스 오브젝트가 활성화된 후에 실행되는 처리 루틴입니다.<br/>
`GameManager` 및 백업용 씬 탐색으로 플레이어 참조 정보를 완벽히 확정 지은 뒤, `SoundManager`의 보스 전용 BGM 재생 기능(PlayBossBGM)을 호출하여 배경음을 보스전 BGM으로 교체합니다.

<br/>

~~~csharp
private void Update()
{
    hpBar.value = Health / maxHealth;
}
~~~
플레이어의 누적 공격이나 궁극기에 의해 실시간으로 가변하는 보스의 현재 체력 비율을 매 프레임 연산하여 보스 전용 HP 슬라이더 UI에 직관적이고 즉각적으로 반영하는 프레임 업데이트 루프입니다.

~~~csharp
private void FixedUpdate()
{
    Move();
    if (canAttack) Attack();
}
~~~
프레임률이 변하더라도 일정한 보스의 비행 궤적과 정확한 탄막 타이밍을 보장하기 위해, 플레이어 추적 이동 함수 `Move()` 및 상태 기반 공격 패턴 트리거 함수 `Attack()`을 `canAttack`의 값에 따라 유니티의 독립 물리 루프인 FixedUpdate에서 실시간 실행합니다.

<br/>

~~~csharp
private void Attack()
{
    canAttack = false;
    int patternCnt = 3;
    int num = Random.Range(0, patternCnt);

    switch (num)
    {
        case 0:
            StartCoroutine(FireForward());
            break;
        case 1:
            StartCoroutine(FireShot());
            break;
        case 2:
            StartCoroutine(FireAround());
            break;
        case 3:
            break;
    }
}
~~~
보스의 공격 패턴을 무작위로 선택하여 작동시키는 난수 기반 공격 패턴 함수입니다.<br/>
중복 패턴 중첩 실행을 막기 위해 실행 즉시 공격 가능 플래그 `canAttack`을 `false`로 변경합니다. 이후 `Random.Range`를 통해 임의의 난수 번호를 생성하고 `switch-case`문을 통해 전방 사격(`FireForward`), 부채꼴 사격(`FireShot`), 원형 사격(`FireAround`) 코루틴 중 하나를 무작위로 골라 실행합니다.

<br/>

~~~csharp
IEnumerator FireAround()
{
    float angleOffset = 0;
    for (int cnt = 0; cnt < fireAroundCount; cnt++) 
    {
        angleOffset += 15f * Mathf.Deg2Rad;
        yield return new WaitForSeconds(fireCircleDelay);
        for (int i = 0; i < fireCircleBulletNum; i++)
        {
            EnemyBullet bullet = GameManager.Instance.GetEnemyBullet();
            if (bullet == null) continue;
            bullet.gameObject.SetActive(true);
            bullet.gameObject.transform.position = transform.position;

            float curAngle = Mathf.PI * 2 * i / fireCircleBulletNum + angleOffset;
            bullet.moveVec = new Vector2(Mathf.Cos(curAngle), Mathf.Sin(curAngle)).normalized;
        }
    }
    canAttack = true;
}
~~~
탄환을 보스 중심으로 하여 360도 전 방향 원형 모양으로 화려하게 뿌리는 전체 탄막 사격 패턴 코루틴입니다.<br/>
패턴 제한 횟수 `fireAroundCount` 동안 루프를 도는데, 플레이어가 특정한 위치에서 이동하지 않아도 공격이 회피되는 것을 방지하기 위해 매 발사 턴마다 오프셋 각도를 `15도`씩 라디안 단위 `Mathf.Deg2Rad`로 비틀어 가며 사격 궤적을 꼬아버립니다.<br/>
원형 연산 공식인 **360도(Mathf.PI * 2)를 총알 개수로 나눈 값**에 현재 순번 `i`와 오프셋 각도를 결합하여 매 탄환의 독립 각도 `curAngle`을 계산합니다. 이를 삼각함수 `Mathf.Cos()`와 `Mathf.Sin()`을 사용해 방사형 2D 방향 벡터를 계산하여 오브젝트 풀 에서 총알을 받아와서 발사합니다.<br/> 전체 발사 사이클이 완수되면 다시 새로운 무작위 공격을 할 수 있도록 플래그 `canAttack`를 원상 복구합니다.

<br/>

~~~csharp
IEnumerator FireShot()
{
    for (int cnt = 0; cnt < fireSectorCount; cnt++)
    {
        yield return new WaitForSeconds(fireDelay);

        if (playerTransform == null) continue;

        //보스에서 플레이어를 바라보는 기본 방향 벡터와 중심 각도
        Vector2 dir = playerTransform.position - transform.position;
        float centerAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //부채꼴의 시작 각도
        float startAngle = centerAngle - (angle / 2f);

        //30발의 총알이 부채꼴 공간에 고르게 퍼지기 위한 간격 각도를 계산합니다.
        float angleStep = angle / (fireShotBulletNum - 1);

        //지정한 총알 개수 한 발씩 각도를 계산해 발사
        for (int i = 0; i < fireShotBulletNum; i++)
        {
            // 이번 차례에 쏠 총알의 최종 각도
            float targetAngle = startAngle + (angleStep * i);

            //라디안으로 변환하여 2D 방향 벡터로 변경
            float rad = targetAngle * Mathf.Deg2Rad;
            Vector2 fireDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

            //총알을 폴에서 가져오기
            EnemyBullet bullet = GameManager.Instance.GetEnemyBullet();
            if (bullet == null) continue;

            //가져온 총알에 계산해 둔 방향 벡터 설정
            bullet.gameObject.SetActive(true);
            bullet.gameObject.transform.position = transform.position;
            bullet.moveVec = fireDirection;

            // 총알 오브젝트의 이미지 각도도 날아가는 방향을 바라보게 회전시켜 줍니다.
            bullet.transform.rotation = Quaternion.Euler(0, 0, targetAngle - 90f);
        }
    }
    canAttack = true;
}
~~~
플레이어의 현재 위치를 타겟팅하여 그 주변 영역에 부채꼴(산탄) 형태로 사격하는 탄막 발사 패턴 코루틴입니다.<br/>
보스 위치에서 플레이어 위치를 기반으로 기본 조준 벡터 `dir`를 구한 후, 역삼각함수인 **Mathf.Atan2**를 이용해 플레이어가 있는 방향의 중심 각도 `centerAngle`를 계산합니다. 해당 중심 각도를 기준으로 설정 부채꼴 전개각 `angle`의 절반만큼 왼쪽으로 꺾어 탄막 발사 시작각 `startAngle`을 계산합니다.<br/>
설정된 총알 개수가 지정된 각도 공간 안에 일정한 조밀도로 균등하게 쪼개져 나갈 수 있게 간격 계산 공식 `angle / (fireShotBulletNum - 1)`을 도입해 발사할 총알 사이 각도 `angleStep`를 산출합니다. 이후 내부 루프에서 각도 누적 연산 및 라디안 변환 처리를 거쳐 사선 방향 유도 벡터 `fireDirection`을 구한 뒤 탄환을 발사합니다. <br/>
적 총알의 프리팹 이미지 스프라이트 각도 회전 처리를 위해 `Quaternion.Euler()`를 사용하여 비행 방향과 스프라이트 방향이 일치하도록 보정 연산(`targetAngle - 90f`)해 총알이 발사될 때 발사되는 방향을 바라보게 만들었습니다.

<br/>

~~~csharp
IEnumerator FireForward()
{
    for (int cnt = 0; cnt < fireForwardCount; cnt++) 
    {
        yield return new WaitForSeconds(fireDelay);

        for (int i = 0; i < bulletNum; i++)
        {
            yield return new WaitForSeconds(fireGap);
            
            EnemyBullet bullet = GameManager.Instance.GetEnemyBullet();
            if (bullet == null) continue;

            bullet.gameObject.SetActive(true);
            bullet.gameObject.transform.position = transform.position;
            bullet.moveVec = (playerTransform.transform.position - transform.position).normalized;
        }
    }
    canAttack = true;
}
~~~
전형적인 타겟 추적형 일직선 연사 패턴 코루틴입니다.<br/>
지정된 탄수만큼 내부 루프를 돌면서 한 지점에 탄환이 겹치지 않고 연속해서 날아가도록 발사 간격 `fireGap`을 통해 연사 속도를 조절했습니다.<br/>
각 탄환을 발사하는 시점의 플레이어 좌표를 매번 추적하여, 플레이어가 한 자리에 가만히 있지 못하고 계속 움직이도록 패턴을 설계했습니다.

<br/>

~~~csharp
IEnumerator FireRapid()
{
    while (true)
    {
        // 원하는 공격 딜레이만큼 대기
        yield return new WaitForSeconds(fireDelay);

        if (playerTransform == null) continue;

        for (int i = 0; i < fireRapidBulletNum; i++)
        {
            // 연사 느낌을 주기 위해 총알 한 발당 미세한 시간 차이
            yield return new WaitForSeconds(fireGap);
            if (playerTransform == null) break;

            //보스에서 플레이어를 바라보는 기본 방향과 중심 각도 계산
            Vector2 dir = playerTransform.position - transform.position;
            float centerAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            //설정 각도에 따라 총알 퍼짐 무작위 설정
            float randomOffset = Random.Range(-angle / 2f, angle / 2f);
            float finalAngle = centerAngle + randomOffset;

            //최종 결정된 랜덤 각도를 방향 벡터로 변환
            float rad = finalAngle * Mathf.Deg2Rad;
            Vector2 fireDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

            //총알을 생성하고 방향을 주입
            EnemyBullet bullet = GameManager.Instance.GetEnemyBullet();
            if (bullet == null) continue;

            bullet.gameObject.SetActive(true);
            bullet.gameObject.transform.position = transform.position;
            bullet.moveVec = fireDirection;
        }
    }
}
~~~
광폭화(Rage) 상태에 진입했을 때 기존 일반 무작위 패턴들을 중단시키고, 보스 오브젝트가 파괴될 때까지 영구히 화면 전체에 소나기처럼 무차별 탄환을 난사하는 특수 전용 패턴 코루틴입니다.<br/>
`while(true)` 루프에서 동작하며 플레이어 조준 중심 각도 `centerAngle`를 연산하고 **Random.Range(-angle/2f, angle/2f)**를 통해 부채꼴 사거리 영역 내의 임의의 각도를 무작위로 계속 계산하여 사격합니다. 난수를 각도 적용 및 벡터 변환을 `fireGap` 단위로 연속 수행하여 탄막을 형성합니다.

<br/>

~~~csharp
private void Move()
{
    if (playerTransform == null)
    {
        Debug.Log("Input player object");
        rigid.linearVelocity = Vector2.zero;
        return;
    }

    // 플레이어와 보스의 실제 위치 차이 
    // 보스에서 플레이어로 향하는 방향 벡터
    float directionX = playerTransform.position.x - transform.position.x;
    float directionY = playerTransform.position.y - transform.position.y;

    float nextVelocityX = 0f;
    float nextVelocityY = 0f;

    // 플레이어와의 가로 거리가 가깝지 않다면 
    // moveSpeed만큼 이동
    if (Mathf.Abs(directionX) > minDistX)
    {
        nextVelocityX = Mathf.Sign(directionX) * moveSpeed;
    }

    // 보스가 화면 밖에서 등장해 6f 만큼 이동
    // 현재 내 y 좌표가 6f보다 크다면 아래로 이동

    if (transform.position.y > minDistY)
    {
        nextVelocityY = -moveSpeed; // 아래로 하강
    }
    else
    {
        nextVelocityY = 0f; // 원하는 높이에 도달하면 Y축 이동 중지
    }

    // 최종적으로 계산된 속도를 바탕으로 실질적인 이동
    rigid.linearVelocity = new Vector2(nextVelocityX, nextVelocityY);
}
~~~
보스가 최초 화면 상단에서 진입하고, 플레이어의 가로축 움직임을 따라가며 이동하는 함수입니다.<br/>
보스와 플레이어 간의 실시간 축별 거리 차이 `directionX`, `directionY`를 산출합니다.<br/>
**수직 이동 :** 보스가 맵 상단에서 생성되어 내려올 때, 보스의 현재 Y 좌표가 설정 수치 `minDistY`보다 클 때만 속도를 아래 방향(`-moveSpeed`)으로 주어 스테이지 영역 안에서 수직 하강하게 유도합니다. 기준 고도인 `minDistY` 이하로 안착하는 즉시 Y 속도를 `0`으로 처리해 더 이상 아래로 밀려 내려오지 않게 Y축 거리를 유지합니다.<br/>
**수평 이동 :** 플레이어와의 X축 거리의 절대값 `Mathf.Abs()`이 오차 허용치 `minDistX`보다 크다면 부호 추출 함수인 `Mathf.Sign()`을 이용해 플레이어가 보스 기준 우측에 있으면 `+1`, 좌측에 있으면 `-1`을 반환받아 플레이어가 위치한 가로 방향으로 보스를 횡이동시킵니다. 최종 연산된 축별 연산 물리 벡터를 유니티 6의 `rigid.linearVelocity`를 사용해 플레이어를 수평 방향으로 추적합니다.

<br/>

~~~csharp
IEnumerator HitEffect()
{
    render.enabled = false;
    yield return new WaitForSeconds(0.05f);
    render.enabled = true;
}
~~~
보스가 플레이어의 공격에 피격당했을 때 발생하는 피격 이펙트 코루틴입니다.<br/>
대미지 연산 즉시 보스의 이미지 스프라이트 렌더러 컴포넌트를 비활성화하고 `0.05초` 동안 대기한 뒤 다시 활성화시켜, 거대 보스가 대미지를 입을 때 번쩍이는 시각적인 효과를 연출하기 위한 코루틴입니다.

<br/>

~~~csharp
private void Death()
{
    scoreSO.AddScore(score);

    if (gameClearEvent != null)
    {
        gameClearEvent.Raise();
    }

    StopAllCoroutines();
    gameObject.SetActive(false);
}
~~~
보스가 파괴되었을 때 스테이지를 종료하는 함수입니다.<br/>
보스 격추 시 점수 `score`를 스크립터블 오브젝트인 `scoreSO`에서 처리하여 UI와 연동합니다. 이후 연결된 스크립터블 오브젝트 **gameClearEvent.Raise()**를 호출하여 시스템 전체에 보스 파괴 및 게임 클리어 UI를 호출합니다. 이후 보스 오브젝트의 모든 코루틴을 종료시키고 비활성화 시킵니다.

<br/>

~~~csharp
private void Rage()
{
    StopAllCoroutines();
    canAttack = true;
    isRage = true;
    fireDelay /= 2;
    fireGap /= 2;
    StartCoroutine(FireRapid());
}
~~~
보스전의 난이도를 체력 상황에 따라 조절하기 위한 함수입니다.<br/>
페이즈 변환이 시작되는 즉시 기존에 실행 중이던 모든 1페이즈용 기본 무작위 공격 코루틴 패턴을 `StopAllCoroutines()`을 호출하여 중단 시킵니다. 공격 가능 여부를 갱신하기 위해 `canAttack`을 true로 바꾸고 광폭화 상태 플래그 `isRage`를 true로 변경합니다. 보스의 패턴간 변경 빈도 `fireDelay`과 연사 빈도 `fireGap` 수치를 기존의 절반으로 바꿔 공격 속도를 빈도를 높이고, 2페이즈 전용 무제한 폭풍 난사 코루틴 패턴인 `FireRapid()`를 호출합니다.

<br/>

~~~csharp
private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.gameObject.layer != 8) return;

    if (collision.gameObject.CompareTag("Ult"))
        Health -= 500;
    else
        Health -= collision.gameObject.GetComponent<PlayerBullet>().damage;
    

    if (Health <= 0f)
    {
        Death();
        return;
    }

    if (!isRage && Health / maxHealth <= 0.5f)
    {
        Debug.Log("in rage mode");
        Rage();
    }
    StartCoroutine(HitEffect());
}
~~~
BoxCollider2D 컴포넌트의 트리거 충돌 시스템을 활용하여 보스의 피격을 확인하고 상황에 맞는 동작을 위한 이벤트 핸들러입니다.<br/>
**피격 오브젝트 제한 :** 오직 아군 투사체 레이어 **PlayerBullet**과의 충돌만 유효하게 허용합니다. 만약 충돌한 아군 오브젝트의 태그가 플레이어의 궁극기 `Ult`인 경우, 고정 수치인 `500` 포인트의 데미지를 체력에서 차감합니다. 일반 탄환일 경우 해당 탄환 컴포넌트 `PlayerBullet`을 참조해 데미지 수치만큼 체력을 감소시킵니다.<br/>
**파괴 및 Rage 진입 :** 연산 후 보스의 현재 체력이 0 이하로 떨어지면 스테이지 승리 처리 함수인 **Death()**를 호출하합니다. 아직 보스가 살아있다면 현재 보스가 아직 1페이즈 상태 `!isRage`인지 검사하고 남은 체력이 50% 미만인지 체크합니다. <br/>
체력이 50% 미만이면 2페이즈 변환 함수인 `Rage()`를 호출하고, 조건을 모두 통과한 일반 피격 시에는 `HitEffect()` 코루틴을 호출합니다.

## EnemyBullet.cs 상세 설명

`EnemyBullet.cs`는 적 캐릭터와 보스 몬스터가 발사하는 탄환 오브젝트의 물리적 이동과 충돌 판정을 제어담당하는 클래스입니다. 일반 적 오브젝트와 보스 오브젝트 둘 모두에서 사용할 수 있도록 설계했으며, 메모리 최적화를 위해 **GameManager**의 오브젝트 풀 시스템과 연동되어 동작합니다.

~~~csharp
public class EnemyBullet : MonoBehaviour
{
    public Vector2 moveVec = Vector2.down;

    private Rigidbody2D rigid;
    public float moveSpeed = 2f;
    private Vector2 bulletVec;
    public float damage = 1f;
}
~~~
EnemyBullet.cs 클래스의 상단에서 초기화한 변수들로 탄환의 기본 비행 방향 벡터(moveVec), 물리 처리를 위한 컴포넌트 참조 변수, 이동 속도 상수 `moveSpeed`를 선언합니다. 또한 플레이어 피격 시 연산될 데미지 `damage` 변수를 설정합니다.

<br/>

~~~csharp
private void Awake()
{
    rigid = GetComponent<Rigidbody2D>();
}
~~~
런타임 최적화를 위해 주요 컴포넌트(Rigidbody2D)를 런타임 전에 미리 변수에 할당하여 초기화합니다.

<br/>

~~~csharp
public void OnEnable() //활성화 시에 방향 결정 => 오브젝트 폴 및 보스 오브젝트에서도 동일한 총알 사용을 위해 수정, 적 오브젝트에서 총알 발사시 총알 위치 지정하도록 변경
{
    /*
    if (playerTransform != null)
        bulletVec = (playerTransform.position - transform.position).normalized;
    */
}
~~~
오브젝트 풀 시스템에 의해 탄환이 활성화 될 때마다 매번 호출되는 함수입니다.<br/>
기존에는 탄환 스스로 생성 시점에 플레이어 방향을 추적하도록 설계되어 있었으나, 일반 적의 사격뿐만 아니라 보스의 원형/부채꼴 방사형 탄막 등 다양한 패턴에 탄환을 공동으로 재사용하기 위해 자체 추적 주석 처리 및 외부에서 방향 벡터를 직접 지정하여 동작하도록 구조를 변경했습니다.<br/>
 주석 처리된 부분은 해당 코드의 작성자와 `GameManager`를 통한 오브젝트 폴을 담당한 인원이 달라 코드 변경시 어떤 문제가 발생할지 모르기 때문에 주석 처리했습니다.

<br/>

~~~csharp
private void OnDisable()
{
    rigid.linearVelocity = Vector2.zero; //속도 초기화
}
~~~
탄환이 플레이어에게 적중하거나 화면 밖으로 벗어나 비활성화될 때 호출됩니다.<br/>
탄환이 비활성화되어 오브젝트 풀로 돌아가 대기하는 동안에도 물리 속도가 그대로 남아있어, 나중에 재사용될 때 엉뚱한 속도로 튀어나가는 오작동을 방지하기 위해 `rigid.linearVelocity`를 `Vector2.zero`로 깨끗하게 초기화합니다.

<br/>

~~~csharp
private void FixedUpdate()
{
    rigid.linearVelocity = moveVec * moveSpeed;
}
~~~
프레임률이 변하더라도 일정한 탄환 비행 속도를 보장하기 위해, 물리에 기반한 이동 로직을 일반 Update가 아닌 유니티의 독립 물리 루프인 FixedUpdate에서 실행합니다.<br/>
유니티 6 사양에 맞추어 기존 velocity 대신 rigid.linearVelocity 속성에 외부에서 주입받은 탄환 고유의 방향 벡터 `moveVec`와 속도 변수 `moveSpeed`를 연산하여 대입함으로써 탄환을 실시간 이동시킵니다.

<br/>

~~~csharp
private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.gameObject.tag == "Enemy") return; //적끼리는 무시

    if (collision.gameObject.layer == 6) //Player와 만나면 제거
    {
        GameManager.Instance.ReturnEnemyBullet(this);
    }

    if (collision.gameObject.layer == 0) //Border와 만나면 제거
        GameManager.Instance.ReturnEnemyBullet(this);

    if (collision.gameObject.CompareTag("Ult")) // Ult와 충돌 시 제거
        GameManager.Instance.ReturnEnemyBullet(this);
}
~~~
CircleCollider2D 트리거 충돌 시스템을 활용하여 적 탄환이 플레이어, 화면 경계선, 플레이어의 궁극기 등과 충돌했을 때의 예외 처리 및 수거 로직을 수행합니다.<br/>
**충돌 예외 처리:** 적 캐릭터가 쏜 탄환이 다른 적(Tag: "Enemy")에게 부딪혀 지워지는 불상사를 막기 위해 즉시 함수를 종료합니다.<br/>
**플레이어 적중 및 스테이지 이탈 :** 탄환이 플레이어에게 적중했거나, 스테이지를 벗어났을 경우, 혹은 플레이어가 발사한 궁극기 범위에 닿아 소멸 판정을 받았을 때는 메모리 누수를 막고 재사용하기 위해 `Destroy()`를 호출하는 대신 **GameManager.Instance.ReturnEnemyBullet(this)**를 호출하여 오브젝트 풀에 탄환을 즉각 안전하게 반환합니다.