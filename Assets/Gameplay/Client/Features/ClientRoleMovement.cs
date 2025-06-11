
public class ClientRoleMovement : RoleMovement, IUpdateable {
    
    public override void OnInitialize(ref WorldLink link) {
        
    }

    public void OnUpdate() {
        /*var states = registry.GameIdToLocalStates;
        foreach (var state in states.Values) {
            bool isRemoteStateExist = registry.GameIdToRoleNets.TryGetValue(state.RoleId, out var remote);
            if (!isRemoteStateExist) {
                continue;
            }

            state.Pos = remote.Pos.Value;
            state.Rot = remote.Rot.Value;
            state.SetPos(state.Pos);
        }*/
    }
}