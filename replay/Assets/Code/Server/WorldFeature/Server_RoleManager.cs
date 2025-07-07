using System.Collections.Generic;

namespace KillCam.Server
{
    public class Server_RoleManager : Feature
    {
        private readonly Dictionary<int, IServerRoleNet> roleNets = new();
        private readonly Dictionary<int, Server_RoleLogic> roleLogics = new();
        public IReadOnlyDictionary<int, IServerRoleNet> RoleNets => roleNets;
        public IReadOnlyDictionary<int, Server_RoleLogic> RoleLogics => roleLogics;

        public override void OnCreate()
        {
            RoleNet.OnServerSpawn += OnRoleNetSpawn;
            RoleNet.OnServerDespawn += OnRoleNetDespawn;
            world.AddFrameUpdate(OnFrameTick);
            world.AddLogicUpdate(OnLogicTick);
        }

        public override void OnDestroy()
        {
            RoleNet.OnServerSpawn -= OnRoleNetSpawn;
            RoleNet.OnServerDespawn -= OnRoleNetDespawn;
            world.RemoveFrameUpdate(OnFrameTick);
            world.RemoveLogicUpdate(OnLogicTick);
        }
        
        private void OnFrameTick(float delta)
        {
            foreach (var role in roleLogics.Values)
            {
                role.TickFrame(delta);
            }
        }
        
        private void OnLogicTick(double delta)
        {
            foreach (var role in roleLogics.Values)
            {
                role.TickLogic(delta);
            }

            foreach (var state in roleNets.Values)
            {
                if (roleLogics.TryGetValue(state.GetId(), out var logic))
                {
                    state.SetServerSyncData(logic.GetNetStateData());
                }
            }
        }

        private void OnRoleNetSpawn(IServerRoleNet net)
        {
            var id = net.GetId();
            var data = net.GetData();
            roleNets.Add(id, net);

            var newLogic = new Server_RoleLogic();
            if (roleLogics.TryAdd(id, newLogic))
            {
                newLogic.Init(world);
            }
        }
        
        private void OnRoleNetDespawn(IServerRoleNet net)
        {
            var id = net.GetId();
            if (roleLogics.Remove(id, out var logic))
            {
                logic.Clear();
            }

            roleNets.Remove(id);
        }

        public bool TryGetRole(int playerId, out Server_RoleLogic roleLogic)
        {
            if (roleLogics.TryGetValue(playerId, out roleLogic))
            {
                return true;
            }

            return false;
        }
    }
}