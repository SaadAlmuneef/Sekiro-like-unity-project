using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // References
    private HealthHandler healthHandler;

    [Header("UI Settings")]
    [SerializeField] float lerp_speed = 0.03f;

    public void UpdateImageBar(ref Slider image, float currentValue)
    {
        image.value = Mathf.Lerp(image.value, currentValue, lerp_speed);
    }


    private void Awake()
    {
        healthHandler = GetComponent<HealthHandler>();
        healthHandler.OnDeath += YouDied;
    }


    private void Start() => healthHandler.OnDeath += YouDied;
    private void OnDestroy() => healthHandler.OnDeath -= YouDied;

    private void YouDied()
    {

    }
}
