using System;
using System.Collections.Generic;

namespace KillCam.Client.Replay
{
    public class Replay_StreamParse : Feature, IRoleSpawnProvider, INetwork
    {
        public event Action<IClientRoleNet> OnRoleSpawn;
        public event Action<IClientRoleNet> OnRoleDespawn;
        
        private Queue<(uint, Dictionary<int, RoleStateSnapshot>)> playQueue;
        private bool isNeedHandle;
        private (uint, Dictionary<int, RoleStateSnapshot>) waitHandle;
        private uint playTick;

        public void StartHandleStream(Dictionary<uint, Dictionary<int, RoleStateSnapshot>> data)
        {
            playQueue = new Queue<(uint, Dictionary<int, RoleStateSnapshot>)>();
            foreach (var kvp in data)
            {
                playQueue.Enqueue((kvp.Key, kvp.Value));
            }
        }

        public override void OnCreate()
        {
            world.SetNetwork(this);
            world.AddLogicUpdate(OnLogicTick);
        }

        public override void OnDestroy()
        {
            world.RemoveNetwork(this);
            world.RemoveLogicUpdate(OnLogicTick);
        }

        private void OnLogicTick(double delta)
        {
            playTick++;

            if (playQueue.Count > 0 && waitHandle.Item2 == null || waitHandle.Item2.Count == 0)
            {
                waitHandle = playQueue.Dequeue();
            }
            
            // 检查是否达到要处理的tick
            if (waitHandle.Item2 != null && waitHandle.Item2.Count > 0 && playTick == waitHandle.Item1)
            {
                OnHandleState(waitHandle.Item2);
                waitHandle.Item2 = null;
            }
        }

        private HashSet<int> spawnRoleIds = new HashSet<int>();
        private void OnHandleState(Dictionary<int, RoleStateSnapshot> data)
        {
            foreach (var kvp in data)
            {
                if (!spawnRoleIds.Contains(kvp.Key))
                {
                    spawnRoleIds.Add(kvp.Key);
                    OnRoleSpawn?.Invoke(new ReplayRoleNet()
                    {
                        Id = kvp.Key,
                        Data = kvp.Value,
                    });
                }
            }
        }

        public void Send(INetworkSerialize data) { }

        public void Rpc(INetworkSerialize data) { }

        public new uint GetTick()
        {
            return playTick;
        }
    }
}