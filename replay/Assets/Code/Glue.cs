using System.Collections.Generic;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

public class Glue : MonoBehaviour
{
    public NetworkManager manager;
    public int RoleCount { get; private set; }
    public GameState GameState { get; private set; }
    
    public Dictionary<int, ServerRoleState> ServerRoles = new Dictionary<int, ServerRoleState>();
    public Dictionary<int, ClientRoleState> ClientRoles = new Dictionary<int, ClientRoleState>();
    
    void Start()
    {
        manager.ServerManager.StartConnection();
        manager.ClientManager.StartConnection();
        manager.ServerManager.RegisterBroadcast<Login>(OnLogin);
        manager.ServerManager.OnServerConnectionState += OnServerState;
        manager.ClientManager.OnClientConnectionState += OnClientState;
    }

    private void OnClientState(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            manager.ClientManager.Broadcast(new Login());
        }
    }

    private void OnDestroy()
    {
        manager.ServerManager.OnServerConnectionState -= OnServerState;
        manager.ClientManager.OnClientConnectionState -= OnClientState;
        manager.ServerManager.UnregisterBroadcast<Login>(OnLogin);
    }

    private void OnServerState(ServerConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        var asset = Resources.Load<GameObject>("GameState");
        var instance = Instantiate(asset);
        var networkObject = instance.GetComponent<NetworkObject>();
        GameState = networkObject.GetComponent<GameState>();
        manager.ServerManager.Spawn(networkObject);
    }
    
    private void OnLogin(NetworkConnection conn, Login req, Channel channel)
    {
       var asset = Resources.Load<GameObject>("RoleState");
       var instance = Instantiate(asset);
       var networkObject = instance.GetComponent<NetworkObject>();
       var serverRole = instance.GetComponentInChildren<ServerRoleState>();
       serverRole.Id.Value = ++RoleCount;
       serverRole.Pos.Value = Vector3.zero;
       serverRole.Rot.Value = Quaternion.identity;
       
       var clientRole = instance.GetComponentInChildren<ClientRoleState>();
       clientRole.Id.Value = ++RoleCount;
       clientRole.Pos.Value = Vector3.zero;
       clientRole.Rot.Value = Quaternion.identity;
       
       ClientRoles.Add(clientRole.Id.Value, clientRole);
       ServerRoles.Add(serverRole.Id.Value, serverRole);
       
       manager.ServerManager.Spawn(networkObject, conn);
    }
}

public struct Login : IBroadcast
{
    
}