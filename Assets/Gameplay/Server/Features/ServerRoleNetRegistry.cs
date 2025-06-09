using System.Collections.Generic;

public class ServerRoleNetRegistry : Feature {
    public Dictionary<int, RoleNet> RoleNets = new Dictionary<int, RoleNet>();
    private WorldEvents worldEvents;
    
    public override void OnInitialize(ref WorldLink link) {
        worldEvents = link.RequireFeature<WorldEvents>();
        worldEvents.Register<WorldEventDefine.RoleNetSpawn>(OnRoleSpawn);
    }

    public override void OnClear() {
        worldEvents.Unregister<WorldEventDefine.RoleNetSpawn>(OnRoleSpawn);
    }

    private void OnRoleSpawn(WorldEventDefine.RoleNetSpawn ent) {
        /*if (ent.IsSpawn) {
            RoleNets.Add(ent.RoleNet.RoleId.Value, ent.RoleNet);
            worldEvents.Publish(new ServerWorldEvents.RoleSpawn() {
                RoleId = ent.RoleNet.RoleId.Value,
                DefaultPos = ent.RoleNet.Pos.Value,
                DefaultRot = ent.RoleNet.Rot.Value,
            });
        }else {
            RoleNets.Remove(ent.RoleNet.RoleId.Value);  
            worldEvents.Publish(new ServerWorldEvents.RoleDespawn() {
                RoleId = ent.RoleNet.RoleId.Value,
            });
        }*/
    }
}