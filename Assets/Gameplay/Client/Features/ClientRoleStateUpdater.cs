public class ClientRoleStateUpdater : Feature, IUpdateable {
    private ClientRoleRegistry registry;
    
    public override void OnInitialize(ref WorldLink link) {
        registry = link.RequireFeature<ClientRoleRegistry>();
    }

    public void OnUpdate() {
        var remoteStates = registry.GameIdToRoleNets;
        foreach (var remote in remoteStates.Values) {
            bool localStateExist = registry.GameIdToLocalStates.TryGetValue(remote.GameId.Value, out var local);
            if (!localStateExist) {
                continue;
            }
            
            local.Pos = remote.Pos.Value;
            local.Rot = remote.Rot.Value;
        }
    }
}