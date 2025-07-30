using System;

namespace KillCam.Client {
    public interface IRoleSpawnProvider {
        event Action<IClientCharacterNet> OnRoleSpawn;
        event Action<IClientCharacterNet> OnRoleDespawn;
    }
}