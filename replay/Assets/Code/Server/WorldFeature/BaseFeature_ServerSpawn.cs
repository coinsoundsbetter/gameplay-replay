using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using UnityEngine;

namespace KillCam.Server
{
    public class BaseFeature_ServerSpawn : Feature, INetworkServer
    {
        private NetworkManager mgr;
        private readonly Dictionary<int, RoleNet> roleStates = new();
        private readonly Dictionary<int, Server_RoleLogic> roleLogics = new();
        
        public BaseFeature_ServerSpawn(NetworkManager manager)
        {
            mgr = manager;
        }

        public override void OnCreate()
        {
            RoleNet.OnServerSpawn += OnRoleNetSpawn;
            RoleNet.OnServerDespawn += OnRoleDespawn;
            world.AddFrameUpdate(OnFrameTick);
            world.AddLogicUpdate(OnLogicTick);
        }

        public override void OnDestroy()
        {
            RoleNet.OnServerSpawn -= OnRoleNetSpawn;
            RoleNet.OnServerDespawn -= OnRoleDespawn;
            world.RemoveFrameUpdate(OnFrameTick);
            world.RemoveLogicUpdate(OnLogicTick);
        }
        
        private void OnFrameTick(float delta)
        {
            foreach (var role in roleLogics.Values)
            {
                role.TickFrame(delta);
            }
        }
        
        private void OnLogicTick(double delta)
        {
            foreach (var role in roleLogics.Values)
            {
                role.TickLogic(delta);
            }
        }

        private void OnRoleNetSpawn(RoleNet net)
        {
            var id = net.Id.Value;
            roleStates.Add(id, net);
            var logic = new Server_RoleLogic();
            roleLogics.Add(id, logic);
            logic.Init(world);
        }
        
        private void OnRoleDespawn(RoleNet net)
        {
            var id = net.Id.Value;
            if (roleLogics.Remove(id, out var logic))
            {
                logic.Clear();
            }
            roleStates.Remove(id);
        }

        public void SpawnRole(NetworkConnection conn, int playerId)
        {
            var asset = Resources.Load<GameObject>("RoleNet");
            var instance = Object.Instantiate(asset);
            var role = instance.GetComponent<RoleNet>();
            role.Id.Value = playerId;
            var networkObj = instance.GetComponent<NetworkObject>();
            mgr.ServerManager.Spawn(networkObj, conn);
        }

        public void Rpc(INetworkSerialize data)
        {
            foreach (var roleNet in roleStates.Values)
            {
                roleNet.Rpc(data);
            }
        }

        public uint GetTick()
        {
            return mgr.TimeManager.LocalTick;
        }

        public bool TryGetRole(int playerId, out Server_RoleLogic roleLogic)
        {
            if (roleLogics.TryGetValue(playerId, out roleLogic))
            {
                return true;
            }

            return false;
        }
    }
}