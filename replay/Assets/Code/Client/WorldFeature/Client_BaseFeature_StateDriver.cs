using System.Collections.Generic;
using FishNet.Managing;

namespace KillCam.Client
{
    public class Client_BaseFeature_StateDriver : Feature, INetwork
    {
        private NetworkManager mgr;
        private RoleNet localRoleNet;
        private readonly Dictionary<int, RoleNet> _roleNets = new();
        private readonly Dictionary<int, Client_RoleLogic> _roleLogics = new();

        public Client_BaseFeature_StateDriver(NetworkManager manager)
        {
            mgr = manager;
        }

        public override void OnCreate()
        {
            world.SetNetwork(this);
            RoleNet.OnClientSpawn += OnRoleNetSpawn;
            RoleNet.OnClientDespawn += OnRoleDespawn;
            world.AddFrameUpdate(OnFrameUpdate);
            world.AddLogicUpdate(OnLogicUpdate);
        }

        public override void OnDestroy()
        {
            world.SetNetwork(null);
            RoleNet.OnClientSpawn -= OnRoleNetSpawn;
            world.RemoveFrameUpdate(OnFrameUpdate);
            world.RemoveLogicUpdate(OnLogicUpdate);
        }

        private void OnFrameUpdate(float delta)
        {
            foreach (var roleLogic in _roleLogics.Values)
            {
                roleLogic.TickFrame(delta);
            }
        }
        
        private void OnLogicUpdate(double delta)
        {
            foreach (var roleLogic in _roleLogics.Values)
            {
                roleLogic.TickLogic(delta);
            }
        }
        
        private void OnRoleNetSpawn(RoleNet net)
        {
            var logic = new Client_RoleLogic();
            logic.Init(world);
            _roleLogics.Add(net.Id.Value, logic);
            if (net.IsOwner)
            {
                localRoleNet = net;
            }
            _roleNets.Add(net.Id.Value, net);
        }
        
        private void OnRoleDespawn(RoleNet net)
        {
            var id = net.Id.Value;
            if (_roleLogics.Remove(id, out var logic))
            {
                logic.Clear();
            }
            _roleNets.Remove(id);
            if (net.IsOwner)
            {
                localRoleNet = null;
            }
        }

        public void Send(INetworkSerialize data)
        {
            if (localRoleNet)
            {
                localRoleNet.Send(data);
            }
        }

        public void Rpc(INetworkSerialize data) { }

        public new uint GetTick()
        {
            return mgr.TimeManager.LocalTick;
        }
    }
}