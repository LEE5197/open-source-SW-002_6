# GameManager.cs 관련 코드 상세 설명
코드 상세 설명<br/>
[Player](./Player_Code_Details.md)<br/>
[Item](./Item_Code_Details.md)<br/>
[Enemy](./Enemy_Code_details.md)<br/>
[GameManager](./GameManager_Code_details.md)<br/>
[UI](./UI_Code_details.md)<br/>
[README](./README.md)<br/>
## GameManager.cs 상세 설명

`GameManager.cs`는 게임의 전체적인 라이프사이클(일시정지, 게임 오버, 게임 클리어)을 총괄하고, 실시간으로 대량 생성되는 탄환과 아이템의 메모리 최적화를 위한 **오브젝트 풀링(Object Pooling) 시스템**을 집중 관리하는 핵심 싱글톤 클래스입니다.

~~~csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsPaused { get; private set; }

    [Header("Events")]
    [SerializeField] private GameEvent resetEvent;
    public bool IsGameRunning = true;
    public Transform playerTransform;

    private float cachedTimeScale = 1f;

    [Header("ObjectPool")]
    public GameObject PlayerBulletPrefab;
    public GameObject EnemyBulletPrefab;
    public List<GameObject> itemprefabList;

    private Queue<PlayerBullet> playerBullets;
    private Queue<EnemyBullet> enemyBullets;
    private List<GameObject> items;

    [Header("CameraBorder")]
    public GameObject top;
    public GameObject bottom;
    public GameObject left;
    public GameObject right;
}
~~~
GameManager.cs 클래스의 상단에서 선언 및 초기화한 변수들입니다. 전역 어디서나 매니저에 접근할 수 있도록 인스턴스 전역 변수 `Instance`를 싱글톤 구조로 선언합니다. 일시정지 상태 여부 `IsPaused`, 게임 루프 진행 플래그 `IsGameRunning`, 플레이어 위치 참조값 `playerTransform`을 정의합니다.<br/>
추가로 화면 정지 시 기존 배속을 보관할 백업 변수 `cachedTimeScale`, 스크립터블 오브젝트 기반 이벤트 `resetEvent`를 할당합니다. 오브젝트 풀링을 구축하기 위해 플레이어/적 탄환 프리팹과 아이템 프리팹 리스트를 선언하고, 이를 **Queue** 및 **List** 형태로 보관할 풀링 저장소 변수들과 스테이지 경계선 오브젝트들을 배치합니다.

~~~csharp
private void Awake()
{
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
        return;
    }

    Instance = this;
    DontDestroyOnLoad(gameObject);

    if (resetEvent != null) resetEvent.RegisterListener(HandleReset);

    if (playerTransform == null)
        playerTransform = GameObject.FindWithTag("Player").transform;

    playerBullets = new Queue<PlayerBullet>();
    enemyBullets = new Queue<EnemyBullet>();
    items = new List<GameObject>();

    AddPlayerBullet();
    AddEnemyBullet();
    AddItem();
}
~~~
게임이 구동될 때 씬 전역에서 단 하나의 게임 매니저만 존재하도록 강제하는 싱글톤 보안 로직과 초기 풀 적재를 수행하는 생성자 단계 함수입니다.<br/>
이미 생성된 인스턴스가 있다면 중복 생성된 오브젝트를 `Destroy()` 함수 호출을 통해 파괴하고, 최초 생성된 매니저는 씬이 전환되어도 사라지지 않도록 **DontDestroyOnLoad**를 지정합니다. 리셋 이벤트 리스너를 할당하고 플레이어 참조 변수를 초기화 한 뒤, 탄환 오브젝트 폴에 사용할 큐(Queue)와 아이템용 리스트를 할당합니다. 이후 최적화를 위해 초기 기본 탄환과 아이템들을 미리 생성해 풀에 저장하는 내부 함수들을 차례로 호출합니다.

~~~csharp
private void OnDestroy()
{
    if (Instance != this) return;
    if (resetEvent != null) resetEvent.UnregisterListener(HandleReset);
}
~~~
게임 오버 후 방을 나가거나 게임이 완전히 종료되어 매니저가 파괴되는 시점에 호출되는 함수입니다.<br/>
자신이 정식 싱글톤 인스턴스일 경우에만 `resetEvent`에 등록했던 이벤트 리스너 메서드를 해제 `UnregisterListener`하여, 메모리 누수 및 연동 에러를 안전하게 방지합니다.

