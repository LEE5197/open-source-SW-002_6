# Player 관련 코드 상세 설명
코드 상세 설명<br/>
[Player](./Player_Code_Details.md)<br/>
[Item](./Item_Code_Details.md)<br/>
[Enemy](./Enemy_Code_details.md)<br/>
[GameManager](./GameManager_Code_details.md)<br/>
[UI](./UI_Code_details.md)<br/>
[README](./README.md)<br/>

플레이어 오브젝트와 직접적인 연관이 있는 C# 파일의 코드 상세 설명

## Player.cs 상세 설명

`Player.cs`는 플레이어 캐릭터를 제어하는 클래스입니다. 유니티 6의 **Input System** 이벤트 인터페이스를 기반으로 키보드 입력을 처리하며, 이동, 총알 및 궁극기 발사, 스크립터블 오브젝트를 기반으로(Scriptable Object) 상태 데이터와 UI(체력/점수/궁극기) 연동, 아이템 상호작용 및 피격 연출을 담당합니다.

~~~csharp
public class Player : MonoBehaviour
{
    // 플레이어 오브젝트를 움직이기 위한 변수
    private Rigidbody2D rigid;
    private Vector2 moveVec = Vector2.zero;
    public float moveSpeed = 8f; // 플레이어 오브젝트의 이동 속도

    // 플레이어 오브젝트에서 총알을 발사하기 위한 변수
    private bool canFire = true;
    private bool isFire = false;
    private float damage = 1f;
    public float fireDelay = 0.2f; // 총알 발사 딜레이 지정할 변수

    // 스크립터블 오브젝트를 통한 UI 및 데이터 연동
    public HealthSO healthSO;
    public ScoreSO scoreSO;

    // 궁극기 오브젝트 및 갯수 관리
    public GameObject ultPrefabs;
    public UltCountSO ultCount;
    private GameObject ultObject;

    // 현재 활성화된 보조무기 오브젝트를 관리할 변수
    public List<GameObject> subWeaponList;
    private int subIdx = 0;

    [Header("Audio Clips")]
    public AudioClip PlayerShootClip;
    public AudioClip PlayerUltClip;
    public AudioClip PlayerHitClip;

    private SpriteRenderer spriteRenderer;
    private Coroutine hitRoutine;

    ...

}
~~~
Player.cs 클래스의 상단에서 초기화한 변수들로 컴포넌트 참조 변수, 전투 밸런스 상수, 사운드 에셋을 선언합니다. 특히 HealthSO, ScoreSO, UltCountSO 같은 스크립터블 오브젝트(Scriptable Object)를 사용하여 플레이어 상태 데이터를 UI와 유연하게 연동할 수 있도록 설계했습니다.

<br/>

~~~csharp
private void Awake()
{
    rigid = GetComponent<Rigidbody2D>();
    spriteRenderer = GetComponent<SpriteRenderer>();

    ultObject = Instantiate(ultPrefabs);
    ultObject.SetActive(false);
}
~~~

런타임 최적화를 위해 주요 컴포넌트(Rigidbody2D, SpriteRenderer)를 런타임 전에 미리 변수에 할당합니다. 
<br/>
매번 새로 생성하기에는 컴퓨터 자원 소모가 큰 궁극기 오브젝트를 게임 시작 시 미리 생성(Instantiate)하고 비활성화해 두는 일종의 오브젝트 풀링(Object Pooling) 기법을 적용하여 최적화를 진행했습니다.

<br/>

~~~csharp
private void Update()
{
    if (isFire && canFire)
    {
        Fire();
    }
}
~~~
매 프레임마다 플레이어의 공격키 입력 상태 `isFire`와 발사 가능 쿨타임 여부 `canFire`를 실시간으로 체크하여 조건을 만족할 때만 탄환 발사 함수`Fire()`를 호출하여 총알 발사 빈도를 조절했습니다.

<br/>

~~~csharp
private void FixedUpdate()
{
    Move();
}
~~~
프레임률이 변하더라도 일정한 물리 연산 속도를 보장하기 위해, 플레이어의 물리 기반 이동 함수인 Rigidbody2D 컴포넌트를 이용해 이동하는 `Move()`를 일반 Update가 아닌 유니티의 독립 물리 루프인 FixedUpdate에서 실행합니다.

<br/>

