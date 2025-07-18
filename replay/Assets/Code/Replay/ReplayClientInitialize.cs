using System.Collections.Generic;
using System.IO;
using FishNet.Serializing;

namespace KillCam.Client.Replay
{
    public class ReplayIInitialize : InitializeFeature, IReplayPlayer
    {
        private bool isPrepareFeatures;
        private byte[] playData;
        private Replay_StreamParse streamParse;
        private Replay_SpawnProvider spawnProvider;
        
        public override void OnCreate()
        {
            ClientWorldsChannel.SetReplayPlayer(this);
        }

        public override void OnDestroy()
        {
            ClientWorldsChannel.SetReplayPlayer(null);
        }

        public void SetData(byte[] data)
        {
            playData = data;
        }

        public void Play()
        {
            var result = new SortedList<uint, byte[]>();
            using var ms = new MemoryStream(playData);
            using var br = new BinaryReader(ms);
            int count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                uint tick = br.ReadUInt32();
                int length = br.ReadInt32();
                byte[] snapshotBytes = br.ReadBytes(length);
                result.Add(tick, snapshotBytes);
            }
            
            var playStates = new SortedList<uint, S2C_Replay_WorldStateSnapshot>();
            foreach (var kvp in result)
            {
                uint tick = kvp.Key;
                byte[] data = kvp.Value;

                var reader = new Reader(data, null);
                var worldState = new S2C_Replay_WorldStateSnapshot();
                worldState.Deserialize(reader);
                playStates[tick] = worldState;
            }
            
            AddFeatures();
            streamParse.StartHandleStream(playStates);
        }

        public void Pause()
        {
            
        }

        public void Resume()
        {
            
        }

        public void Stop()
        {
            
        }

        private void AddFeatures()
        {
            if (isPrepareFeatures)
            {
                return;
            }
            
            world.Add(streamParse = new Replay_StreamParse());
            world.Add(spawnProvider = new Replay_SpawnProvider());
            world.Add<Replay_InputProvider>();
            world.Add(new Client_RoleManager(spawnProvider));
        }
    }
}