using System.Collections.Generic;

namespace KillCam.Client.Replay
{
    public class Replay_StreamParse : Feature, INetwork
    {
        private bool isNeedHandle;
        private SortedList<uint, S2C_Replay_WorldStateSnapshot> playStreams = new();
        private uint playTick;

        public void StartHandleStream(SortedList<uint, S2C_Replay_WorldStateSnapshot> streams)
        {
            playStreams = streams;
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

            bool isPlayNextState = false;
            S2C_Replay_WorldStateSnapshot nextPlayState = default;
            if (playStreams.Count > 0)
            {
                nextPlayState = playStreams.Values[0];
            }

            if (!nextPlayState.IsNull() && playTick == nextPlayState.Tick)
            {
                OnPlayState(nextPlayState);
                isPlayNextState = true;
            }

            if (isPlayNextState)
            {
                playStreams.Remove(playTick);
            }
        }

        private void OnPlayState(S2C_Replay_WorldStateSnapshot state)
        {
            // 确保角色生成
            var spawnMgr = world.Get<Replay_SpawnProvider>();
            spawnMgr.EnsureSpawn(state);
            
            // 重放输入数据
            var inputMgr = world.Get<Replay_InputProvider>();
            inputMgr.SetInput(state);
        }

        public void Send(INetworkSerialize data)
        {
        }

        public void Rpc(INetworkSerialize data)
        {
        }

        public new uint GetTick()
        {
            return playTick;
        }
    }
}