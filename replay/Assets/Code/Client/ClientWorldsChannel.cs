using System;

namespace KillCam.Client
{
    public class ClientWorldsChannel
    {
        private static ClientWorldsChannel Instance;

        public static void Create()
        {
            Instance = new ClientWorldsChannel();
        }

        public static void Destroy()
        {
            Instance = null;
        }

        public static void StartReplay(Action isFinish)
        {
            
        }

        public static void SetReplayData(byte[] data)
        {
            
        }
    }
}