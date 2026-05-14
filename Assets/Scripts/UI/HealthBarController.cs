using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private HealthSO health;
    [SerializeField] private Slider healthSlider;

    void Awake()
    {
        if (!healthSlider)
        {
            Debug.LogError("Health Slider is null!");
            enabled = false;
            return;
        }

        healthSlider.minValue = 0;
        healthSlider.maxValue = 1;
    }
    void OnEnable()
    {
        if (!health)
        {
            Debug.LogError("Health is null!");
            enabled = false;
            return;
        }
        health.OnValueChanged += UIUpdate;
        UIUpdate(health.Value);
    }

    void OnDisable()
    {
        if(health) health.OnValueChanged -= UIUpdate;
    }
    
    void UIUpdate(int _)
    {
        healthSlider.value = health.Normalized;
    }
}
