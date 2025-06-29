using System.Collections.Generic;

namespace KillCam.Client
{
    public class Client_BaseFeature_StateDriver : Feature, INetworkClient
    {
        private RoleNet localRoleNet;
        private readonly Dictionary<int, RoleNet> _roleNets = new();
        private readonly Dictionary<int, Client_RoleLogic> _roleLogics = new();
        
        public override void OnCreate()
        {
            world.SetNetworkClient(this);
            RoleNet.OnClientSpawn += OnRoleNetSpawn;
            RoleNet.OnClientDespawn += OnRoleDespawn;
            world.AddUpdate(Update);
        }

        public override void OnDestroy()
        {
            world.SetNetworkClient(null);
            RoleNet.OnClientSpawn -= OnRoleNetSpawn;
            world.RemoveUpdate(Update);
        }

        private void Update(float delta)
        {
            foreach (var roleLogic in _roleLogics.Values)
            {
                roleLogic.Update();
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
    }
}