using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public GameObject bullet;
    public float fireDelay = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Fire());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Fire()
    {
        yield return new WaitForSeconds(fireDelay);
        Instantiate(bullet, this.gameObject.transform, this.transform.rotation);
    }
}
