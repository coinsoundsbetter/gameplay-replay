using System.Collections.Generic;

namespace KillCam.Client.Replay {
    public class Replay_StreamParse : Feature, INetwork {
        private bool isNeedHandle;
        private List<S2C_Replay_WorldStateSnapshot> playStreams = new();
        private uint playTick;

        public void StartHandleStream(List<S2C_Replay_WorldStateSnapshot> streams) {
            playStreams = streams;
        }

        public override void OnCreate() {
            world.SetNetwork(this);
            world.AddLogicUpdate(OnLogicTick);
        }

        public override void OnDestroy() {
            world.RemoveNetwork(this);
            world.RemoveLogicUpdate(OnLogicTick);
        }

        private void OnLogicTick(double delta) {
            playTick++;

            bool isPlayNextState = false;
            S2C_Replay_WorldStateSnapshot nextPlayState = default;
            if (playStreams.Count > 0) {
                nextPlayState = playStreams[0];
            }

            if (!nextPlayState.IsNull() && playTick == nextPlayState.Tick) {
                OnPlayState(nextPlayState);
                isPlayNextState = true;
            }

            if (isPlayNextState) {
                playStreams.RemoveAt(0);
            }
        }

        private void OnPlayState(S2C_Replay_WorldStateSnapshot state) {
            // 确保角色生成
            var spawnMgr = world.Get<Replay_SpawnProvider>();
            spawnMgr.EnsureSpawn(state);

            // 纠正状态
            var stateMgr = world.Get<Replay_StateProvider>();
            stateMgr.SetState(state);

            // 重放输入数据
            var inputMgr = world.Get<Replay_InputProvider>();
            inputMgr.SetInput(state);

            // 用完了清理非托管资源
            state.CharacterSnapshot.Dispose();
        }

        public void Send(INetworkSerialize data) {
        }

        public void Rpc(INetworkSerialize data) {
        }

        public new uint GetTick() {
            return playTick;
        }
    }
}