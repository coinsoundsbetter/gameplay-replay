using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

public class ServerFishNet : Feature {
    private NetworkManager manager;
    private int gameIdPool;
    
    public override void OnInitialize(ref WorldLink link) {
        manager = InstanceFinder.NetworkManager;
        manager.ServerManager.RegisterBroadcast<FishBroadcastDefine.LoginRequest>(MsgLoginRequest);
        manager.ServerManager.RegisterBroadcast<FishBroadcastDefine.InputInfo>(MsgInputInfo);
    }

    public override void OnClear() {
        manager.ServerManager.UnregisterBroadcast<FishBroadcastDefine.LoginRequest>(MsgLoginRequest);
        manager.ServerManager.UnregisterBroadcast<FishBroadcastDefine.InputInfo>(MsgInputInfo);
    }

    private void MsgInputInfo(NetworkConnection conn, FishBroadcastDefine.InputInfo ent, Channel channel) {
        var worlds = WorldManager.Instance.GetWorlds(WorldType.Server | WorldType.Replay);
        foreach (var w in worlds) {
            w.GetFeature<ServerEventHandler>().OnRoleInput(ent.GameId, ent.Move);
        }
    }

    private void MsgLoginRequest(NetworkConnection conn, FishBroadcastDefine.LoginRequest request, Channel channel) {
        SpawnRole(conn);   
    }

    public void StartConnect() {
        manager.ServerManager.StartConnection();
    }

    public void SpawnGame(GameMode mode) {
        var asset = Resources.Load("Networking/GameNet");
        var instance = (GameObject)Object.Instantiate(asset);
        var netObj = instance.GetComponent<NetworkObject>();
        var net = instance.GetComponent<GameNet>();
        net.GameMode.Value = mode;
        net.GameState.Value = GameState.Waiting;
        manager.ServerManager.Spawn(netObj);
    }

    private void SpawnRole(NetworkConnection conn) {
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
}