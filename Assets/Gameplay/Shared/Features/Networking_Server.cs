using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

public partial class NetworkRegistry {
    
    private void OnLoginRequest(NetworkConnection conn, FishBroadcastDefine.LoginRequest arg2, Channel arg3) {
        Debug.Log("1");
        manager.ServerManager.Broadcast(conn, new FishBroadcastDefine.LoginResult() {
            IsSuccess = true,
        });

        var asset = Resources.Load("Networking/RoleNet");
        var instance = (GameObject)Object.Instantiate(asset);
        var networkObject = instance.GetComponent<NetworkObject>();
        manager.ServerManager.Spawn(networkObject, conn);
    }
    
    private void OnRemoteClientConnectState(NetworkConnection conn, RemoteConnectionStateArgs arg2) {
        
    }
}