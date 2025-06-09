using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

public class ServerFishNet : Feature {
    private NetworkManager manager;
    private int gameIdPool;
    private ServerRoleMovement movement;
    
    public override void OnInitialize(ref WorldLink link) {
        manager = InstanceFinder.NetworkManager;
        movement = link.RequireFeature<ServerRoleMovement>();
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
        var roleNetInstance = (GameObject)Object.Instantiate(roleNetAsset);
        var roleNetObj = roleNetInstance.GetComponent<NetworkObject>();
        var roleNet = roleNetInstance.GetComponent<RoleNet>();
        roleNet.Pos.Value = new Vector3(0, 0, 0);
        roleNet.Rot.Value = Quaternion.identity;
        roleNet.RoleId.Value = ++gameIdPool;
        roleNet.GameName.Value = $"GoodGuy_{gameIdPool}";
        manager.ServerManager.Spawn(roleNetObj, conn);
    }

    public void StartConnect() {
        manager.ServerManager.StartConnection();
    }
}