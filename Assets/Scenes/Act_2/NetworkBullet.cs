using Unity.Netcode;
using UnityEngine;

public class NetworkBullet : NetworkBehaviour
{
    public float speed = 15f;
    public float damage = 20f;
    public float lifetime = 3f;
    public ulong OwnerClientId;

    [Header("Damage Popup")]
    [SerializeField] private GameObject damagePopupPrefab;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }

        if (IsServer)
        {
            Destroy(gameObject, lifetime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        NetworkPlayerHealth health = other.GetComponent<NetworkPlayerHealth>();
        if (health != null)
        {
            if (health.OwnerClientId != OwnerClientId)
            {
                health.TakeDamage(damage, OwnerClientId);

                // Ipakita ang damage number sa lahat ng clients
                ShowDamagePopupClientRpc(transform.position, damage);

                Destroy(gameObject);
            }
        }
        else if (!other.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }

    [ClientRpc]
    private void ShowDamagePopupClientRpc(Vector3 hitPosition, float damageAmount)
    {
        if (damagePopupPrefab == null) return;

        GameObject popup = Instantiate(damagePopupPrefab, hitPosition + Vector3.up * 0.5f, Quaternion.identity);
        DamagePopup popupScript = popup.GetComponent<DamagePopup>();
        if (popupScript != null)
        {
            popupScript.SetDamage(damageAmount);
        }
    }
}