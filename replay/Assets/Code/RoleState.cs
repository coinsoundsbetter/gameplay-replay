using Code;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Serializing;
using UnityEngine;

public class RoleState : NetworkParticipant
{
    public readonly SyncVar<int> Id = new SyncVar<int>();
    public readonly SyncVar<Vector3> Pos = new SyncVar<Vector3>();
    public readonly SyncVar<Quaternion> Rot = new SyncVar<Quaternion>();
    public readonly SyncVar<int> Health = new SyncVar<int>();

    [SerializeField] private ServerRoleState server;
    [SerializeField] private ClientRoleState client;

    /// <summary>
    /// 共享字段
    /// </summary>
    protected Vector2 MoveInput;

    /// <summary>
    /// 共享方法
    /// 记住这里的方法必须满足输入->确定输出
    /// </summary>
    public void SimulateMove(Vector2 move)
    {
        var forward = (Rot.Value * Vector3.forward).normalized;
        var right = (Rot.Value * Vector3.right).normalized;
        var moveDir = forward * move.y + right * move.x;
        var motion = moveDir * 3f;
        Pos.Value += motion;
        transform.position = Pos.Value;
    }

    // 客户端发送消息
    public void Send(INetworkSerialize serialize)
    {
        var writer = new Writer();
        writer.WriteUInt16((ushort)serialize.GetMsgType());
        serialize.Serialize(writer);
        server.RpcToServer(writer.GetBuffer());
    }

    // 服务器下发消息
    public void Rpc(INetworkSerialize serialize)
    {
        var writer = new Writer();
        writer.WriteUInt16((ushort)serialize.GetMsgType());
        serialize.Serialize(writer);
        client.RpcToClient(writer.GetBuffer());
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
}

public struct RoleStateSnapshot 
{
    public int Id;
    public Vector3 Pos;
    public Quaternion Rot;
    public int Health;
}
