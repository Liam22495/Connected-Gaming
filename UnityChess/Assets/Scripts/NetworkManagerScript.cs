using Unity.Netcode;
using UnityEngine;

public class NetworkManagerScript : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject); // Persist NetworkManager across scenes

        // Listen for player connections & disconnections
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    public void StartHost()
    {
        if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Host Started");
        }
    }

    public void StartClient()
    {
        if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Client Connecting...");
        }
    }

    public void RejoinSession()
    {
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
        {
            Debug.LogWarning("Already in a session.");
            return;
        }

        NetworkManager.Singleton.StartClient();
        Debug.Log("Rejoining session...");
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} has connected.");
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} has disconnected.");

        // Check if the client was rejected (common cause of failure)
        if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
        {
            Debug.LogError("Connection failed: Client disconnected unexpectedly.");
        }
    }
}
