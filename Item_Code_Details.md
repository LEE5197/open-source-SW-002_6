# Item.cs 관련 코드 상세 설명
코드 상세 설명<br/>
[Player](./Player_Code_Details.md)<br/>
[Item](./Item_Code_Details.md)<br/>
[Enemy](./Enemy_Code_details.md)<br/>
[GameManager](./GameManager_Code_details.md)<br/>
[UI](./UI_Code_details.md)<br/>
[README](./README.md)<br/>
## Item.cs 상세 설명

`Item.cs`는 게임 내에서 적을 처치하거나 특정 조건에서 드롭되는 아이템 오브젝트의 무작위 이동, 벽 충돌 시 이동 방향 변경, 획득 사운드 출력 및 자동 소멸 로직을 제어하는 클래스입니다.

~~~csharp
public class Item : MonoBehaviour
{
    private Rigidbody2D rigid;
    public Vector2 moveVec = Vector2.zero;
    public float moveSpeed = 10f;
    public float deleteTimer = 5f;

    [Header("Audio Clips")]
    public AudioClip ItemClip;
}
~~~
Item.cs 클래스의 상단에서 초기화한 변수들로 아이템의 물리 처리를 위한 컴포넌트 참조 변수, 이동 속도 상수를 선언합니다. 또한 아이템이 실시간으로 나아갈 방향 벡터 `moveVec`와 플레이어가 획득하지 못했을 때 소멸시킬 자동 비활성화 시간 변수 `deleteTimer`, 획득 시 재생할 오디오 에셋 `ItemClip`을 설정합니다.

<br/>

~~~csharp
private void Awake()
{
    // 이동에 필요한 컴포넌트 할당
    rigid = GetComponent<Rigidbody2D>();
}
~~~
런타임 최적화를 위해 주요 컴포넌트(Rigidbody2D)를 런타임 전에 미리 변수에 할당하여 초기화합니다.

<br/>

~~~csharp
private void OnEnable()
{
    float x = Random.Range(-1f, 1f);
    float y = Random.Range(-1f, 1f);

    moveVec = new Vector2(x, y).normalized;

    // deleteTimer 이후 오브젝트 삭제를 위한 함수 호출
    StartCoroutine(deleteObject());
}
~~~
오브젝트 풀에서 아이템이 활성화 될 때마다 매번 실행되는 함수입니다.<br/>
아이템이 생성되었을 때 사방으로 퍼지는 효과를 주기 위해 `Random.Range`를 활용하여 X축과 Y축 각각 `-1f`에서 `1f` 사이의 무작위 방향 벡터를 생성합니다. 이때 비스듬한 방향으로 갈 때 속도가 빨라지는 현상을 막기 위해 Vector 클래스의 `.normalized`를 통해 정규화한 뒤, 자동 소멸 코루틴인 **deleteObject()**를 작동시킵니다.

<br/>

~~~csharp
private void OnDisable()
{
    StopAllCoroutines();
}
~~~
플레이어가 아이템을 획득하거나 제한 시간이 지나 오브젝트가 비활성화 될 때 호출되는 함수입니다.<br/>
오브젝트가 꺼진 상태에서 백그라운드로 코루틴이 계속 돌아가며 메모리를 낭비하거나 에러를 일으키지 않도록 **StopAllCoroutines()**를 통해 실행 중인 모든 코루틴을 중단합니다.

<br/>

~~~csharp
private void FixedUpdate()
{
    // 아이템 오브젝트 이동
    rigid.linearVelocity = moveVec * moveSpeed;
}
~~~
프레임률이 변하더라도 일정한 아이템 비행 속도를 보장하기 위해, 물리에 기반한 이동 로직을 일반 Update가 아닌 유니티의 독립 물리 루프인 FixedUpdate에서 실행합니다.<br/>
유니티 6 사양에 맞추어 기존 velocity 대신 rigid.linearVelocity 속성에 정규화된 무작위 방향 벡터 `moveVec`와 속도 변수 `moveSpeed`를 연산하여 대입함으로써 아이템을 이동시킵니다.

<br/>

~~~csharp
private void OnTriggerEnter2D(Collider2D collision)
{
    // 플레이어와 충돌하면 아이템 오브젝트 비활성화
    if (collision.gameObject.layer == 6)
    {
        //Debug.Log("hit player");
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySfx(ItemClip);
        }
        gameObject.SetActive(false);
        return;
    }
    
    // 경계선과 충돌하면 충돌 반대 방향으로 방향 전환
    switch (collision.gameObject.name)
    {
        case "Border_UP":
            moveVec.y = -moveVec.y;
            break;
        case "Border_DOWN":
            moveVec.y = -moveVec.y;
            break;
        case "Border_LEFT":
            moveVec.x = -moveVec.x;
            break;
        case "Border_RIGHT":
            moveVec.x = -moveVec.x;
            break;
    }
}
~~~
BoxCollider2D 컴포넌트의 트리거 충돌 시스템을 활용하여 플레이어의 아이템 획득 및 화면 외곽 벽에 부딪혔을 때의 이동 방향을 변경하는 로직을 수행합니다.<br/>
**플레이어 충돌:** 플레이어 오브젝트와 충돌하면 `SoundManager`를 통해 아이템 획득 효과음을 출력하고, 오브젝트를 즉시 비활성화하여 오브젝트 풀로 수거합니다.<br/>
**화면 경계선 반사:** 아이템이 화면 밖으로 나가지 않도록 4방향 경계선 오브젝트의 이름을 `switch-case` 문으로 판별합니다. 상/하 벽에 부딪히면 Y축 방향 벡터를 반전시키고, 좌/우 벽에 부딪히면 X축 방향 벡터를 반전시켜 스테이지 경계 밖으로 아이템이 나가지 않도록 만들었습니다.

