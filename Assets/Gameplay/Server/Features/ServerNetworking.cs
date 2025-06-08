using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

public class ServerNetworking : Feature {
    private NetworkManager manager;
    private int gameIdPool;
    private WorldType worldType;
    private Transform networkObjParent;
    private ServerRoleMovement movement;
    
    public override void OnInitialize(ref WorldLink link) {
        manager = InstanceFinder.NetworkManager;
        worldType = link.GetWorldType();
        movement = link.RequireFeature<ServerRoleMovement>();
        networkObjParent = new GameObject($"Network_{worldType}").transform;
        manager.ServerManager.RegisterBroadcast<FishBroadcastDefine.LoginRequest>(MsgLoginRequest);
        manager.ServerManager.RegisterBroadcast<FishBroadcastDefine.InputInfo>(MsgInputInfo);
    }

    public override void OnClear() {
        manager.ServerManager.UnregisterBroadcast<FishBroadcastDefine.LoginRequest>(MsgLoginRequest);
        manager.ServerManager.UnregisterBroadcast<FishBroadcastDefine.InputInfo>(MsgInputInfo);
    }

    private void MsgInputInfo(NetworkConnection arg1, FishBroadcastDefine.InputInfo arg2, Channel arg3) {
        movement.OnRoleInput(arg2.GameId, arg2.Move);
    }

    private void MsgLoginRequest(NetworkConnection conn, FishBroadcastDefine.LoginRequest arg2, Channel arg3) {
        var roleNetAsset = Resources.Load("Networking/RoleNet");
        var roleNetInstance = (GameObject)Object.Instantiate(roleNetAsset, networkObjParent, true);
        var roleNetObj = roleNetInstance.GetComponent<NetworkObject>();
        var roleNet = roleNetInstance.GetComponent<RoleNet>();
        roleNet.IsInReplayWorld = worldType == WorldType.Replay;
        roleNet.Pos.Value = new Vector3(0, 0, 0);
        roleNet.Rot.Value = Quaternion.identity;
        roleNet.GameId.Value = ++gameIdPool;
        roleNet.GameName.Value = $"GoodGuy_{gameIdPool}";
        // 对于回放世界,有一个特殊之处就是这里不需要同步给客户端
        if (worldType != WorldType.Replay) {
            manager.ServerManager.Spawn(roleNetObj, conn);
        }else {
            // 手动触发
            roleNet.OnStartServer();    
        }
    }

    public void StartConnect() {
        manager.ServerManager.StartConnection();
    }
}