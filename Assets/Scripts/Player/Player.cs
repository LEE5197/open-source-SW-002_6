using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // 플레이어 오브젝트를 움직이기 위한 변수
    private Rigidbody2D rigid;
    private Vector2 moveVec = Vector2.zero;
    // 플레이어 오브젝트의 이동 속도
    public float moveSpeed = 8f;

    // 플레이어 오브젝트에서 총알을 발사하기 위한 변수
    private bool canFire = true;
    private bool isFire = false;

    // 총알 발사 딜레이 지정할 변수
    public float fireDelay = 0.2f;

    // 스크립터블 오브젝트를 통한 UI 연동
    public HealthSO healthSO;
    public ScoreSO scoreSO;

    // 궁극기 오브젝트 및 갯수
    public GameObject ultPrefabs;
    public UltCountSO ultCount;
    private GameObject ultObject;

    // 현재 활성화된 보조무기 오브젝트를 관리할 변수
    public List<GameObject> subWeaponList;
    private int subIdx = 0;

    [Header("Audio Clips")]
    public AudioClip PlayerShootClip;
    public AudioClip PlayerUltClip;


    // 플레이어 오브젝트 생성 세팅
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        ultObject = Instantiate(ultPrefabs);
        ultObject.SetActive(false);
    }
    // 프레임 단위로 플레이어 입력 따라서 작동할 로직
    private void Update()
    {
        if (isFire && canFire)
        {
            Fire();
        }
    }
    // 플레이어 움직임 등 물리적 상호작용 반영할 로직
    private void FixedUpdate()
    {
        Move();
    }
    // 플레이어의 입력에 따라 지정된 속도로 이동하는 함수
    // 이후 플레이어 피격 이벤트 등에 따라 추가 로직 구현 예정
    private void Move()
    {
        rigid.linearVelocity = moveVec * moveSpeed;
    }
    // 플레이어 총알 발사 함수
    private void Fire()
    {
        PlayerBullet bullet = GameManager.Instance.GetPlayerBullet();
        if (bullet == null) return;

        if (SoundManager.Instance != null) //효과음
        {
            SoundManager.Instance.PlaySfx(PlayerShootClip);
        }

        bullet.gameObject.SetActive(true);
        bullet.gameObject.transform.position = transform.position;
        bullet.moveVec = Vector2.up;

        StartCoroutine(FireCoroutine());
    }
    // 총알 발사 딜레이 확인용 코루틴
    IEnumerator FireCoroutine()
    {
        canFire = false;
        yield return new WaitForSeconds(fireDelay);
        canFire = true;
    }
	// 총알 충돌 확인
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == 9)    // Enemy Bullet 레이어 번호
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

            return;
            // 충돌 확인용
            // Debug.Log($"damage {bullet}");
		}

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

                case 15:    // 보조무기 추가 아이템
                    Debug.Log("sub weapon item");
                    if (subIdx < 4)
                    {
                        Debug.Log(subWeaponList.Count);
                        subWeaponList[subIdx].SetActive(true);
                        subIdx++;
                    }
                    break;
            }
            return;
		}
	}

    // 총알 데미지 증가
    
    void IncreaseBulletDamage(int weight)
	{
        /*
        float updateDamage = 0f;
        foreach(var it in defaultBulletList)
		{
            PlayerBullet tmp = it.GetComponent<PlayerBullet>();
            tmp.damage += weight;
            Debug.Log($"current bullet damage : { tmp.damage}");
            if (updateDamage != 0f) updateDamage = tmp.damage;
		}
        defaultBulletPrefab.GetComponent<PlayerBullet>().damage = updateDamage;
        */
    }

    // Input System 이용해서 플레이어 키 입력을 받기 위한 함수
    void OnMove(InputValue value)
    {
        moveVec = value.Get<Vector2>();
    }
    // Input System 이용해서 플레이어 키 입력을 받기 위한 함수
    void OnFire(InputValue value)
    {
        isFire = value.isPressed;
    }

    // 궁극기 횟수가 1 이상이고, 일정 시간이 지났다면 궁극기 사용
    void OnUlt(InputValue value)
	{
       // if (ult <= 0) return;
        if (ultObject.activeSelf) return;
        if(ultCount.UseUlt())
            StartCoroutine(ActiveUlt());
	}
    IEnumerator ActiveUlt()
	{
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySfx(PlayerUltClip);
        }

        ultObject.SetActive(true);
        ultObject.transform.position = new Vector2(0, -7f);
        yield return new WaitForSeconds(3f);
        ultObject.SetActive(false);
	}
}
