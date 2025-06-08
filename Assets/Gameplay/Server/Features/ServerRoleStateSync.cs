public class ServerRoleStateSync : Feature, IUpdateable {
    private ServerRoleRegistry registry;
    
    public override void OnInitialize(ref WorldLink link) {
        registry = link.RequireFeature<ServerRoleRegistry>();
    }

    public void OnUpdate() {
        var serverRoleStates = registry.GameIdToRoleStates;
        foreach (var serverRoleState in serverRoleStates.Values) {
            bool isNetExist = registry.GameIdToRoleNets.TryGetValue(serverRoleState.GameId, out var net);
            if (!isNetExist) {
                continue;
            }

            net.Pos.Value = serverRoleState.Pos;
            net.Rot.Value = serverRoleState.Rot;
        }
    }
}