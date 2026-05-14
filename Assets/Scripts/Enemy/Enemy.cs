using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public GameObject enemyBullet; //적 bullet 프리팹
    public float fireDelay = 1f; //공격 속도
    public int bulletNum = 1; //연속으로 발사하는 총알 숫자 

    private List<GameObject> EnemyBulletList;
    private int curIdx = 0;

    void Start()
    {
        StartCoroutine(Fire(bulletNum)); //발사 시작

        EnemyBulletList = new List<GameObject>();
        for (int i = 0; i < 50; i++)
        {
            GameObject bullet = Instantiate(enemyBullet);
            bullet.SetActive(false);
            EnemyBulletList.Add(bullet);
        }
    }

    IEnumerator Fire(int bulletNum)
    {
        while (true)
        {
            yield return new WaitForSeconds(fireDelay); //지연 시간만큼 대기

            for (int i = 0; i < bulletNum; i++) //총알 개수만큼 연속 발사
            {
                yield return new WaitForSeconds(0.1f);

                EnemyBulletList[curIdx].transform.position = transform.position; //총알 생성
                EnemyBulletList[curIdx++].SetActive(true);
                curIdx %= EnemyBulletList.Count;
            }
        }
    }

    // 총알의 충돌을 감지하기위한 함수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy") return;

        // 오브젝트 폴링 기법 사용을 위한 충돌 시 오브젝트 비활성화 설정
        this.gameObject.SetActive(false);
    }
}
