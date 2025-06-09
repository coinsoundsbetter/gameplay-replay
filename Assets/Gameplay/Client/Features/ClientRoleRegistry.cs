using System.Collections.Generic;

public class ClientRoleRegistry : Feature {
    public readonly Dictionary<int, ClientRoleState> GameIdToLocalStates = new();
    private WorldEvents worldEvents;

    public override void OnInitialize(ref WorldLink link) {
        worldEvents = link.RequireFeature<WorldEvents>();
        worldEvents.Register<ClientWorldEvents.RoleSpawn>(OnRoleSpawn);
        worldEvents.Register<ClientWorldEvents.RoleDespawn>(OnRoleDespawn);
    }

    public override void OnClear() {
        worldEvents.Unregister<ClientWorldEvents.RoleSpawn>(OnRoleSpawn);
        worldEvents.Unregister<ClientWorldEvents.RoleDespawn>(OnRoleDespawn);
    }
    
    private void OnRoleSpawn(ClientWorldEvents.RoleSpawn ent) {
        
    }
    
    private void OnRoleDespawn(ClientWorldEvents.RoleDespawn ent) {
        
    }
}