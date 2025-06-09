using System.Collections.Generic;

public class ServerRoleRegistry : Feature {
    public Dictionary<int, ServerRoleState> GameIdToRoleStates = new Dictionary<int, ServerRoleState>();
    private WorldEvents worldEvents;
    
    public override void OnInitialize(ref WorldLink link) {
        worldEvents = link.RequireFeature<WorldEvents>();
        worldEvents.Register<ServerWorldEvents.RoleSpawn>(OnRoleSpawn);
    }

    public override void OnClear() {
        worldEvents.Unregister<ServerWorldEvents.RoleSpawn>(OnRoleSpawn);
    }
    
    private void OnRoleSpawn(ServerWorldEvents.RoleSpawn ent) {
        var newState = new ServerRoleState();
        GameIdToRoleStates.Add(ent.RoleId, newState);
        newState.RoleId = ent.RoleId;
        newState.Pos = ent.DefaultPos;
        newState.Rot = ent.DefaultRot;
    }
}