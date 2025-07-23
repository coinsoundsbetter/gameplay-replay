using System;

namespace KillCam.Client
{
    public interface IRoleSpawnProvider
    {
        event Action<IClientRoleNet> OnRoleSpawn;
        event Action<IClientRoleNet> OnRoleDespawn;
    }
}