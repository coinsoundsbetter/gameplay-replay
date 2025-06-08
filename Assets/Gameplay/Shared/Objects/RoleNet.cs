using FishNet.Object.Synchronizing;
using UnityEngine;

public class RoleNet : NetworkObj {
    public readonly SyncVar<int> GameId = new (new SyncTypeSettings(WritePermission.ServerOnly));
    public readonly SyncVar<string> GameName = new (new SyncTypeSettings(WritePermission.ServerOnly));
    public readonly SyncVar<Vector3> Pos = new (new SyncTypeSettings(WritePermission.ServerOnly));
    public readonly SyncVar<Quaternion> Rot = new (new SyncTypeSettings(WritePermission.ServerOnly));
    public bool IsInReplayWorld { get; set; }

    public override void OnStartServer() {
        if (IsInReplayWorld) {
            WorldManager.Instance.GetWorld(WorldType.Replay)
                .GetFeature<WorldEvents>().Publish(new WorldEventDefine.RoleNetSpawn() {
                    IsSpawn = true,
                    RoleNet = this,
                });
        }else {
            WorldManager.Instance.GetWorld(WorldType.Server)
                .GetFeature<WorldEvents>().Publish(new WorldEventDefine.RoleNetSpawn() {
                    IsSpawn = true,
                    RoleNet = this,
                });
        }
    }

    public override void OnStopServer() {
        var worldMgr = WorldManager.Instance;
        if (worldMgr == null) {
            return;
        }
        
        if (IsInReplayWorld) {
            WorldManager.Instance.GetWorld(WorldType.Replay)
                .GetFeature<WorldEvents>().Publish(new WorldEventDefine.RoleNetSpawn() {
                    IsSpawn = false,
                    RoleNet = this,
                });
        }else {
            WorldManager.Instance.GetWorld(WorldType.Server)
                .GetFeature<WorldEvents>().Publish(new WorldEventDefine.RoleNetSpawn() {
                    IsSpawn = false,
                    RoleNet = this,
                });
        }
    }

    public override void OnStartClient() {
        WorldManager.Instance.GetWorld(WorldType.Client)
            .GetFeature<WorldEvents>().Publish(new WorldEventDefine.RoleNetSpawn() {
                IsSpawn = true,
                RoleNet = this,
            });
    }

    public override void OnStopClient() {
        var worldMgr = WorldManager.Instance;
        if (worldMgr == null) {
            return;
        }
        
        WorldManager.Instance.GetWorld(WorldType.Client)
            .GetFeature<WorldEvents>().Publish(new WorldEventDefine.RoleNetSpawn() {
                IsSpawn = false,
                RoleNet = this,
            });
    }
}