~~~csharp
private void HandleReset()
{
    IsGameRunning = true;
    Resume();
}
~~~
스테이지 리셋 또는 재시작 이벤트 `resetEvent`가 발행되었을 때 자동으로 실행되는 콜백 함수입니다. 게임 진행 플래그를 다시 가동 상태 `true`로 복원하고, 멈춰있던 게임 속도를 정상화하는 `Resume()` 함수를 호출합니다.

~~~csharp
public void Pause()
{
    if (IsPaused) return;
    cachedTimeScale = Time.timeScale;
    Time.timeScale = 0f;
    IsPaused = true;
}

public void Resume()
{
    if (!IsPaused) return;
    Time.timeScale = cachedTimeScale;
    IsPaused = false;
}
~~~
게임의 흐름을 제어하는 일시정지 및 해제 시스템입니다.<br/>
**Pause() :** 현재 구동 중인 게임 배속 배율 `Time.timeScale`을 `cachedTimeScale`에 저장한 뒤, 배속을 `0f`로 변경하여 씬 내의 모든 물리, 코루틴, Update 연산을 정지시킵니다.<br/>
**Resume() :** 일시정지를 해제할 때는 백업해 두었던 원래의 게임 배속 수치 `cachedTimeScale`를 다시 `Time.timeScale`에 대입하여 플레이가 하던 시점의 속도로 복구합니다.

~~~csharp
public void NotifyGameOver()
{
    IsGameRunning = false;
    Pause();
}
    
public void NotifyGameClear()
{
    IsGameRunning = false;
    Pause();
}
~~~
플레이어가 파괴되거나 스테이지 보스를 파괴하여 게임의 클리어 여부가 결정되었을 때 외부 스크립트에서 호출되는 함수입니다. 게임 실행 플래그를 정지 `false`로 변환하고 즉시 `Pause()`를 호출하여 게임 화면을 정지 상태로 고정합니다.

~~~csharp
private void AddPlayerBullet()
{
    for(int i = 0; i < 30; i++)
    {
        GameObject bullet = Instantiate(PlayerBulletPrefab, transform.position, Quaternion.identity, null);
        bullet.SetActive(false);

        playerBullets.Enqueue(bullet.GetComponent<PlayerBullet>());
    }
}

public PlayerBullet GetPlayerBullet()
{
    if (playerBullets.Count == 0)
    {
        AddPlayerBullet();
    }

    return playerBullets.Dequeue();
}

public void ReturnPlayerBullet(PlayerBullet bullet)
{
    bullet.gameObject.SetActive(false);
    playerBullets.Enqueue(bullet);
}
~~~
플레이어 탄환 오브젝트 풀링의 **생성-대여-반환** 사이클을 담당하는 함수입니다.<br/>
**AddPlayerBullet:** 한 번에 30발의 탄환 프리팹을 `Instantiate`로 대량 생산한 뒤, 씬 화면에 바로 보이지 않도록 `SetActive(false)`를 호출하여 비활성 처리합니다. 이후 탄환의 `PlayerBullet` 컴포넌트만을 추출하여 오브젝트 폴 `playerBullets`에 **Enqueue**하여 적재합니다.<br/>
**GetPlayerBullet:** 총알이 필요한 외부 스크립트에서 호출하는 함수입니다. 순간적으로 총알이 부족해 큐가 비어있다면 30발을 자동 추가 한 뒤, 최상단에 대기 중인 탄환 하나를 풀에서 **DeQueue**하여 반환합니다.<br/>
**ReturnPlayerBullet:** 벽에 부딪히거나 적에게 격추된 탄환을 파괴하지 않고 폴에 반환하는 함수입니다. 총알 오브젝트를 비활성화하여 화면에서 감춘 뒤 다시 오브젝트 폴에 **Enqueue**하여 재사용합니다.

~~~csharp
private void AddEnemyBullet()
{
    for(int i = 0; i < 30; i++)
    {
        GameObject bullet = Instantiate(EnemyBulletPrefab, transform.position, Quaternion.identity, null);
        bullet.SetActive(false);

        enemyBullets.Enqueue(bullet.GetComponent<EnemyBullet>());
    }
}

public EnemyBullet GetEnemyBullet()
{
    if (enemyBullets.Count == 0)
    {
        AddEnemyBullet();
    }

    return enemyBullets.Dequeue();
}

