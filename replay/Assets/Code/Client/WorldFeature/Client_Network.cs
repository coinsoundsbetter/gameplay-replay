using System;
using FishNet.Managing;
using FishNet.Transporting;
using UnityEngine;

namespace KillCam.Client
{
    public class Client_Network : Feature, INetwork
    {
        private readonly NetworkManager manager;
        private IClientRoleNet sender;
        private Action startAction;

        public Client_Network(NetworkManager mgr)
        {
            manager = mgr;
        }

        public void Start(Action started)
        {
            startAction = started;
            world.SetNetwork(this);
            RoleNet.OnClientSpawn += OnClientSpawn;
            RoleNet.OnClientDespawn += OnClientDespawn;
            manager.ClientManager.OnClientConnectionState += OnLocalConnectState;
            manager.ClientManager.StartConnection();
        }

        public void Stop()
        {
            world.RemoveNetwork(this);
            manager.ClientManager.StopConnection();
            RoleNet.OnClientSpawn -= OnClientSpawn;
            RoleNet.OnClientDespawn -= OnClientDespawn;
            manager.ClientManager.OnClientConnectionState -= OnLocalConnectState;
        }
        
        private void OnClientSpawn(IClientRoleNet net)
        {
            if (net.IsClientOwned())
            {
                sender = net;
            }
        }
        
        private void OnClientDespawn(IClientRoleNet net)
        {
            if (net.IsClientOwned())
            {
                sender = null;
            }
        }
        
        private void OnLocalConnectState(ClientConnectionStateArgs args)
        {
            if (args.ConnectionState == LocalConnectionState.Started)
            {
                startAction?.Invoke();
            }
        }

        public void SendLoginRequest(string userName)
        {
            manager.ClientManager.Broadcast<Login>(new Login()
            {
                UserName = "Coin",
            });    
        }

        public void Send(INetworkSerialize data)
        {
            sender?.Send(data);
        }

        public void Rpc(INetworkSerialize data) { }

        public new uint GetTick()
        {
            return manager.TimeManager.LocalTick;
        }
    }
}