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
        private Dictionary<int, RoleNet> roleStates = new();
        
        public BaseFeature_ServerSpawn(NetworkManager manager)
        {
            mgr = manager;
        }

        public override void OnCreate()
        {
            RoleNet.OnServerSpawn += OnRoleNetSpawn;
            RoleNet.OnServerDespawn += OnRoleDespawn;
        }

        public override void OnDestroy()
        {
            RoleNet.OnServerSpawn -= OnRoleNetSpawn;
            RoleNet.OnServerDespawn -= OnRoleDespawn;
        }

        private void OnRoleNetSpawn(RoleNet net)
        {
            roleStates.Add(net.Id.Value, net);
        }
        
        private void OnRoleDespawn(RoleNet net)
        {
            roleStates.Remove(net.Id.Value);
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
    }
}