public void ReturnEnemyBullet(EnemyBullet bullet)
{
    bullet.gameObject.SetActive(false);
    enemyBullets.Enqueue(bullet);
}
~~~
적군 및 보스의 대량 탄막 패턴을 뒷받침하기 위한 적군 탄환 `EnemyBullet` 전용 오브젝트 풀링 함수입니다. 앞선 플레이어 탄환 제어 구조와 완벽히 동일한 메커니즘(30발 단위 예비 적재, 자동 추가 적재, 오브젝트 폴에 반환)으로 작동하여 총알 발사에 따른 컴퓨터 자원 소모를 줄였습니다.

~~~csharp
private void AddItem()
{
    for(int i = 0; i < itemprefabList.Count; i++)
    {
        GameObject item = Instantiate(itemprefabList[i], transform.position, Quaternion.identity, null);
        item.SetActive(false);
        items.Add(item);
    }
}

public void GetItem(Vector2 pos)
{
    int n = Random.Range(0, 10);
    if (n > 2) return;

    n = Random.Range(0, items.Count);
    if (items[n].activeSelf) return;

    items[n].transform.position = pos;
    items[n].SetActive(true);
}
~~~
적이 죽었을 때 무작위 확률로 스테이지에 아이템을 드랍하는 풀링 함수입니다.<br/>
인스펙터 창에 등록된 아이템 프리팹 개수만큼 루프를 돌며 아이템 오브젝트들을 미리 생성해 비활성화한 뒤 오브젝트 폴에 사용할 `List`에 저장합니다.<br/>
적이 파괴되면 `GetItem()`을 좌표(`pos`)를 인자로 호출하면 첫 번째 난수 검사(`Random.Range(0, 10)`)를 시행합니다. 값이 2 이하일 때만 통과하도록 제어하여 아이템 드롭률을 30%로 설정했습니다. 확률을 통과하면 풀 내에서 다시 무작위로 인덱스를 하나 선택하고, 만약 해당 아이템이 이미 스테이지에 활성화 된 상태가 아니라면 해당 적이 파괴된 위치(`pos`)로 아이템 좌표를 이동시킨 뒤 `SetActive(true)`를 호출하여 아이템을 활성화해서 스테이지에 드롭합니다.

## SoundManager.cs 상세 설명

`SoundManager.cs`는 게임 내의 배경음악(BGM)과 효과음(SFX)을 관리하는 클래스입니다. 전역에서 쉽게 접근할 수 있도록 싱글톤 패턴이 적용되어 있으며, 여러 효과음이 동시에 겹치더라도 끊김 없이 부드럽게 출력될 수 있도록 오디오 컴포넌트를 미리 늘려두고 번갈아 사용하는 **멀티 채널 **로 구현했습니다.

~~~csharp
public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private int maxChannels = 10; // 동시에 날 수 있는 최대 효과음 개수
    private AudioSource[] sfxChannels;
    private int currentChannelIndex = 0;

    public AudioSource BGMChannel;

    [Header("Audio Clips")]
    public AudioClip BasicBGMClip;
    public AudioClip UISoundClip;
    public AudioClip BossEntryClip;

    private bool IsBGMOn = false;

    //싱글톤 패턴
    public static SoundManager Instance;
}
~~~
SoundManager.cs 클래스 상단에서 선언된 멤버 변수들입니다. 게임 실행 중 동시에 재생 가능한 효과음의 최대 한계치 `maxChannels = 10`와 이를 구현할 `AudioSource` 배열, 현재 몇 번째 채널을 사용할지 가리키는 인덱스 변수를 정의합니다. 또한 독립적인 제어가 필요한 배경음악 전용 컴포넌트 `BGMChannel`와 기본 BGM, UI 연출음, 보스 등장 임팩트 오디오 클립 에셋을 확보하고, 전역 어디서든 싱글톤 형태로 즉시 호출할 수 있도록 인스턴스 전역 변수 `Instance`를 선언합니다.

