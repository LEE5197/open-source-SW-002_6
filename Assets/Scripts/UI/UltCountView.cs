using System.Collections.Generic;
using UnityEngine;

public class UltCountView : MonoBehaviour
{
    [SerializeField] private GameObject ultIconItemPrefab;
    [SerializeField] private GameObject ultIconParent;
    private List<GameObject> ultCountItems = new List<GameObject>();
    private int maxValue;

    public void Initialize(int max)
    {
        if (ultCountItems.Count > 0) return;
        if (!ultIconParent)
        {
            Debug.LogError("[UltCountView] ultIconParent is not assigned");
            return;
        }
        maxValue = max;
        for (int i = 0; i < maxValue; i++)
        {
            GameObject item = Instantiate(ultIconItemPrefab, ultIconParent.transform);
            ultCountItems.Add(item);
            item.SetActive(false);
        }
    }

    public void ChangeCount(int value)
    {
        if (ultCountItems.Count == 0) return;
        int clamped = Mathf.Clamp(value, 0, maxValue);
        for (int i = 0; i < maxValue; i++)
            ultCountItems[i].SetActive(i < clamped);
    }
}
