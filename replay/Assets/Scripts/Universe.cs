using System;
using UnityEngine;

namespace KillCam {
    public class Universe : MonoBehaviour {
        public bool IsStartServer { get; private set; }
        public bool IsStartClient { get; private set; }
    
        private Client client;
        private Server server;
    
        private void Start() {
            IsStartServer = LaunchData.Instance.IsServer;
            IsStartClient = LaunchData.Instance.IsClient;
            if (IsStartServer) {
                server = new Server();
                server.Init();
            }
            if (IsStartClient) {
                client = new Client();
                client.Init();
            }
        }

        private void OnDestroy() {
            server?.Clear();
            client?.Clear();
        }
    }
}