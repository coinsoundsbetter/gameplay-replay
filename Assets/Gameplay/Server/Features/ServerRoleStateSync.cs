public class ServerRoleStateSync : Feature, IUpdateable {
    private ServerRoleNetRegistry roleNetRegistry;
    private ServerRoleRegistry roleRegistry;
    
    public override void OnInitialize(ref WorldLink link) {
        roleRegistry = link.RequireFeature<ServerRoleRegistry>();
        roleNetRegistry = link.RequireFeature<ServerRoleNetRegistry>();
    }

    public void OnUpdate() {
        foreach (var serverRole in roleRegistry.GameIdToRoleStates.Values) {
            bool isNetExist = roleNetRegistry.RoleNets.TryGetValue(serverRole.RoleId, out var net);
            if (!isNetExist) {
                continue;
            }

            net.Pos.Value = serverRole.Pos;
            net.Rot.Value = serverRole.Rot;
        }
    }
}