using System;

namespace KillCam.Client {
    public interface IRoleSpawnProvider {
        event Action<IClientHeroNet> OnRoleSpawn;
        event Action<IClientHeroNet> OnRoleDespawn;
    }
}