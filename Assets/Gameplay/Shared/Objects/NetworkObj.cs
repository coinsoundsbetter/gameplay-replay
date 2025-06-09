using FishNet.Object;

public class NetworkObj : NetworkBehaviour {
    public override void OnStartServer() {
        var serverWorld = WorldManager.Instance.GetWorld(WorldType.Server);
        serverWorld.GetFeature<WorldEvents>().Publish(new WorldEventDefine.NetObjSpawn() {
            IsSpawn = true,
            Obj = this,
        });
    }

    public override void OnStopServer() {
        var worldMgr = WorldManager.Instance;
        if (worldMgr == null) {
            return;
        }
        
        var serverWorld = WorldManager.Instance.GetWorld(WorldType.Server);
        serverWorld.GetFeature<WorldEvents>().Publish(new WorldEventDefine.NetObjSpawn() {
            IsSpawn = false,
            Obj = this,
        });
    }

    public override void OnStartClient() {
        var clientWorld = WorldManager.Instance.GetWorld(WorldType.Client);
        clientWorld.GetFeature<WorldEvents>().Publish(new WorldEventDefine.NetObjSpawn() {
            IsSpawn = true,
            Obj = this,
        });
    }

    public override void OnStopClient() {
        var worldMgr = WorldManager.Instance;
        if (worldMgr == null) {
            return;
        }
        
        var clientWorld = WorldManager.Instance.GetWorld(WorldType.Client);
        clientWorld.GetFeature<WorldEvents>().Publish(new WorldEventDefine.NetObjSpawn() {
            IsSpawn = false,
            Obj = this,
        });
    }
}