~~~csharp
private void Awake()
{
    if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
    else { Destroy(gameObject); return; }

    //오디오 소스를 원하는 개수만큼.
    sfxChannels = new AudioSource[maxChannels];
    for (int i = 0; i < maxChannels; i++)
    {
        sfxChannels[i] = gameObject.AddComponent<AudioSource>();
        sfxChannels[i].playOnAwake = false;
        sfxChannels[i].spatialBlend = 0f;
    }

    if (BGMChannel == null) BGMChannel = gameObject.AddComponent<AudioSource>();

    if (IsBGMOn == false)
    {
        //AudioClip clip = ;
        SoundManager.Instance.PlayBGM(SoundManager.Instance.BasicBGMClip);
    }
}
~~~
게임이 처음 구동될 때 단 하나만 존재해야 하는 사운드 매니저의 유일성을 보장하고, 사운드 채널들을 선제적으로 동적 적재하는 초기화 루틴입니다.<br/>
**싱글톤 보장:** 씬 내에 중복 인스턴스가 잡히면 자기 자신을 파괴(`Destroy`)하고, 최초 생성된 매니저는 씬이 넘어가도 살아있도록 `DontDestroyOnLoad`를 설정합니다.<br/>
**멀티 채널:** 효과음의 병렬 재생을 위해 `maxChannels(10개)`만큼 반복문을 돌며 `AddComponent`로 오디오 소스를 컴포넌트로 배열에 담습니다. 이때 씬이 켜지자마자 소리가 나는 현상을 막기 위해 `playOnAwake = false`를 주며, 슈팅 게임 특성상 거리나 방향에 상관없이 소리가 균일하게 들리도록 3D 입체 음향 수치인 `spatialBlend`를 `0f`로 설정합니다.<br/>
**기본 BGM:** BGM 전용 오디오 소스를 최종 확인 및 장착한 뒤, 배경음이 꺼져있는 최초 런타임 상태 `IsBGMOn == false`라면 즉시 내장된 기본 BGM `BasicBGMClip`을 출력합니다.

~~~csharp
public void PlayBGM(AudioClip clip)
{
    if (clip == null) return;

    BGMChannel.clip = clip;
    BGMChannel.loop = true;
    BGMChannel.Play();
}
~~~
상시 출력해야 하는 배경음악 전용 재생 함수입니다.<br/>
인자로 전달받은 음원 에셋의 유효성을 검사한 뒤 `BGMChannel`에 클립을 참조합니다. 게임 도중 배경음이 중간에 뚝 끊기지 않도록 반복 재생 플래그인 `loop = true`를 할당한 뒤 BGM을 실행합니다.

~~~csharp
public void PlayBossBGM(AudioClip clip)
{
    StartCoroutine(BossSoundSequenceRoutine(BossEntryClip, clip));
}

private IEnumerator BossSoundSequenceRoutine(AudioClip entryClip, AudioClip bgmClip)
{
    if (entryClip == null || bgmClip == null) yield break;

    // 1. 보스 엔트리 사운드 세팅 및 재생
    //PlayBGM(entryClip);

    // 2. 엔트리 사운드의 길이(초)만큼 정확하게 대기합니다.
    //yield return new WaitForSeconds(entryClip.length);

    // 3. 엔트리 소리가 끝나면 바로 보스 본 BGM으로 교체 후 무한 반복 재생
    PlayBGM(bgmClip);
}
~~~
일반 보스 테마곡을 보스가 등장하는 타이밍에 맞춰 본 게임 보스전 BGM으로 넘어가도록 설계된 오디오 시스템입니다.<br/>

~~~csharp
public void PlaySfx(AudioClip clip)
{
    if (clip == null) return;

    // 순환 큐(Circular Queue) 방식으로 다음 채널을 선택해 재생
    AudioSource currentChannel = sfxChannels[currentChannelIndex];
    currentChannel.clip = clip;
    currentChannel.loop = false;
    currentChannel.Play();

    // 인덱스를 넘겨서 다음 소리 준비 (오버플로우 방지)
    currentChannelIndex = (currentChannelIndex + 1) % maxChannels;
}
~~~
플레이어 탄환 발사, 적 오브젝트 파괴, 아이템 획득 등 스테이지 전역에서 호출되는 효과음을 처리하는 사운드 출력 함수입니다.<br/>
하나의 오디오 소스만 사용해 효과음을 틀면 새로운 소리가 날 때 기존 소리가 뚝 끊기는 문제가 발생하는데, 이를 해결하기 위해 미리 생성해 둔 10개의 채널 배열을 **순환 큐**로 번갈아 재사용합니다.<br/>
이번 차례에 해당하는 인덱스의 `AudioSource`를 지정해 클립을 얹고, 단발성 사운드이므로 반복 재생을 해제(`loop = false`)한 뒤 독립적으로 재생합니다. 재생이 끝나면 다음 발사 통로를 지정하기 위해 인덱스를 1 가산한 뒤, 배열 범위 최대치(`maxChannels`)로 매번 나머지 연산 처리를 함으로써 배열 범위 초과(IndexOutOfRangeException) 에러 없이 `0번부터 9번` 채널까지 반복해서 돌며 사운드를 덮어쓰는 순환 풀링으로 구현했습니다.

