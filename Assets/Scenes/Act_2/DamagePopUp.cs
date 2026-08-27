using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private float lifetime = 1f;

    private float timer;

    private void Awake()
    {
        if (text == null)
            text = GetComponentInChildren<TMP_Text>();
    }

    public void SetDamage(float damageAmount)
    {
        if (text != null)
            text.text = "-" + damageAmount.ToString("0");
    }

    private void Update()
    {
        // Lumulutang paitaas
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Palagi harap sa camera
        if (Camera.main != null)
            transform.forward = Camera.main.transform.forward;

        timer += Time.deltaTime;

        // Unti-unting mawawala (fade out)
        if (text != null)
        {
            Color c = text.color;
            c.a = Mathf.Lerp(1f, 0f, timer / lifetime);
            text.color = c;
        }

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}