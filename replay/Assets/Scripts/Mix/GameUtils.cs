using Unity.Entities;
using UnityEngine;

namespace KillCam {
    public class GameUtils
    {
        public static FishNet.Managing.NetworkManager NetMgr() => FishNet.InstanceFinder.NetworkManager;
    }

    public static class ClientLog
    {
        public static void Log(object message)
        {
            Debug.Log($"[Client]{message}");
        }
    }
    
    public static class ServerLog
    {
        public static void Log(object message)
        {
            Debug.Log($"[Server]{message}");
        }
    }
}