~~~csharp
private void Move()
{
    rigid.linearVelocity = moveVec * moveSpeed;
}
~~~
Input System에서 이동 입력키를 통해 얻은 방향 벡터 `moveVec`에 속도 변수 `moveSpeed`를 연산하여 플레이어를 이동시킵니다. 유니티 6 사양에 맞추어 기존 velocity 대신 linearVelocity 속성을 직접 제어하도록 구현했습니다.

<br/>

~~~csharp
private void Fire()
{
    PlayerBullet bullet = GameManager.Instance.GetPlayerBullet();
    if (bullet == null) return;

    if (SoundManager.Instance != null) // 효과음 재생
    {
        SoundManager.Instance.PlaySfx(PlayerShootClip);
    }

    bullet.damage = damage;
    bullet.gameObject.SetActive(true);
    bullet.gameObject.transform.position = transform.position;
    bullet.moveVec = Vector2.up;
    bullet.transform.up = Vector2.up;
    Debug.Log($"current damage : {damage}");

    StartCoroutine(FireCoroutine());
}
~~~
`GameManager.Instance`에서 `GetPlayerBullet()`을 호출하여 오브젝트 풀에서 총알을 가져옵니다. 총알이 정상적으로 로드되면 사운드를 재생하고, 현재 플레이어의 공격력(damage)을 총알 데미지에 적용한 뒤, 플레이어 위치에서 위쪽(Vector2.up)을 향해 발사되도록 세팅하고 `FireCoroutine()`을 호출하여, 앞서 공격 빈도를 조절하기 위해 사용한 Update()에서 사용된 `canFire` 변수를 false로 바꾸고, 일정 시간이 지난 후 true로 바꿔 공격 빈도를 조절합니다.

<br/>

~~~csharp
void IncreaseBulletDamage(int weight)
{
    if (damage > 100) return;
    damage += weight;
}
~~~
총알 데미지 증가 아이템을 먹었을 때 공격력을 증가시키는 함수입니다. 보스 오브젝트가 너무 빨리 처지되는 것을 막기 위해 최대 공격력이 100 이상이면 공격력을 증가시키지 않도록 제한 했습니다.

<br/>

~~~csharp
private void OnTriggerEnter2D(Collider2D collision)
{
    // [1] 적 탄환(Layer 9)과의 충돌 피격 처리
    if (collision.gameObject.layer == 9)    
    {
        float bullet = 0f;
        if (collision.gameObject.GetComponent<EnemyBullet>() != null)
        {
            bullet = collision.gameObject.GetComponent<EnemyBullet>().damage;
        }
        else
        {
            bullet = collision.gameObject.GetComponent<EnemyBulletTypeB>().damage;
        }
        healthSO.Damage((int)bullet);

        if (hitRoutine == null)
            hitRoutine = StartCoroutine(HitEffect());
        
        return;
    }

    // [2] 아이템(Tag: Item) 획득 및 레이어별 버프 처리
    if (collision.gameObject.CompareTag("Item"))
    {
        switch (collision.gameObject.layer)
        {
            case 11:    // 총알 데미지 증폭 아이템
                IncreaseBulletDamage(10);
                break;

            case 12:    // 점수 추가 아이템
                scoreSO.AddScore(100);
                break;

            case 13:    // 체력 회복 아이템
                healthSO.Heal(10);
                break;

            case 14:    // 궁극기 추가 아이템
                ultCount.GetUlt();
                break;

            case 15:    // 보조무기 추가 아이템 (최대 4개까지 순차 활성화)
                if (subIdx < 4)
                {
                    subWeaponList[subIdx].SetActive(true);
                    subIdx++;
                }
                break;
            }
            return;
    }
}
~~~

BoxCollider2D 컴포넌트의 트리거 충돌 시스템을 활용하여 적의 공격 판정 및 아이템 처리를 일괄 수행합니다.<br/>
적 탄환과 충돌할 경우, 적 탄환 종류에 따라 다른 종류의 컴포넌트를 받아와서 데미지를 초기화하고, 스크립터블 오브젝트를 통해 체력을 소모합니다.<br/>
아이템과 충돌 시 아이템 오브젝트의 세부 레이어 번호(11~15)를 switch-case 문으로 판별하여 공격력 버프, 스코어 가산, 힐, 궁극기 충전, 그리고 최대 4개까지 장착 가능한 보조 무기(subWeaponList)를 활성화합니다.

<br/>

~~~csharp
void OnMove(InputValue value)
{
    moveVec = value.Get<Vector2>();
}

void OnFire(InputValue value)
{
    isFire = value.isPressed;
}

