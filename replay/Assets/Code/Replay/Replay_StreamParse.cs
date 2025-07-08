using System;

namespace KillCam.Client.Replay
{
    public class Replay_StreamParse : Feature, IRoleSpawnProvider
    {
        public event Action<IClientRoleNet> OnRoleSpawn;
        public event Action<IClientRoleNet> OnRoleDespawn;

        // 在这里解析回放流数据,并转换成世界需要的内容
        public void OnHandleStream(byte[] data)
        {
            
        }
    }
}