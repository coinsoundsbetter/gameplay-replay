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
            var worldStreams = new List<S2C_Replay_WorldStateSnapshot>();
            using (var ms = new MemoryStream(playData))
            using (var br = new BinaryReader(ms))
            {
                int count = br.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    int len = br.ReadInt32();
                    byte[] data = br.ReadBytes(len);
                    var snapshot = new S2C_Replay_WorldStateSnapshot();
                    snapshot.Deserialize(new Reader(data, null));
                    worldStreams.Add(snapshot);
                }
            }    
            
            AddFeatures();
            streamParse.StartHandleStream(worldStreams);
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
            world.Add<ActorManager>();
            world.Add<Replay_InputProvider>();
            world.Add(new Client_RoleManager(spawnProvider));
        }
    }
}