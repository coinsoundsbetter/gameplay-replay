using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Managing;
using UnityEngine;

namespace KillCam {
    public class BattleInitializer : MonoBehaviour {
        [SerializeField] private NetworkManager manager;
        [SerializeField] private bool isStartServer;
        [SerializeField] private bool isStartClient;
        [SerializeField] private bool isOpenReplayFunction;
        private BattleWorld server;
        private BattleWorld client;
        private BattleWorld replayClient;
        public static event Action OnGUIContent;

        private void Start() {
            if (isStartServer) {
                LoadServer();
            }

            if (isStartClient) {
                LoadClient();
            }

            manager.TimeManager.OnUpdate += OnFrameTick;
            manager.TimeManager.OnTick += OnFixedTick;
        }

        private void OnDestroy() {
            manager.TimeManager.OnUpdate -= OnFrameTick;
            manager.TimeManager.OnTick -= OnFixedTick;
            replayClient?.Destroy();
            server?.Destroy();
            client?.Destroy();
            OnGUIContent = null;
        }

        private void OnGUI() {
            OnGUIContent?.Invoke();
        }

        private void OnFrameTick() {
            var delta = Time.deltaTime;
            server?.FrameUpdate(delta);
            client?.FrameUpdate(delta);
            replayClient?.FrameUpdate(delta);
        }

        private void OnFixedTick() {
            var delta = manager.TimeManager.TickDelta;
            server?.LogicUpdate(delta);
            client?.LogicUpdate(delta);
            replayClient?.LogicUpdate(delta);
        }

        private void LoadServer() {
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(asm => asm.GetName().Name?.Contains("KillCam.Server") == true);
            var types = assembly.GetTypes().Where(t =>
                !t.IsAbstract && !t.IsInterface && typeof(ServerEntryBase).IsAssignableFrom(t));
            var enumerable = types as Type[] ?? types.ToArray();
            if (!enumerable.Any() || enumerable.Length > 1) {
                UnityEngine.Debug.LogError($"Load Server Failed");
                return;
            }

            var getType = enumerable.First();
            var ctor = getType.GetConstructor(new[] { typeof(NetworkManager) });
            if (ctor != null) {
                var instance = ctor.Invoke(new object[] { manager }) as ServerEntryBase;
                server = new BattleWorld();
                server.Init(WorldFlag.Server, instance, "ServerWorld");
            }
        }

        private void LoadClient() {
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(asm => asm.GetName().Name?.Contains("KillCam.Client") == true);

            if (assembly == null) {
                UnityEngine.Debug.LogError("Client assembly not found.");
                return;
            }

            // 处理 ClientInitialize
            var clientType = assembly.GetTypes()
                .FirstOrDefault(t => !t.IsAbstract && !t.IsInterface && typeof(ClientEntryBase).IsAssignableFrom(t));

            if (clientType == null) {
                UnityEngine.Debug.LogError("Load Client Failed");
                return;
            }

            var ctor = clientType.GetConstructor(new[] { typeof(NetworkManager) });
            if (ctor == null) {
                UnityEngine.Debug.LogError("Client constructor not found.");
                return;
            }

            var boostrap = ctor.Invoke(new object[] { manager }) as GameEntry;
            client = new BattleWorld();
            client.Init(WorldFlag.Client, boostrap, "ClientWorld");

            // 处理 ReplayInitialize
            if (isOpenReplayFunction) {
                var replayType = assembly.GetTypes()
                    .FirstOrDefault(
                        t => !t.IsAbstract && !t.IsInterface && typeof(ReplayEntryBase).IsAssignableFrom(t));

                if (replayType == null) {
                    UnityEngine.Debug.LogError("Load Client Replay Failed");
                    return;
                }

                var ctorReplay = replayType.GetConstructor(Type.EmptyTypes);
                if (ctorReplay == null) {
                    UnityEngine.Debug.LogError("Replay constructor not found.");
                    return;
                }

                var replayBoostrap = ctorReplay.Invoke(null) as GameEntry;
                replayClient = new BattleWorld();
                replayClient.Init(WorldFlag.Client | WorldFlag.Replay, replayBoostrap, "ReplayWorld");
            }
        }
    }
}