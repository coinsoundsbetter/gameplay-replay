using System;
using System.Collections.Generic;
using UnityEngine;

namespace KillCam.Client.Replay {
    
    public class Replay_SpawnProvider : Feature, IRoleSpawnProvider{
        public event Action<IClientRoleNet> OnRoleSpawn;
        public event Action<IClientRoleNet> OnRoleDespawn;
        private readonly HashSet<int> spawnRoleIds = new();

        public void EnsureSpawn(S2C_Replay_WorldStateSnapshot snapshot)
        {
            if (snapshot.RoleStateSnapshots == null)
            {
                return;
            }

            foreach (var id in snapshot.RoleStateSnapshots.Keys)
            {
                if (spawnRoleIds.Add(id))
                {
                    Debug.Log("生成角色!");
                    OnRoleSpawn?.Invoke(new ReplayRoleNet()
                    {
                        Id = id,
                        Data = snapshot.RoleStateSnapshots[id],
                    });
                }
            }
        }
    }
}