# SpawnEnemy.cs 상세 설명

`SpawnEnemy.cs`는 게임 진행 중에 3종의 적 오브젝트를 자동 생성하고, 특정 시간이 지나면 보스를 스테이지에 등장시키는 클래스입니다. 시간에 따라 적 스폰 주기가 빨라지는 난이도 조절 시스템과 이전에 생성한 오브젝트 종류에 따라 다음 오브젝트 생성을 판단하는 로직이 포함됩니다.

### 1. 변수 선언 및 초기화 파트
`SpawnEnemy.cs` 클래스 상단에서 선언된 멤버 변수들입니다. 인스펙터에서 등록할 적 오브젝트를 프리팹 보관 리스트 `EnemyPrefabList`와 기체 종류를 직관적으로 판별하기 위한 열거형 데이터타입 `EnemyType`을 정의합니다. 또한 적이 리스폰되는 기본 주기인 스폰 딜레이 상수와, 바로 직전에 소환되었던 기체 유형을 기억하여 중복 생성을 억제할 비교용 변수 `previousEnemy`를 선언합니다.

```csharp
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

/*
1. 우측/좌측에서 화면으로 등장하는 기체 2기
2. 상단에서 등장해 화면 내에 머무는 기체 2기
3. 상단에서 빠르게 하강하는 기체 1기
 */

public class SpawnEnemy : MonoBehaviour
{
    public List<GameObject> EnemyPrefabList; //적 기체 종류

    public float EnemySpawnDelay = 2f; //스폰 딜레이 (3 초과 필수)
    public float spawnAngle = 15f;
    enum EnemyType { NORMAL, BIG, FAST } //적 타입

    private EnemyType previousEnemy = EnemyType.FAST;

    ...
}
```

~~~csharp
    private void Start()
    {
        StartCoroutine(Spawn());
        StartCoroutine(SpawnBoss());
    }
~~~
게임 스테이지가 시작되어 스크립트가 활성화되는 최초 시점에 일반 적들을 생성하는 Spawn() 코루틴과, 정해진 유효 플레이 시간 오버 시 최종 보스를 생성하는 SpawnBoss() 코루틴을 동시에 동작합니다. 각 코루틴이 독립적으로 동작하기 때문에 보스 오브젝트 생성 후에도 일반 적 오브젝트가 등장하도록 설계하여 난이도를 조절했습니다.

~~~csharp
IEnumerator Spawn()
    {
        while (GameManager.Instance.IsGameRunning == true) //게임이 실행 중이면
        {
            yield return new WaitForSeconds(EnemySpawnDelay); //지연시간만큼 대기
            if (EnemySpawnDelay > 1.5f)
                EnemySpawnDelay -= 0.2f;
            if (EnemySpawnDelay < 1.5f)
                EnemySpawnDelay = 1f;

            EnemyType enemyType = (EnemyType)Random.Range(0, 3); //계속 다른걸로 뽑기
            while (previousEnemy == enemyType)
                enemyType = (EnemyType)Random.Range(0, 3);
            previousEnemy = enemyType;
            //enemyType = EnemyType.BIG;        //디버그용

            Vector3 enemyVector;
            float angle;
            //enemyType에 따라 Vector와 앵글을 정해주고 소환
            switch (enemyType)
            {
                case EnemyType.NORMAL:
                    enemyVector = new Vector3(((Random.Range(0, 2) * 2) - 1) * GameManager.Instance.right.transform.position.x + 2f, Random.Range(-2, 2), 0);

                    angle = Random.Range(90f - spawnAngle / 2f, 90 + spawnAngle / 2f);
                    if (enemyVector.x < 0)
                        angle *= -1;

                    Instantiate(EnemyPrefabList[(int)enemyType], enemyVector, Quaternion.Euler(0f, 0f, angle));
                    yield return new WaitForSeconds(0.2f);
                    Instantiate(EnemyPrefabList[(int)enemyType], enemyVector, Quaternion.Euler(0f, 0f, angle));

                    break;
                case EnemyType.BIG:
                    enemyVector = new Vector3(5f, GameManager.Instance.top.transform.position.y + 2f, 0);
                    
                    angle = 180f;

                    Instantiate(EnemyPrefabList[(int)enemyType], enemyVector, Quaternion.Euler(0f, 0f, angle));
                    enemyVector.x *= -1;
                    Instantiate(EnemyPrefabList[(int)enemyType], enemyVector, Quaternion.Euler(0f, 0f, angle));

                    break;
                case EnemyType.FAST:
                    enemyVector = new Vector3(Random.Range(-8f, 8f), GameManager.Instance.top.transform.position.y + 2f, 0);

                    angle = 180f;

                    Instantiate(EnemyPrefabList[(int)enemyType], enemyVector, Quaternion.Euler(0f, 0f, angle));

                    break;
            }
        }
    }
