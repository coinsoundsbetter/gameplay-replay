using UnityEngine;

namespace KillCam.Client {
    public class ClientData {
        public static ClientData Instance { get; private set; }

        public static void Create() {
            if (Instance != null) {
                Debug.LogError("multi instance already exists!");
                return;
            }

            Instance = new ClientData();
        }

        public static void Destroy() {
            Instance = null;
        }

        public bool IsReplayPrepare { get; set; }
        public bool IsReplaying { get; set; }
    }
}