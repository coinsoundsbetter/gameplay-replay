using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Serializing;
using UnityEngine;

public class RoleNet : NetworkParticipant, ISerializeAs<RoleStateSnapshot>
{
    public readonly SyncVar<int> Id = new SyncVar<int>();
    public readonly SyncVar<Vector3> Pos = new SyncVar<Vector3>();
    public readonly SyncVar<Quaternion> Rot = new SyncVar<Quaternion>();
    public readonly SyncVar<int> Health = new SyncVar<int>();

    public static event Action<RoleNet> OnServerSpawn;
    public static event Action<RoleNet> OnServerDespawn;
    public static event Action<RoleNet> OnClientSpawn;
    public static event Action<RoleNet> OnClientDespawn;

    public override void OnStartServer()
    {
        OnServerSpawn?.Invoke(this);
    }

    public override void OnStopServer()
    {
        OnServerDespawn?.Invoke(this);
    }

    public override void OnStartClient()
    {
        OnClientSpawn?.Invoke(this);
    }

    public override void OnStopClient()
    {
        OnClientDespawn?.Invoke(this);
    }

    /// <summary>
    /// 共享字段
    /// </summary>
    protected Vector2 MoveInput;

    protected float GetTickDelta() => (float)TimeManager.TickDelta;

    /// <summary>
    /// 共享方法
    /// 记住这里的方法必须满足输入->确定输出
    /// </summary>
    public void SimulateMove(Vector2 move, float deltaTime)
    {
        var forward = (Rot.Value * Vector3.forward).normalized;
        var right = (Rot.Value * Vector3.right).normalized;
        var moveDir = forward * move.y + right * move.x;
        var motion = moveDir * 3f * deltaTime;
        Pos.Value += motion;
        transform.position = Pos.Value;
    }

    // 客户端发送消息
    public void Send(INetworkSerialize serialize)
    {
        var writer = new Writer();
        writer.WriteUInt16((ushort)serialize.GetMsgType());
        serialize.Serialize(writer);
        RpcToServer(writer.GetBuffer());
    }

    // 服务器下发消息
    public void Rpc(INetworkSerialize serialize)
    {
        var writer = new Writer();
        writer.WriteUInt16((ushort)serialize.GetMsgType());
        serialize.Serialize(writer);
        RpcToClient(writer.GetBuffer());
    }

    [ObserversRpc]
    private void RpcToClient(byte[] data)
    {
        OnClientReceived(data);
    }

    [ServerRpc]
    private void RpcToServer(byte[] data)
    {
        OnServerReceived(data);
    }
    
    protected virtual void OnClientReceived(byte[] data) { }
    protected virtual void OnServerReceived(byte[] data) { }
    
    public byte[] Serialize()
    {
        var writer = new Writer();
        writer.Write(Id.Value);
        writer.Write(Pos.Value);
        writer.Write(Rot.Value);
        return writer.GetBuffer();
    }

    public RoleStateSnapshot Deserialize(byte[] data)
    {
        var reader = new Reader(data, NetworkManager);
        var snapshot = new RoleStateSnapshot
        {
            Id = reader.ReadInt32(),
            Pos = reader.ReadVector3(),
            Rot = reader.ReadQuaternion64()
        };
        return snapshot;
    }

    public RoleStateSnapshot Parse()
    {
        return new RoleStateSnapshot()
        {
            Id = Id.Value,
            Pos = Pos.Value,
            Rot = Rot.Value,
        };
    }
}

public interface INetworkSerialize
{
    byte[] Serialize(Writer writer);
    void Deserialize(Reader reader);
    NetworkMsg GetMsgType();
}

public interface ISerializeAs<T>
{
    byte[] Serialize();
    T Deserialize(byte[] data);
    T Parse();
}

public struct RoleStateSnapshot 
{
    public int Id;
    public Vector3 Pos;
    public Quaternion Rot;
    public int Health;
}
