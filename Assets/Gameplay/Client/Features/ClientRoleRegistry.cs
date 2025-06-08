using System.Collections.Generic;

public class ClientRoleRegistry : Feature {
    public readonly Dictionary<int, ClientRoleState> GameIdToLocalStates = new();
    public readonly Dictionary<int, RoleNet> GameIdToRoleNets = new();
    public ClientRoleState LocalState;
    public RoleNet LocalNet;
    public bool IsLocalInitialized => LocalState != null && LocalNet != null;
    private WorldEvents worldEvents;

    public override void OnInitialize(ref WorldLink link) {
        worldEvents = link.RequireFeature<WorldEvents>();
        worldEvents.Register<WorldEventDefine.RoleNetSpawn>(MsgRoleNetSpawn);
    }

    public override void OnClear() {
        worldEvents.Unregister<WorldEventDefine.RoleNetSpawn>(MsgRoleNetSpawn);
    }

    private void MsgRoleNetSpawn(WorldEventDefine.RoleNetSpawn ent) {
        var gameId = ent.RoleNet.GameId.Value;
        
        // 远端状态
        if (ent.IsSpawn) {
            GameIdToRoleNets.Add(gameId, ent.RoleNet);
        }else {
            GameIdToRoleNets.Remove(gameId);
        }
        
        // 本地状态
        if (ent.IsSpawn) {
            var roleControl = new ClientRoleState();
            roleControl.Init(gameId);
            GameIdToLocalStates.Add(gameId, roleControl);
            if (ent.RoleNet.IsOwner) {
                LocalState = roleControl;
                LocalNet = ent.RoleNet;
            }
        }else {
            if (GameIdToLocalStates.Remove(gameId, out var remove)) {
                remove.Clear();
            }
            if (ent.RoleNet.IsOwner) {
                LocalState = null;
                LocalNet = null;
            }
        }
    }
}