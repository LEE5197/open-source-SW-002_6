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

    // 총알 오브젝트 프리펩
    public GameObject defaultBulletPrefab;
    // 오브젝트 폴링을 통한 총알 발사를 구현하기 위한 리스트, 현재 발사해야 할 총알 순서를 저장할 변수
    private List<GameObject> defaultBulletList;
    private int curIdx = 0;

    // 총알 발사 딜레이 지정할 변수
    public float fireDelay = 0.2f;

    // 플레이어 오브젝트 생성 세팅
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        // 총알 리스트에 총알 프리펩 할당
        defaultBulletList = new List<GameObject>();
        for(int i = 0; i < 30; i++)
        {
            GameObject bullet = Instantiate(defaultBulletPrefab);
            bullet.SetActive(false);
            defaultBulletList.Add(bullet);
        }
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
        // 현재 발사해야할 총알이 활성화 상태가 아니라면 리스트의 총알 갯수가 모자른 것이므로
        // 총알 리스트 확장
        if (defaultBulletList[curIdx].activeSelf)
        {
            curIdx = defaultBulletList.Count;
            for (int i = 0; i < 30; i++)
            {
                GameObject bullet = Instantiate(defaultBulletPrefab);
                bullet.SetActive(false);
                defaultBulletList.Add(bullet);
            }
            // 리스트 확장 확인용 
            // Debug.Log($"총알 리스트 확장 : {defaultBulletList.Count}");
        }
        
        // 현재 발사할 총알 위치 플레이어 위치로 조정 후 확성화
        defaultBulletList[curIdx].transform.position = transform.position;
        defaultBulletList[curIdx++].SetActive(true);
        curIdx %= defaultBulletList.Count;
        StartCoroutine(FireCoroutine());
    }
    // 총알 발사 딜레이 확인용 코루틴
    IEnumerator FireCoroutine()
    {
        canFire = false;
        yield return new WaitForSeconds(fireDelay);
        canFire = true;
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
}
