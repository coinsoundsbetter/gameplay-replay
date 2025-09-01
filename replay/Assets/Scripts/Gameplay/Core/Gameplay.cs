using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FishNet.Managing;
using UnityEngine;

namespace Gameplay.Core {
    public class Gameplay : MonoBehaviour {
        [SerializeField] private NetworkManager netPlugin;
        [SerializeField] private bool isLaunchServer = true;
        [SerializeField] private bool isLaunchClient = true;
        [SerializeField] private bool isLaunchReplay = false;
        private readonly List<World> worlds = new();
        private readonly List<WorldBootstrap> bootstraps = new();
        
        private void Start() {
            if (isLaunchClient) {
                LoadWorld("ClientBootstrap", "ClientWorld", WorldFlag.Client);
            }

            if (isLaunchServer) {
                LoadWorld("ServerBootstrap", "ServerWorld", WorldFlag.Server);
            }

            if (isLaunchReplay) {
                LoadWorld("ReplayBootstrap", "ReplayWorld", WorldFlag.Client | WorldFlag.Replay);
            }
            
            netPlugin.TimeManager.OnTick += OnLogicTick;
            netPlugin.TimeManager.OnUpdate += OnFrameTick;
        }

        private void OnDestroy() {
            foreach (var world in worlds) {
                world.Dispose();
            }

            foreach (var bootstrap in bootstraps) {
                bootstrap.Dispose();
            }
            
            worlds.Clear();
        }

        private void OnLogicTick() {
            var delta = netPlugin.TimeManager.TickDelta;
            foreach (var world in worlds) {
                world.TickLogic(delta);
            }
        }
        
        private void OnFrameTick() {
            var delta = Time.deltaTime;
            foreach (var world in worlds) {
                world.TickFrame(delta);
            }
        }
        
        private void LoadWorld(string bootstrapName, string worldName, WorldFlag flag)
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var type = allAssemblies
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null)!; }
                })
                .FirstOrDefault(t => t.Name == bootstrapName && typeof(WorldBootstrap).IsAssignableFrom(t));

            if (type == null)
            {
                Debug.LogWarning($"未找到 {bootstrapName}");
                return;
            }

            var world = new World(worldName);
            var bootstrap = (WorldBootstrap)Activator.CreateInstance(type, world, flag)!;
            bootstrap.Initialize(netPlugin);
            bootstraps.Add(bootstrap);
            worlds.Add(world);
            Debug.Log($"加载 {worldName} 成功 ({bootstrapName})");
        }
    }
}