<br/>

~~~csharp
IEnumerator deleteObject()
{
    yield return new WaitForSeconds(deleteTimer);
    gameObject.SetActive(false);
}
~~~
아이템이 플레이어에게 먹히지 않고 방치되었을 때 작동하는 자동 소멸용 코루틴입니다.<br/>
지정된 제한 시간 동안 플레이어가 아이템을 획득하지 못하면 화면에서 자동으로 비활성화시켜 오브젝트 폴에 반환합니다.

## SubWeapon.cs 상세 설명

`SubWeapon.cs`는 플레이어 주변을 따라다니며 자동으로 주변의 적을 탐지하고 추적하여 공격하는 보조 무기의 움직임과 공격을 제어하는 클래스입니다.

~~~csharp
public class SubWeapon : MonoBehaviour
{
    [SerializeField] private Transform targetPos;
    private Rigidbody2D rigid;
    private Vector2 lookVec = Vector2.up;

    public Vector2 offset;
    public float maxSpeed = 8f;
    public float minDist = 1f;
    public float slowDist = 3f;

    public float attackDelay = 1f;
    public LayerMask enemyLayer;
    public float attackRange = 7f;

    private bool canAttack = true;

    [Header("Audio Clips")]
    public AudioClip PlayerShootClip;
}
~~~
SubWeapon.cs 클래스의 상단에서 초기화한 변수들로 플레이어를 추적하기 위한 위치 정보 `targetPos`, 물리 처리를 위한 컴포넌트 변수, 타겟을 바라보는 방향 벡터 `lookVec`를 선언합니다. 또한 플레이어와의 상대적 고정 위치를 잡기 위한 오프셋 `offset`, 부드러운 이동을 위한 속도 및 거리 상수를 정의합니다. 추가로 자동 타겟팅을 위한 레이어 마스크 `enemyLayer`, 공격 사거리 `attackRange`, 공격 가능 여부 판단할 플래그 `canAttack`와 효과음 에셋을 설정합니다.

<br/>

~~~csharp
private void Awake()
{
    // 이동에 필요한 Rigidbody2D 컴포넌트 할당
    rigid = GetComponent<Rigidbody2D>();
}
~~~
런타임 최적화를 위해 주요 컴포넌트(Rigidbody2D)를 런타임 전에 미리 변수에 할당하여 초기화합니다.

<br/>

~~~csharp
private void Start()
{
    if (targetPos == null)
        targetPos = GameManager.Instance.playerTransform;
    if (targetPos == null)
        targetPos = GameObject.FindWithTag("Player").transform;
}
~~~
스크립트가 시작될 때 플레이어의 위치를 참조하는 로직입니다.<br/>
기본적으로 인스펙터 창에서 직접 할당하지 않았더라도 무기가 정상 작동할 수 있도록 `GameManager` 싱글톤의 플레이어 참조값을 먼저 가져오며, 만약 비어있다면 `"Player"` 태그를 가진 오브젝트를 씬에서 탐색하여 자동으로 `targetPos`을 바인딩합니다.

<br/>

~~~csharp
private void Update()
{
    if (canAttack)
    {
        Attack();
    }
    SetLookVec();
}
~~~
매 프레임마다 보조 무기가 적을 감지하고 공격하는 루프입니다.<br/>
공격 가능 상태 `canAttack`가 충족되면 자동으로 적을 찾아 공격하는 `Attack()` 함수를 실행하고, 공격 여부와 상관없이 매 프레임 가장 가까운 적을 조준하도록 유도하는 조준 함수 `SetLookVec()`을 상시 호출합니다.

<br/>

~~~csharp
private void FixedUpdate()
{
    Move();
}
~~~
프레임률이 변하더라도 일정한 추적 물리 성능을 보장하기 위해, 플레이어를 따라 움직이는 물리 기반 이동 로직인 Move() 함수를 유니티의 독립 물리 루프인 FixedUpdate에서 실행합니다.

<br/>

~~~csharp
private void SetLookVec()
{
    // 가장 가까운 적 위치를 담을 변수
    Collider2D []enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
    Collider2D nearEnemy = null;

    // 가장 가까운 적을 알아내기 위한 변수
    float lowDist = Mathf.Infinity;

    foreach(Collider2D it in enemies)
    {	// 연산 부담을 줄이기 위해 sqrt 대신 sqrMagnitude 사용
        float dist = (transform.position - it.transform.position).sqrMagnitude;

        // 최소 거리가 현재 가리키는 적과의 거리보다 길다면 
        // 최소 거리와 현재 가리키는 적 위치 업데이트
        if (lowDist > dist)
        {
            nearEnemy = it;
            lowDist = dist;
        }
    }

    if (nearEnemy != null)
        lookVec = (nearEnemy.gameObject.transform.position - transform.position).normalized;
    else
        lookVec = Vector2.up;

    // 목표 방향 설정할 회전값 계산
    Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, lookVec);

    // 일정한 속도로 회전
    transform.rotation = Quaternion.RotateTowards(
        transform.rotation,
        targetRotation,
        720f * Time.deltaTime
    );
}
~~~
공격 사거리 내에서 가장 가까운 적을 찾아내고, 해당 방향으로 자연스럽게 회전 조준하는 정밀 타겟팅 함수입니다.<br/>
`Physics2D.OverlapCircleAll`을 사용하여 주변의 모든 적 레이어 오브젝트들을 탐지합니다. 연산 최적화를 위해 제곱근을 구하는 비용이 큰 `Vector2.Distance` 대신 제곱 비례 거리 연산인 **.sqrMagnitude**를 활용하여 거리 수치 비교를 하여 가장 가까운 거리의 적 오브젝트 위치를 토대로 바라보는 방향 `lookVec`을 계산합니다.<br/>