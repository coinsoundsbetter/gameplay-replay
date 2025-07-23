using KillCam.Client.Replay;

namespace KillCam.Client
{
    public class ClientWorldsChannel
    {
        private static ClientWorldsChannel Instance;
        private IReplayPlayer replayPlayer;

        public static void Create()
        {
            Instance = new ClientWorldsChannel();
        }

        public static void Destroy()
        {
            Instance = null;
        }

        public static void SetReplayPlayer(IReplayPlayer player)
        {
            if (Instance != null)
            {
                Instance.replayPlayer = player;
            }
        }

        public static void StartReplay(byte[] data)
        {
            if (Instance != null)
            {
                Instance.replayPlayer.SetData(data);
                Instance.replayPlayer.Play();
            }
        }
    }
}