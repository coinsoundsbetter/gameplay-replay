using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {
    private ClientWorld client;
    private ServerWorld server;
    private ReplayWorld replay;
    private NetworkRegistry networkRegistry;
    public static WorldManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        networkRegistry = new NetworkRegistry();
        if (Launch.Singleton.StartAsServer) {
            server = new ServerWorld();
            if (Launch.Singleton.UseReplay) {
                replay = new ReplayWorld();
            }
        }
        if (Launch.Singleton.StartAsClient) {
            client = new ClientWorld();
        }
        networkRegistry.OnReady(Launch.Singleton.StartAsClient, Launch.Singleton.StartAsServer);
        server?.Initialize();
        replay?.Initialize();
        client?.Initialize();
    }

    private void Update() {
        server?.Update();
        replay?.Update();
        client?.Update();
    }

    private void LateUpdate() {
        server?.LateUpdate();
        replay?.LateUpdate();
        client?.LateUpdate();
    }

    private void OnDestroy() {
        client?.Clear();
        server?.Clear();
        replay?.Clear();
        networkRegistry.OnClean();
        Launch.Clean();
    }

    public WorldBase GetWorld(WorldType flag) {
        switch (flag) {
            case WorldType.Server:
                return server;
            case WorldType.Client:
                return client;
            case WorldType.Replay:
                return replay;
        }

        return null;
    }
    
    public WorldBase[] GetWorlds(WorldType flag) {
        var res = new List<WorldBase>();
        if ((WorldType.Client & flag) != 0 && client != null) {
            res.Add(client);
        }

        if ((WorldType.Server & flag) != 0 && server != null) {
            res.Add(server);
        }

        if ((WorldType.Replay & flag) != 0 && replay != null) {
            res.Add(replay);
        }

        return res.ToArray();
    }

    public WorldType GetWorldType(WorldBase check) {
        if (check is ClientWorld) {
            return WorldType.Client;
        }

        if (check is ServerWorld) {
            return WorldType.Server;
        }

        if (check is ReplayWorld) {
            return WorldType.Replay;
        }

        return WorldType.Undefined;
    }
}
