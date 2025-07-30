using System;
using System.Collections.Generic;

namespace KillCam.Client.Replay {
    public class Replay_SpawnProvider : Feature, IRoleSpawnProvider {
        public event Action<IClientCharacterNet> OnRoleSpawn;
        public event Action<IClientCharacterNet> OnRoleDespawn;
        private readonly HashSet<int> spawnRoleIds = new();

        public void EnsureSpawn(S2C_WorldStateSnapshot snapshot) {
            if (snapshot.CharacterSnapshot.IsEmpty()) {
                return;
            }

            foreach (var kvp in snapshot.CharacterSnapshot.StateData) {
                var id = kvp.Key;
                var character = kvp.Value;
                if (spawnRoleIds.Add(id)) {
                    OnRoleSpawn?.Invoke(new ReplayCharacterNet() {
                        Id = id,
                        Data = character,
                    });
                }
            }
        }
    }
}