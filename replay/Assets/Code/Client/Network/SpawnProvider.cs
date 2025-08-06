using System;

namespace KillCam.Client {
    public class SpawnProvider : Feature, IRoleSpawnProvider {
        public event Action<IClientHeroNet> OnRoleSpawn;
        public event Action<IClientHeroNet> OnRoleDespawn;

        protected override void OnSetup() {
            HeroNet.OnClientSpawn += OnClientSpawn;
            HeroNet.OnClientDespawn += OnClientDespawn;
        }

        protected override void OnDestroy() {
            HeroNet.OnClientSpawn -= OnClientSpawn;
            HeroNet.OnClientDespawn -= OnClientDespawn;
        }

        private void OnClientSpawn(IClientHeroNet net) {
            OnRoleSpawn?.Invoke(net);
        }

        private void OnClientDespawn(IClientHeroNet net) {
            OnRoleDespawn?.Invoke(net);
        }
    }
}