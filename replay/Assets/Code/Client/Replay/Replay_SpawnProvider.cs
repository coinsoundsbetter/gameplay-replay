using System;
using System.Collections.Generic;

namespace KillCam.Client.Replay {
    public class Replay_SpawnProvider : Feature, IRoleSpawnProvider {
        public event Action<IClientHeroNet> OnRoleSpawn;
        public event Action<IClientHeroNet> OnRoleDespawn;
        private readonly HashSet<int> spawnRoleIds = new();

        public void EnsureSpawn(S2C_WorldStateSnapshot snapshot) {
            if (snapshot.HeroSnapshot.IsEmpty()) {
                return;
            }

            foreach (var kvp in snapshot.HeroSnapshot.StateData) {
                var id = kvp.Key;
                var character = kvp.Value;
                if (spawnRoleIds.Add(id)) {
                    OnRoleSpawn?.Invoke(new ReplayHeroNet() {
                        Id = id,
                        Data = character,
                    });
                }
            }
        }
    }
}