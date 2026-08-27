using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkHealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private NetworkPlayerHealth playerHealth;

    private void Start()
    {
        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = 100;
            healthSlider.value = 100;
        }
    }

    private void Update()
    {
        if (playerHealth == null)
            return;

        healthSlider.value = playerHealth.currentHealth.Value;
    }
}