using System.Collections.Generic;

namespace KillCam.Client.Replay {
    public class Replay_StreamParse : Feature {
        private bool isNeedHandle;
        private List<S2C_WorldStateSnapshot> playStreams = new();
        private uint playTick;

        public void StartHandleStream(List<S2C_WorldStateSnapshot> streams) {
            playStreams = streams;
        }

        protected override void OnTick() {
            playTick++;

            ref var worldTime = ref GetWorldDataRef<NetworkTime>();
            worldTime.Tick = playTick;
            
            bool isPlayNextState = false;
            S2C_WorldStateSnapshot nextPlayState = default;
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

        private void OnPlayState(S2C_WorldStateSnapshot state) {
            // 确保角色生成
            var spawnMgr = GetSingletonFeature<Replay_SpawnProvider>();
            spawnMgr.EnsureSpawn(state);

            // 纠正状态
            var stateMgr = GetSingletonFeature<Replay_StateProvider>();
            stateMgr.SetState(state);

            // 重放输入数据
            var inputMgr = GetSingletonFeature<Replay_InputProvider>();
            inputMgr.SetInput(state);

            // 用完了清理非托管资源
            state.HeroSnapshot.Dispose();
        }

        public new uint GetTick() {
            return playTick;
        }
    }
}