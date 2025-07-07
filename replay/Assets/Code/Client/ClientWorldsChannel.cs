using System;

namespace KillCam.Client
{
    public class ClientWorldsChannel
    {
        private static ClientWorldsChannel Instance;
        public event Action<string> startReplayByFile;

        public static void Create()
        {
            Instance = new ClientWorldsChannel();
        }

        public static void Destroy()
        {
            Instance = null;
        }
    }
}