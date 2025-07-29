using System;
using System.Collections.Generic;

namespace KillCam.Client.Replay {
    public class Replay_SpawnProvider : Feature, IRoleSpawnProvider {
        public event Action<IClientRoleNet> OnRoleSpawn;
        public event Action<IClientRoleNet> OnRoleDespawn;
        private readonly HashSet<int> spawnRoleIds = new();

        public void EnsureSpawn(S2C_Replay_WorldStateSnapshot snapshot) {
            if (snapshot.CharacterSnapshot.IsEmpty()) {
                return;
            }

            foreach (var kvp in snapshot.CharacterSnapshot.StateData) {
                var id = kvp.Key;
                var character = kvp.Value;
                if (spawnRoleIds.Add(id)) {
                    OnRoleSpawn?.Invoke(new ReplayRoleNet() {
                        Id = id,
                        Data = character,
                    });
                }
            }
        }
    }
}