using System.Collections.Generic;

public class ServerRoleRegistry : Feature {
    public Dictionary<int, ServerRoleState> GameIdToRoleStates = new Dictionary<int, ServerRoleState>();
    public Dictionary<int, RoleNet> GameIdToRoleNets = new Dictionary<int, RoleNet>();
    private WorldEvents worldEvents;
    
    public override void OnInitialize(ref WorldLink link) {
        worldEvents = link.RequireFeature<WorldEvents>();
        worldEvents.Register<WorldEventDefine.RoleNetSpawn>(OnRoleNetSpawn);
    }

    public override void OnClear() {
        worldEvents.Unregister<WorldEventDefine.RoleNetSpawn>(OnRoleNetSpawn);
    }

    private void OnRoleNetSpawn(WorldEventDefine.RoleNetSpawn ent) {
        var id = ent.RoleNet.GameId.Value;
        if (ent.IsSpawn) {
            var newServerState = new ServerRoleState();
            newServerState.Init(id);
            GameIdToRoleStates.Add(id, newServerState);
            GameIdToRoleNets.Add(id, ent.RoleNet);
        }else {
            GameIdToRoleStates.Remove(id);
            GameIdToRoleNets.Remove(id);
        }
    }
}