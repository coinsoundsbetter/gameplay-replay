using UnityEngine;

namespace KillCam.Client {
    public class AppData {
        public static AppData Instance { get; private set; }

        public static void Create() {
            if (Instance != null) {
                Debug.LogError("multi instance already exists!");
                return;
            }

            Instance = new AppData();
        }

        public static void Destroy() {
            Instance = null;
        }

        public bool IsReplayPrepare { get; set; }
        public bool IsReplaying { get; set; }
    }
}