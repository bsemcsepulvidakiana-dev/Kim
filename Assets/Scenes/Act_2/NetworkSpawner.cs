using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkSpawner : NetworkBehaviour
{
    [Header("Network Prefabs")]
    [SerializeField] private GameObject hostPrefab;
    [SerializeField] private GameObject clientPrefab;

    [Header("Spawn Positions")]
    [SerializeField] private Vector3 hostSpawnPos = new Vector3(-2, 1, 0);
    [SerializeField] private Vector3 clientSpawnPos = new Vector3(2, 1, 0);

    private Dictionary<ulong, NetworkObject> spawnedObjects = new Dictionary<ulong, NetworkObject>();

    // Dito natin itatago ang score ng bawat Client ID para manatili kahit mamatay
    public NetworkVariable<int> hostScore = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> clientScore = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public static NetworkSpawner Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void AddScore(ulong killerClientId)
    {
        if (!IsServer) return;

        if (killerClientId == NetworkManager.ServerClientId)
        {
            hostScore.Value += 1;
            Debug.Log($"Host Scored! Total: {hostScore.Value}");
        }
        else
        {
            clientScore.Value += 1;
            Debug.Log($"Client Scored! Total: {clientScore.Value}");
        }
    }

    public void RequestSpawn()
    {
        if (NetworkManager.Singleton != null)
        {
            SpawnObjectServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnObjectServerRpc(ulong requestingClientId)
    {
        if (spawnedObjects.ContainsKey(requestingClientId) && (spawnedObjects[requestingClientId] == null || !spawnedObjects[requestingClientId].IsSpawned))
        {
            spawnedObjects.Remove(requestingClientId);
        }

        if (spawnedObjects.ContainsKey(requestingClientId))
        {
            return;
        }

        bool isHost = (requestingClientId == NetworkManager.ServerClientId);
        GameObject prefabToSpawn = isHost ? hostPrefab : clientPrefab;
        Vector3 spawnPosition = isHost ? hostSpawnPos : clientSpawnPos;

        if (prefabToSpawn == null) return;

        GameObject newObj = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        NetworkObject netObj = newObj.GetComponent<NetworkObject>();

        if (netObj != null)
        {
            netObj.SpawnWithOwnership(requestingClientId);
            spawnedObjects[requestingClientId] = netObj;
        }
    }
}