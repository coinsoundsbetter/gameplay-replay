using FishNet.Managing;
using KillCam.Client;
using KillCam.Server;
using UnityEngine;

namespace KillCam
{
    public class BattleInitializer : MonoBehaviour
    {
        [SerializeField] private NetworkManager manager;
        [SerializeField] private bool isStartServer;
        [SerializeField] private bool isStartClient;
        private BattleWorld server;
        private BattleWorld client;
        private BattleWorld replayClient;

        private void Start()
        {
            server = new BattleWorld("server");
            server.Add(new ServerInitialize(manager));
            
            client = new BattleWorld("client");
            client.Add(new ClientInitialize(manager));
            
            replayClient = new BattleWorld("replay");
            replayClient.Add(new ReplayClientInitialize());
        }

        private void OnDestroy()
        {
            server.Dispose();
            client.Dispose();
        }

        private void Update()
        {
            server.Update();
            client.Update();
        }
    }
}