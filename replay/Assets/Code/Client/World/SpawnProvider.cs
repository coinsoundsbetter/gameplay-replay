using System;

namespace KillCam.Client {
    public class SpawnProvider : Feature, IRoleSpawnProvider {
        public event Action<IClientCharacterNet> OnRoleSpawn;
        public event Action<IClientCharacterNet> OnRoleDespawn;

        public override void OnCreate() {
            CharacterNet.OnClientSpawn += OnClientSpawn;
            CharacterNet.OnClientDespawn += OnClientDespawn;
        }

        public override void OnDestroy() {
            CharacterNet.OnClientSpawn -= OnClientSpawn;
            CharacterNet.OnClientDespawn -= OnClientDespawn;
        }

        private void OnClientSpawn(IClientCharacterNet net) {
            OnRoleSpawn?.Invoke(net);
        }

        private void OnClientDespawn(IClientCharacterNet net) {
            OnRoleDespawn?.Invoke(net);
        }
    }
}