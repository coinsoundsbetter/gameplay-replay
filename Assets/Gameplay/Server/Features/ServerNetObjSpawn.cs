using System.Collections.Generic;

public class ServerNetObjSpawn : Feature {
    public Dictionary<int, RoleNet> RoleNets = new Dictionary<int, RoleNet>();
    public GameNet MyGameNet { get; private set; }
    private WorldEvents worldEvents;
    
    public override void OnInitialize(ref WorldLink link) {
        worldEvents = link.RequireFeature<WorldEvents>();
        worldEvents.Register<WorldEventDefine.NetObjSpawn>(OnNetObjSpawn);
    }

    public override void OnClear() {
        worldEvents.Unregister<WorldEventDefine.NetObjSpawn>(OnNetObjSpawn);
    }
    
    private void OnNetObjSpawn(WorldEventDefine.NetObjSpawn ent) {
        NetworkObj obj = ent.Obj;
        if (obj is RoleNet roleNet) {
            OnRoleNetSpawn(ent.IsSpawn, roleNet);
        }else if (obj is GameNet gameNet) {
            OnGameNetSpawn(ent.IsSpawn, gameNet);
        }
    }

    private void OnRoleNetSpawn(bool isSpawn, RoleNet net) {
        if (isSpawn) {
            RoleNets.Add(net.RoleId.Value, net);
            worldEvents.Publish(new ClientWorldEvents.RoleSpawn() {
                RoleId = net.RoleId.Value,
                Pos = net.Pos.Value,
                Rot = net.Rot.Value
            });
        }else {
            RoleNets.Remove(net.RoleId.Value);    
            worldEvents.Publish(new ClientWorldEvents.RoleDespawn() {
                RoleId = net.RoleId.Value,
            });
        }
    }
    
    private void OnGameNetSpawn(bool isSpawn, GameNet net) {
        if (isSpawn) {
            MyGameNet = net;
            worldEvents.Publish(new ClientWorldEvents.GameSpawn() {
                GameState = net.GameState.Value,
            });
        }else {
            worldEvents.Publish(new ClientWorldEvents.GameDespawn());
            MyGameNet = null;
        }
    }
}