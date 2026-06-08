# UI 코드 관련 상세 설명
코드 상세 설명<br/>
[Player](./Player_Code_Details.md)<br/>
[Item](./Item_Code_Details.md)<br/>
[Enemy](./Enemy_Code_details.md)<br/>
[GameManager](./GameManager_Code_details.md)<br/>
[UI](./UI_Code_details.md)<br/>
[README](./README.md)<br/>
[Background](./Background_Code_details.md)<br/>
# HealthSO.cs 상세 설명

`HealthSO.cs`는 게임 내 캐릭터(플레이어 또는 적)의 체력 데이터를 관리하는 **스크립터블 오브젝트(Scriptable Object)** 클래스입니다. 데이터의 범위를 제한하는 `ClampedIntVariableSO`를 상속받아 체력이 최소값과 최대값 사이를 벗어나지 않도록 보장하며, 체력이 0이 되는 사망 시점에 C# 이벤트와 유니티 게임 이벤트를 동시에 발생시켜 다른 시스템과의 의존도를 낮추도록 설계했습니다.

```csharp
[Header("Events")]
[SerializeField] private GameEvent deathEvent;
public event Action OnDeath;
```
HealthSO.cs 클래스의 상단에서 외부에 신호를 보낼 이벤트 변수들을 선언합니다. 시스템 전역에서 관전할 수 있는 유니티 에셋 기반 이벤트 채널인 `deathEvent`와 내부 로직 스크립트들이 런타임에 구독할 수 있는 C# 표준 `Action`을 선언하여 이벤트 구조를 형성합니다.

<br/>

```csharp
public bool IsDead => runtimeValue <= minValue;
public bool IsFull => runtimeValue >= maxValue;
```
현재 런타임 체력 값(`runtimeValue`)을 상속받은 최소값(`minValue`) 및 최대값(`maxValue`)과 비교하여, 플레이어 오브젝트가 사망 상태인지 또는 체력이 가득 찼는지를 간결하게 판별하는 코드입니다.

<br/>

```csharp
public void Damage(int amount)
{
    if (amount <= 0) return;
    Value -= amount;
}

public void Heal(int amount)
{
    if (amount <= 0) return;
    Value += amount;
}
```
외부(피격 스크립트나 아이템 스크립트)에서 체력을 증감시키기 위해 호출하는 핵심 메소드입니다. 잘못된 양의 매개변수(0 이하의 데이터)가 흘러 들어와 체력이 역으로 치솟거나 깎이는 버그를 사전에 차단하는 예외 처리가 포함되어 있습니다. 부모 클래스인 `ClampedIntVariableSO`가 셋터(Setter) 내부에서 자체적으로 값의 상하한선을 필터링해 주므로, 누적 연산만 수행합니다.

<br/>

```csharp
protected override void OnValueChangedHook(int previous, int current)
{
    if (previous > minValue && current <= minValue)
    {
        OnDeath?.Invoke();
        if (deathEvent != null)
        {
            deathEvent.Raise();
        }
    }
}
```
부모 클래스에서 체력 값이 변경될 때마다 자동으로 실행되도록 설계된 `OnValueChangedHook` 메소드를 재정의(Override)한 영역입니다. 사망 조건이 충족되면 이벤트 구독자들과 유니티 전역 이벤트 리스너들에게 사망 신호(`deathEvent.Raise()`)를 보냅니다.

# ScoreSO.cs 상세 설명

`ScoreSO.cs`는 게임 내에서 획득하는 점수(Score) 데이터를 관리하는 **스크립터블 오브젝트(Scriptable Object)** 클래스입니다. 데이터의 상하한 범위를 제한하는 `ClampedIntVariableSO`를 상속받아 점수가 정의된 최소값과 최대값 사이를 유지하며, 외부 시스템에서 점수를 추가할 수 있는 직관적인 인터페이스를 제공합니다.

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreSO", menuName = "Scriptable Objects/ScoreSO")]
public class ScoreSO : ClampedIntVariableSO
{
```
스크립터블 오브젝트 생성을 위한 프로젝트 창 메뉴 경로(`Scriptable Objects/ScoreSO`)와 기본 파일명을 지정하는 클래스 선언부입니다. `ClampedIntVariableSO`를 부모 클래스로 상속받음으로써, 점수 데이터가 데이터 오버플로우나 유니티 인스펙터 상에서 설정한 최대 제한 수치를 넘지 않도록 보호하는 캡슐화 구조를 적용했습니다.

<br/>

```csharp
    public void AddScore(int amount)
    {
        if (amount <= 0) return;
        Value += amount;
    }
}
```
적 기체를 파괴하거나 아이템을 획득했을 때 외부 스크립트에서 호출하여 값을 변경하는 함수로 점수의 경우 감소하는 경우가 없으므로 증가 함수만 존재합니다.

# HighScoreSO.cs 상세 설명

`HighScoreSO.cs`는 게임의 최고 점수(High Score)를 관리하고 로컬 저장소에 영구히 저장하는 **스크립터블 오브젝트(Scriptable Object)** 클래스입니다. 유니티의 내장 저장 시스템인 `PlayerPrefs`를 활용하여 프로그램이 종료되더라도 최고 점수가 보존되도록 설계되었으며, 새로운 점수가 기존 최고 점수를 경신했을 때만 데이터를 갱신하는 로직이 적용되어 있습니다.

<br/>

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "HighScoreSO", menuName = "Scriptable Objects/HighScoreSO")]
public class HighScoreSO : ScriptableObject
{
    private const string PrefsKey = "HighScore";

    public int Value { get; private set; }
```
스크립터블 오브젝트 에셋 생성을 위한 선언부입니다. 로컬 저장소 기기(`PlayerPrefs`)에 데이터를 쓰고 읽을 때 에러를 방지하기 위해 고정된 키 값인 `PrefsKey`를 문자열 상수(`const`)로 지정해 안전성을 높였습니다. 

