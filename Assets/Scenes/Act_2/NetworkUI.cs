using Unity.Netcode;
using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    [Header("UI Panels / Containers")]
    [SerializeField] private GameObject connectionPanel; 
    [SerializeField] private GameObject spawnPanel;

    private void Start()
    {
        if (connectionPanel != null) connectionPanel.SetActive(true);
        if (spawnPanel != null) spawnPanel.SetActive(false);
    }

    public void StartHost()
    {
        if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsListening)
        {
            if (NetworkManager.Singleton.StartHost())
            {
                ShowSpawnUI();
            }
        }
    }

    public void StartClient()
    {
        if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsListening)
        {
            if (NetworkManager.Singleton.StartClient())
            {
                ShowSpawnUI();
            }
        }
    }

    private void ShowSpawnUI()
    {
        if (connectionPanel != null) connectionPanel.SetActive(false);
        if (spawnPanel != null) spawnPanel.SetActive(true);
    }
}