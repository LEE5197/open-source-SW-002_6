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

    public float EnemySpawnDelay = 2f; //스폰 딜레이
    public float spawnAngle = 15f;
    enum EnemyType { NORMAL, BIG, FAST } //적 타입

    //스폰 시작
    private void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while (GameManager.Instance.IsGameRunning == true) //게임이 실행 중이면
        {
            yield return new WaitForSeconds(EnemySpawnDelay); //지연시간만큼 대기

            EnemyType enemyType = (EnemyType)Random.Range(0, 3);
            //enemyType = EnemyType.BIG;        //디버그용

            Vector3 enemyVector;
            float angle;
            //enemyType에 따라 Vector와 앵글을 정해주고 소환
            switch (enemyType)
            {
                case EnemyType.NORMAL:
                    enemyVector = new Vector3(((Random.Range(0, 2) * 2) - 1) * 9, Random.Range(-2, 2), 0);

                    angle = Random.Range(90f - spawnAngle / 2f, 90 + spawnAngle / 2f);
                    if (enemyVector.x < 0)
                        angle *= -1;

                    Instantiate(EnemyPrefabList[(int)enemyType], enemyVector, Quaternion.Euler(0f, 0f, angle));
                    yield return new WaitForSeconds(0.2f);
                    Instantiate(EnemyPrefabList[(int)enemyType], enemyVector, Quaternion.Euler(0f, 0f, angle));

                    break;
                case EnemyType.BIG:
                    enemyVector = new Vector3(5f, 6f, 0);

                    angle = 180f;

                    Instantiate(EnemyPrefabList[(int)enemyType], enemyVector, Quaternion.Euler(0f, 0f, angle));
                    enemyVector.x *= -1;
                    Instantiate(EnemyPrefabList[(int)enemyType], enemyVector, Quaternion.Euler(0f, 0f, angle));

                    break;
                case EnemyType.FAST:
                    enemyVector = new Vector3(Random.Range(-8f, 8f), 5f, 0);

                    angle = 180f;

                    Instantiate(EnemyPrefabList[(int)enemyType], enemyVector, Quaternion.Euler(0f, 0f, angle));

                    break;
            }
        }
    }
}