또한 현재 최고 점수를 보관하는 `Value`는 외부에서 자유롭게 읽을 수 있지만, 임의로 수정할 수 없도록 수정 권한을 클래스 내부로 제한하여 데이터의 무결성을 보장합니다.

<br/>

```csharp
    private void OnEnable()
    {
        Value = PlayerPrefs.GetInt(PrefsKey, 0);
    }
```
스크립터블 오브젝트 에셋이 메모리에 활성화되는 최초 시점에 자동으로 호출되는 유니티 이벤트 메소드입니다. 

기기에 기존 최고 점수 기록이 남아있다면 `PlayerPrefs.GetInt`를 통해 값을 가져와 `Value` 변수에 대입하며, 만약 게임을 처음 실행하여 저장된 기록이 없다면 기본값인 `0`으로 초기화합니다.

<br/>

```csharp
    public bool TrySave(int score)
    {
        if (score <= Value) return false;
        Value = score;
        PlayerPrefs.SetInt(PrefsKey, Value);
        PlayerPrefs.Save();
        return true;
    }
}
```
게임 오버 또는 스테이지 클리어 시점에 획득한 새로운 점수(`score`)를 전달받아 최고 점수 갱신을 시도하는 함수입니다.

매개변수로 들어온 점수가 현재 보관 중인 최고 점수(`Value`)보다 낮거나 같다면 기록 경신에 실패한 것이므로 아무런 작업 없이 `false`를 반환하고 즉시 종료합니다. 반대로 기존 기록을 넘어선 새로운 점수라면 값을 새 점수로 업데이트한 뒤, `PlayerPrefs`를 통해 로컬 스토리지에 값을 쓰고 `PlayerPrefs.Save()`로 디스크에 최종 영구 저장한 후 `true`를 반환합니다. 

# UltCountSO.cs 상세 설명

`UltCountSO.cs`는 게임 내 플레이어의 궁극기(Ultimate Ability) 사용 가능 횟수를 관리하는 **스크립터블 오브젝트(Scriptable Object)** 클래스입니다. 데이터의 범위를 제한하는 `ClampedIntVariableSO`를 상속받아 궁극기 개수가 정의된 최소값과 최대값 사이를 벗어나지 않도록 보장하며, 궁극기 소비 및 획득 로직을 제어합니다.


```csharp
using UnityEngine;
[CreateAssetMenu(fileName = "UltCountSO", menuName = "Scriptable Objects/UltCountSO")]

public class UltCountSO : ClampedIntVariableSO
{
```
스크립터블 오브젝트 에셋을 생성하기 위한 정의부입니다. `ClampedIntVariableSO`를 상속받아 동작하므로, 내부적으로 궁극기 개수가 음수로 떨어지거나 설정된 최대 보유량을 초과하는 현상을 부모 클래스 선에서 원천 차단하는 정형화된 데이터 설계 구조를 공유합니다.

<br/>

```csharp
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
```
플레이어가 궁극기 발동 키를 눌렀을 때 사용 가능 여부를 검증하고 개수를 차감하는 핵심 함수입니다.

현재 보유량(`Value`)이 1개보다 적다면 궁극기를 발동할 수 없는 상태이므로 `false`를 반환하며 연산을 종료합니다. 사용 가능한 상태(1 이상)라면 보유 개수를 1 감소시킨 뒤 궁극기 발동이 성공했음을 뜻하는 `true`를 반환합니다. 

주석 처리된 부분은 궁극기 UI와 실재 사용 가능한 궁극기 횟수가 제대로 연동되지 않아 코드를 수정한 흔적이며, 스크립터블 오브젝트 작성자와 궁극기와 UI 연동 버그 수정자가 달라 어떤 문제가 생길지 모르기 때문에 주석 처리하여 남겨놓은 흔적입니다.

<br/>

```csharp
    public void GetUlt()
    {
        Value++;
    }
}
```
게임 내에서 특정 게이지를 모두 채우거나, 보상 아이템을 획득했을 때 궁극기 사용 가능 횟수를 충전해 주는 메소드입니다.

단순 누적 연산(`Value++`)을 수행하지만, 부모 클래스인 `ClampedIntVariableSO`의 자체 시스템이 존재하여 인스펙터에 지정된 최대 수치를 넘어가더라도 자동으로 최대 수치에 고정(Clamp)되므로 조건문을 통한 제약 없이 연산을 수행할 수 있습니다.