using System.Collections.Generic;
using System.IO;
using FishNet.Managing;
using FishNet.Serializing;

namespace KillCam.Client.Replay {
    [UnityEngine.Scripting.Preserve]
    public class ReplayIEntry : ReplayEntryBase, IReplayPlayer {
        private bool isPrepareFeatures;
        private byte[] playData;
        private Replay_StreamParse streamParse;
        private Replay_SpawnProvider spawnProvider;

        protected override void OnBeforeStart() {
            ClientWorldsChannel.SetReplayPlayer(this);
        }

        protected override void OnAfterDestroy() {
            ClientWorldsChannel.SetReplayPlayer(null);
        }

        public void SetData(byte[] data) {
            playData = data;
        }

        public void Play() {
            var worldStreams = new List<S2C_WorldStateSnapshot>();
            using (var ms = new MemoryStream(playData))
            using (var br = new BinaryReader(ms)) {
                int count = br.ReadInt32();
                for (int i = 0; i < count; i++) {
                    int len = br.ReadInt32();
                    byte[] data = br.ReadBytes(len);
                    var snapshot = new S2C_WorldStateSnapshot();
                    snapshot.Deserialize(new Reader(data, null));
                    worldStreams.Add(snapshot);
                }
            }

            AddFeatures();
            streamParse.StartHandleStream(worldStreams);
        }

        public void Pause() {
        }

        public void Resume() {
        }

        public void Stop() {
        }

        private void AddFeatures() {
            if (isPrepareFeatures) {
                return;
            }
            
            world.SetupData(new WorldTime());
            world.SetupFeature(streamParse = new Replay_StreamParse(), TickGroup.InitializeLogic);
            world.SetupFeature(spawnProvider = new Replay_SpawnProvider(), TickGroup.InitializeLogic);
            world.SetupFeature<Replay_StateProvider>(TickGroup.InitializeLogic);
            world.SetupFeature<Replay_InputProvider>(TickGroup.InitializeLogic);
            world.SetupFeature(new HeroManager(spawnProvider), TickGroup.InitializeLogic);
            world.SetupFeature<CameraManager>(TickGroup.CameraFrame);
        }
    }
}