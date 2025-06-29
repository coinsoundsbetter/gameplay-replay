using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Serializing;
using UnityEngine;

namespace KillCam
{
    public class RoleNet : NetworkParticipant, ISerializeAs<RoleStateSnapshot>
    {
        /// <summary>
        /// 角色同步状态
        /// </summary>
        public readonly SyncVar<int> Id = new (new SyncTypeSettings(WritePermission.ServerOnly));
        public readonly SyncVar<Vector3> Pos = new (new SyncTypeSettings(WritePermission.ServerOnly));
        public readonly SyncVar<Quaternion> Rot = new (new SyncTypeSettings(WritePermission.ServerOnly));
        public readonly SyncVar<int> Health = new (new SyncTypeSettings(WritePermission.ServerOnly));
        
        /// <summary>
        /// 全局事件
        /// </summary>
        public static event Action<RoleNet> OnServerSpawn;
        public static event Action<RoleNet> OnServerDespawn;
        public static event Action<RoleNet> OnClientSpawn;
        public static event Action<RoleNet> OnClientDespawn;
        public static event Action<byte[]> OnClientReceiveData; 
        public static event Action<int, byte[]> OnServerReceiveData;

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
            OnClientReceiveData?.Invoke(data);
        }

        [ServerRpc]
        private void RpcToServer(byte[] data)
        {
            OnServerReceiveData?.Invoke(Id.Value, data);
        }

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

    public struct RoleStateSnapshot
    {
        public int Id;
        public Vector3 Pos;
        public Quaternion Rot;
        public int Health;
    }
}