using System;
using FishNet.Managing;
using KillCam.Client;
using KillCam.Client.Replay;
using KillCam.Server;
using UnityEngine;

namespace KillCam
{
    public class BattleInitializer : MonoBehaviour
    {
        [SerializeField] private NetworkManager manager;
        [SerializeField] private bool isStartServer;
        [SerializeField] private bool isStartClient;
        [SerializeField] private bool isOpenReplayFunction;
        private BattleWorld server;
        private BattleWorld client;
        private BattleWorld replayClient;
        public static event Action OnGUIContent;

        private void Start()
        {
            if (isStartServer)
            {
                server = new BattleWorld(WorldFlag.Server);
                server.Add(new ServerInitialize(manager));
            }

            if (isStartClient)
            {
                client = new BattleWorld(WorldFlag.Client);
                client.Add(new ClientInitialize(manager));
            }

            if (isOpenReplayFunction)
            {
                replayClient = new BattleWorld(WorldFlag.Client | WorldFlag.Replay);
                replayClient.Add(new ReplayIInitialize());
            }
            
            manager.TimeManager.OnUpdate += OnFrameUpdate;
            manager.TimeManager.OnTick += OnLogicUpdate;
        }
        
        private void OnDestroy()
        {
            manager.TimeManager.OnUpdate -= OnFrameUpdate;
            manager.TimeManager.OnTick -= OnLogicUpdate;
            server?.Dispose();
            client?.Dispose();
            replayClient?.Dispose();
            OnGUIContent = null;
        }

        private void OnGUI()
        {
            OnGUIContent?.Invoke();
        }

        private void OnFrameUpdate()
        {
            var delta = Time.deltaTime;
            server?.FrameUpdate(delta);
            client?.FrameUpdate(delta);
            replayClient?.FrameUpdate(delta);
        }
        
        private void OnLogicUpdate()
        {
            var delta = manager.TimeManager.TickDelta;
            server?.LogicUpdate(delta);
            client?.LogicUpdate(delta);
            replayClient?.LogicUpdate(delta);
        }
    }
}