void OnUlt(InputValue value)
{
    if (ultObject.activeSelf) return; // 이미 궁극기가 켜져 있다면 예외 처리
    if(ultCount.UseUlt())
        StartCoroutine(ActiveUlt());
}
~~~

유니티 6의 기본 입력 시스템을 사용하기 위한 함수로, 미리 지정해둔 Input System 에셋을 이용하여 키 입력을 받습니다. 이 프로젝트의 경우 OnMove의 할당된 키는 `W, A, S, D`, OnFire의 경우 `K`, OnUlt의 경우 `L`키를 지정했습니다.

<br/>

~~~csharp
    IEnumerator FireCoroutine()
    {
        canFire = false;
        yield return new WaitForSeconds(fireDelay);
        canFire = true;
    }

    IEnumerator ActiveUlt()
	{
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySfx(PlayerUltClip);
        }

        ultObject.SetActive(true);
        ultObject.transform.position = transform.position;
        yield return new WaitForSeconds(3f);
        ultObject.SetActive(false);
	}

    IEnumerator HitEffect()
	{
        SoundManager.Instance.PlaySfx(PlayerHitClip);
        for (int i = 0; i < 2; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);

            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        hitRoutine = null;
    }
~~~
각 함수별로 총알 발사 빈도를 조절하기 위한 코루틴, 궁극기 발사 빈도를 조절하기 위한 코루틴, 적 총알에 피격당했을 시 깜박거리는 이팩트를 출력할 코루틴입니다.<br/>

먼저 총알 연사를 제어하기 위한 코루틴 'FireCoroutine()'은 탄환이 발사되는 `Fire()`에서 호출되며, `canFire`를 false로 차단하고, 지정된 딜레이 시간 `fireDelay` 동안 대기한 후 다시 true로 해제하여 일정한 공격 속도를 유지시킵니다.

`ActiveUlt()`는 궁극기 사용 시 효과음을 재생하고, 궁극기 오브젝트를 활성화하여 플레이어 위치에 서 일반 총알과 동일하게 위쪽(Vector2.up) 방향으로 발사합니다. 정확히 3초 동안 활성화한 후, 다시 안전하게 비활성화(오브젝트 풀 수거)합니다.

`HitEffect()`는 플레이어가 적 총알에 피격 시 피극 효과음을 재생하고, `Awake()`에서 SpriteRenderer 컴포넌트를 미리 할당받은 `SpriteRender`를 이용해 플레이어 오브젝트의 스프라이트를 활성화, 비활성화를 반복하여 총알에 피격되었음을 직관적으로 알려줍니다.
<br/>

## PlayerBullet.cs 상세 설명

`PlayerBullet.cs`는 플레이어가 발사하는 총알(탄환) 오브젝트의 물리적 이동과 충돌 판정을 제어하는 클래스입니다. 생성과 소멸에 따른 과부하를 줄이기 위해 **GameManager** 싱글톤의 오브젝트 풀 시스템과 긴밀하게 연동되어 작동합니다.

<br/>

~~~csharp
public class PlayerBullet : MonoBehaviour
{
    private Rigidbody2D rigid;
    public float moveSpeed = 10f;
    public float damage = 1f;
    public Vector2 moveVec = Vector2.up;
}
~~~
PlayerBullet.cs 클래스의 상단에서 초기화한 변수들로 탄환의 물리 처리를 위한 컴포넌트 참조 변수, 이동 속도 상수를 선언합니다. 또한 플레이어의 아이템 획득 상태 등에 따라 가변적으로 적용될 수 있는 데미지 `damage` 변수와 탄환이 나아갈 기본 방향 벡터 `moveVec`를 설정합니다.

<br/>

~~~csharp
private void Awake()
{
    // 총알 이동을 위해 Rigidbody2D 컴포넌트 rigid 변수에 초기화
    rigid = GetComponent<Rigidbody2D>();
}
~~~
런타임 최적화를 위해 주요 컴포넌트(Rigidbody2D)를 런타임 전에 미리 변수에 할당하여 초기화합니다. 

<br/>

~~~csharp
private void FixedUpdate()
{
    // 총알 발사 이후 움직이도록 하는 코드
    rigid.linearVelocity = moveVec * moveSpeed;
}
~~~
프레임률이 변하더라도 일정한 탄환 이동 속도를 보장하기 위해, 물리에 기반한 이동 로직을 일반 Update가 아닌 유니티의 독립 물리 루프인 FixedUpdate에서 실행합니다.<br/>
유니티 6 사양에 맞추어 기존 velocity 대신 rigid.linearVelocity 속성에 `moveVec`와 `moveSpeed`를 연산하여 대입함으로써 총알을 이동시킵니다.

