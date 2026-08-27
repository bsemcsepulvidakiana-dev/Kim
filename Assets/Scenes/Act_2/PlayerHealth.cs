using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class NetworkPlayerHealth : NetworkBehaviour
{
    [Header("Health Settings")]
    public NetworkVariable<float> currentHealth = new NetworkVariable<float>(
        100f, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );

    [Header("Score Settings")]
    public NetworkVariable<int> score = new NetworkVariable<int>(
        0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );

    [Header("References")]
    [SerializeField] private Animator animator;

    private bool isDead = false;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    public override void OnNetworkSpawn()
    {
        currentHealth.OnValueChanged += OnHealthChanged;
    }

    public override void OnNetworkDespawn()
    {
        currentHealth.OnValueChanged -= OnHealthChanged;
    }

    public void TakeDamage(float amount, ulong attackerClientId)
    {
        if (!IsServer || isDead) return;

        currentHealth.Value -= amount;

        if (currentHealth.Value <= 0f)
        {
            currentHealth.Value = 0f;
            isDead = true;

            // Bigyan ng +1 Score ang nakapatay na player
            AddScoreToKiller(attackerClientId);

            // Simulan ang Death Animation at Despawn
            StartCoroutine(DeathAndDespawnSequence());
        }
    }

private void AddScoreToKiller(ulong killerClientId)
{
    if (NetworkSpawner.Instance != null)
    {
        NetworkSpawner.Instance.AddScore(killerClientId);
    }
}

    private void OnHealthChanged(float previousValue, float newValue)
    {
        if (newValue <= 0f && animator != null)
        {
            animator.SetTrigger("Die");
        }
    }

    private IEnumerator DeathAndDespawnSequence()
    {
        yield return new WaitForSeconds(2.5f);

        if (NetworkObject != null && NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn(true);
        }
    }
}