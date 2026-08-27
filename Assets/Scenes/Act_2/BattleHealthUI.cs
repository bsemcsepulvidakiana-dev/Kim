using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Nilikha para gumana ang TextMeshPro

public class BattleHealthUI : MonoBehaviour
{
    [Header("HP Bars")]
    [SerializeField] private Slider player1HealthBar;
    [SerializeField] private Slider player2HealthBar;

    [Header("Score Text UI (TextMeshPro)")]
    [SerializeField] private TMP_Text player1ScoreText; // Pinalitan ng TMP_Text
    [SerializeField] private TMP_Text player2ScoreText; // Pinalitan ng TMP_Text

    private NetworkPlayerHealth player1;
    private NetworkPlayerHealth player2;

private void Update()
{
    if (player1 == null || player2 == null)
    {
        FindPlayers();
    }

    if (player1 != null && player1.IsSpawned)
    {
        if (player1HealthBar != null)
            player1HealthBar.value = player1.currentHealth.Value;
    }

    if (player2 != null && player2.IsSpawned)
    {
        if (player2HealthBar != null)
            player2HealthBar.value = player2.currentHealth.Value;
    }

    // Score galing sa persistent NetworkSpawner, hindi sa player object
    if (NetworkSpawner.Instance != null)
    {
        if (player1ScoreText != null)
            player1ScoreText.text = "P1 Kills: " + NetworkSpawner.Instance.hostScore.Value;

        if (player2ScoreText != null)
            player2ScoreText.text = "P2 Kills: " + NetworkSpawner.Instance.clientScore.Value;
    }
}

    private void FindPlayers()
    {
        NetworkPlayerHealth[] players = FindObjectsByType<NetworkPlayerHealth>(FindObjectsSortMode.None);

        foreach (NetworkPlayerHealth player in players)
        {
            if (!player.IsSpawned)
                continue;

            if (player.OwnerClientId == NetworkManager.ServerClientId)
            {
                player1 = player;
            }
            else
            {
                player2 = player;
            }
        }
    }
}   