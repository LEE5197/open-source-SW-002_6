using System;
using System.Collections.Generic;
using UnityEngine;

public class UltCountView : MonoBehaviour
{
    [SerializeField] private GameObject ultIconItemPrefab;
    [SerializeField] private GameObject ultIconParent;
    private List<GameObject> ultCountItems = new List<GameObject>();
    private int maxValue;

    /*
     * max 수치만큼 아이콘 미리 생성 후 비활성.
     * presenter에서 실행해줌
     */
    public void Initialize(int max)
    {
        maxValue = max;
        for (int i = 0; i < maxValue; i++)
        {
            GameObject item = Instantiate(ultIconItemPrefab, ultIconParent.transform);
            ultCountItems.Add(item);
            item.SetActive(false);
        }
    }

    /*
     * value 수치만큼 Icon 활성화
     */
    public void ChangeCount(int value)
    {
        if(value<0)value=0;
        else if(value>maxValue)value=maxValue;
        for (int i = 0; i < maxValue; i++)
        {
            ultCountItems[i].SetActive(i<value);
        }
    }

    
}
