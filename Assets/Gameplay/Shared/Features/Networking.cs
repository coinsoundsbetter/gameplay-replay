using FishNet.Managing;
using UnityEngine;
using Object = UnityEngine.Object;

public partial class NetworkRegistry {
    private NetworkManager manager;
    private bool isClient;
    private bool isServer;

    public void OnReady(bool client, bool server) {
        isClient = client;
        isServer = server;
        var asset = Resources.Load("Networking/NetworkManager");
        var instance = (GameObject)Object.Instantiate(asset);
        manager = instance.GetComponent<NetworkManager>();
    }

    public void OnClean() {
        if (isClient) {
            manager.ClientManager.StopConnection();
        }
        if (isServer) {
            manager.ServerManager.StopConnection(false);
        }
    }
}