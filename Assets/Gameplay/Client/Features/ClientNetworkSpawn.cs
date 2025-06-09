using System.Collections.Generic;

public class ClientNetworkSpawn : Feature {
    private WorldEvents worldEvents;
    public Dictionary<int, RoleNet> RoleNets = new Dictionary<int, RoleNet>();
    
    public override void OnInitialize(ref WorldLink link) {
        worldEvents = link.RequireFeature<WorldEvents>();
        worldEvents.Register<WorldEventDefine.RoleNetSpawn>(OnRoleNetSpawn);
    }

    public override void OnClear() {
        worldEvents.Unregister<WorldEventDefine.RoleNetSpawn>(OnRoleNetSpawn);
    }

    private void OnRoleNetSpawn(WorldEventDefine.RoleNetSpawn ent) {
        /*if (ent.IsSpawn) {
            RoleNets.Add(ent.RoleNet.RoleId.Value, ent.RoleNet);
            worldEvents.Publish(new ClientWorldEvents.RoleSpawn() {
                RoleId = ent.RoleNet.RoleId.Value,
                Pos = ent.RoleNet.Pos.Value,
                Rot = ent.RoleNet.Rot.Value
            });
        }else {
            RoleNets.Remove(ent.RoleNet.RoleId.Value);    
            worldEvents.Publish(new ClientWorldEvents.RoleDespawn() {
                RoleId = ent.RoleNet.RoleId.Value,
            });
        }*/
    }
}