<br/>

~~~csharp
private void OnTriggerEnter2D(Collider2D collision)
{
    // 플레이어가 발사한 총알은 플레이어를 무시하도록 설정
    // 총알끼리도 부딪히지 않음
    // 아이템과 충돌하지 않음
    if (collision.gameObject.layer == 6 || collision.gameObject.layer == 9
        || collision.gameObject.CompareTag("Item") || collision.gameObject.layer == 8) return;

    // 오브젝트 재사용을 위해 오브젝트 폴에 반환
    GameManager.Instance.ReturnPlayerBullet(this);
}
~~~
BoxCollider2D 컴포넌트의 트리거 충돌 시스템을 활용하여 플레이어 총알이 화면의 적이나 장애물에 부딪혔을 때의 예외 처리 및 수거 로직을 수행합니다.<br/>
예외 처리를 위해 플레이어 본인(Layer 6), 적의 총알(Layer 9), 아이템 태그("Item"), 그리고 아군 탄환(Layer 8)과의 충돌이 감지되었을 때는 `return`을 통해 충돌 로직을 즉시 무시하도록 설정했습니다.<br/>
유효한 타겟(적 캐릭터 등)과 충돌했을 경우에는 메모리 낭비를 방지하고 오브젝트를 안전하게 재사용할 수 있도록 `GameManager.Instance.ReturnPlayerBullet(this)`를 호출하여 오브젝트 풀에 탄환을 반환합니다.

## PlayerUlt.cs 상세 설명

`PlayerUlt.cs`는 플레이어가 사용하는 필살기인 궁극기(Ultimate) 오브젝트의 연출과 물리 이동을 제어하는 클래스입니다. 오브젝트 풀에서 활성화되는 순간을 기점으로 일정 시간이 지난 후 크기가 거대해지며 화면상의 위협을 제거하는 연출 로직이 포함되어 있습니다.

~~~csharp
public class PlayerUlt : MonoBehaviour
{
    private Rigidbody2D rigid;
    public float moveSpeed = 20f;
}
~~~
PlayerUlt.cs 클래스의 상단에서 초기화한 변수들로 궁극기 오브젝트의 물리 처리를 위한 컴포넌트 참조 변수와 이동 속도 상수를 선언합니다.

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
private void OnEnable()
{
    //Debug.Log("active ult");
    moveSpeed = 10f;
    transform.localScale = new Vector3(1f, 1f, 1f);
    StartCoroutine(ScaleRoutine());
}
~~~
오브젝트 풀링 시스템에 의해 궁극기 오브젝트가 활성화(SetActive(true))될 때마다 매번 실행되는 함수입니다.<br/>
궁극기가 재사용될 때를 대비하여 이동 속도를 10f로, 크기(Scale)를 (1, 1, 1)의 기본 수치로 초기화한 뒤, 시간 경과에 따라 크기를 변경하는 코루틴 함수인 `ScaleRoutine()`을 실행합니다.

<br/>

~~~csharp
private void FixedUpdate()
{
    rigid.linearVelocity = Vector2.up * moveSpeed;
}
~~~
프레임률이 변하더라도 일정한 비행 속도를 보장하기 위해, 물리에 기반한 이동 로직을 일반 Update가 아닌 유니티의 독립 물리 루프인 FixedUpdate에서 실행합니다.<br/>
유니티 6 사양에 맞추어 기존 velocity 대신 rigid.linearVelocity 속성을 사용하며, 항상 위쪽 방향(Vector2.up)으로 지정된 속도 `moveSpeed`만큼 투사체를 전진시킵니다.

<br/>

~~~csharp
IEnumerator ScaleRoutine()
{
    yield return new WaitForSeconds(0.5f);
    transform.localScale = new Vector3(10f, 10f, 10f);
    moveSpeed = 5f;
}
~~~
궁극기 오브젝트의 역동적인 연출을 위한 코루틴 함수입니다.<br/>
발사 후 0.5초 동안 대기한 뒤, 오브젝트의 크기를 기존보다 10배 거대한 localScale 값을 10배 확장시켜 화면의 넓은 범위를 커버하도록 만듭니다. 이때 크기가 커진 만큼 화면에 오래 머무르며 적들을 소거할 수 있도록 `moveSpeed`를 `5f`로 대폭 감속시키는 이동 속도 감소 로직이 포함되어 있습니다.