~~~
게임이 정상 구동 중인 상태(IsGameRunning == true)라면 설정된 시간 초만큼 대기한 뒤 적을 소환하는 흐름을 무한히 반복합니다.<br/>
이때 while 루프마다 스폰 지연시간을 0.2초씩 줄여 시간이 지날수록 스테이지에 적이 더 빈번하게 스폰되게 구현했습니다. 단, 연산 과부하 및 오버플로우 방지를 위해 스폰 최소 한계선은 1초로 둡니다.<br/>
동시에 유니티 내장 난수 함수 `Random.Range`로 추출한 타겟 값이 직전 루프에서 출격했던 기체와 완벽히 똑같다면 서로 다른 값이 잡힐 때까지 while 루프를 돌려, 스폰되는 적 기체가 다양하도록 만들었습니다.<br/>
**switch-case**는 계산된 적 기체 종류에 따라 화면의 스폰 포인트 벡터 위치와 회전각을 연산하여 게임 월드에 인스턴스화 시키는 핵심 분기 영역입니다.<br/>
NORMAL: ((Random.Range(0, 2) * 2) - 1) 공식을 대입하여 1 혹은 -1의 극단적인 부호를 획득합니다. 이를 화면 우측 경계선 x 좌표값에 대입하여 좌측 외곽 혹은 우측 진행 방향에 따라 각도를 조절한 뒤 0.2초 간격의 시차 코루틴 지연을 주어 2기의 기체가 이열종대로 스폰되게 만들었습니다.<br/>
BIG: 화면 최상단 우측(X = 5f) 좌표에 아래 방향(180도)을 바라보도록 첫 번째 대형 기체를 스폰합니다. 그 후 좌표값 변수에 단항 연산자 *= -1을 주입하여 정확히 반대편 대칭축인 좌측 상단(X = -5f)으로 위치를 변경한 뒤 두 번째 기체를 스폰함으로써, 화면 위쪽 좌우 공간에서 두 개의 적 오브젝트가 동시에 스폰됩니다.<br/>
FAST: 가로축 전체 공간(-8f ~ 8f) 내에서 완전한 무작위 난수 X 좌표를 연산하고, 180도 회전 상태로 한 기만 스폰해 스테이지를 이동합니다..

~~~csharp
[Header("Boss")]
    private bool IsBossSpawned = false;
    public GameObject Boss;
    public float BossSpawnDelay = 60f;
    IEnumerator SpawnBoss()
    {
        yield return new WaitForSeconds(BossSpawnDelay + EnemySpawnDelay);

        StopCoroutine(Spawn());

        if (IsBossSpawned == false)
        {
            Instantiate(Boss, new Vector3(0, GameManager.Instance.top.transform.position.y + 4f, 0), Quaternion.Euler(0f, 0f, 0f));
        }
    }
}
~~~
스테이지 시작 이후 60초 이후 보스를 스폰하기위한 코루틴입니다.<br/>
보스 스폰 시점에 도달하면 StopCoroutine(Spawn()) 명령을 하달하여, 스테이지에 일반 적 오브젝트를 스폰하는 루프를 일시 정지합니다. 스테이지에 보스 중복 소환 에러 방지용 안전 플래그(IsBossSpawned)를 체크한 뒤, 화면 최상단 정중앙 너머 대기 영역에 최종 보스 오브젝트를 스폰합니다.