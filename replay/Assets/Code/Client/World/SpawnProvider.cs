using System;

namespace KillCam.Client
{
    public class SpawnProvider : Feature, IRoleSpawnProvider
    {
        public event Action<IClientRoleNet> OnRoleSpawn;
        public event Action<IClientRoleNet> OnRoleDespawn;

        public override void OnCreate()
        {
            RoleNet.OnClientSpawn += OnClientSpawn;
            RoleNet.OnClientDespawn += OnClientDespawn;
        }

        public override void OnDestroy()
        {
            RoleNet.OnClientSpawn -= OnClientSpawn;
            RoleNet.OnClientDespawn -= OnClientDespawn;
        }

        private void OnClientSpawn(IClientRoleNet net)
        {
            OnRoleSpawn?.Invoke(net);
        }
        
        private void OnClientDespawn(IClientRoleNet net)
        {
            OnRoleDespawn?.Invoke(net);
        }
    }
}