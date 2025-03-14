using Unity.Netcode;
using UnityEngine;

public class NetworkManagerScript : MonoBehaviour
{
    private void Start()
    {
        // Automatically handle disconnects
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    public void StartHost() => NetworkManager.Singleton.StartHost();
    public void StartClient() => NetworkManager.Singleton.StartClient();
    public void Shutdown() => NetworkManager.Singleton.Shutdown();

    private void OnClientDisconnect(ulong clientId)
    {
        Debug.Log($"Client {clientId} disconnected.");
    }
}
