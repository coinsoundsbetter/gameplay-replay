using System.Collections.Generic;
using FishNet.Managing;

namespace KillCam.Client
{
    public class Client_BaseFeature_Spawn : Feature
    {
        private NetworkManager _manager;
        public readonly Dictionary<int, RoleNet> roleStateInfo = new();
        
        public Client_BaseFeature_Spawn(NetworkManager manager)
        {
            _manager = manager;
        }

        public override void OnCreate()
        {
            RoleNet.OnClientSpawn += OnRoleSpawn;
            RoleNet.OnClientDespawn += OnRoleDespawn;
        }

        public override void OnDestroy()
        {
            RoleNet.OnClientSpawn -= OnRoleSpawn;
            RoleNet.OnClientDespawn -= OnRoleDespawn;
        }

        private void OnRoleSpawn(RoleNet state)
        {
            roleStateInfo.Add(state.Id.Value, state);
        }
        
        private void OnRoleDespawn(RoleNet state)
        {
            roleStateInfo.Remove(state.Id.Value);
        }
    }
}