using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Serializing;
using UnityEngine;

namespace KillCam
{
    public class RoleNet : NetworkParticipant, IClientRoleNet, IServerRoleNet
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
        public static event Action<IServerRoleNet> OnServerSpawn;
        public static event Action<IServerRoleNet> OnServerDespawn;
        public static event Action<IClientRoleNet> OnClientSpawn;
        public static event Action<IClientRoleNet> OnClientDespawn;
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

        public void SetServerSyncData(RoleStateSnapshot syncData)
        {
            Pos.Value = syncData.Pos;
            Rot.Value = syncData.Rot;
            Health.Value = syncData.Health;
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

        public RoleStateSnapshot Read()
        {
            return new RoleStateSnapshot()
            {
                Pos = Pos.Value,
                Rot = Rot.Value,
            };
        }

        public void Write(RoleStateSnapshot data)
        {
            
        }

        public int GetId()
        {
            return Id.Value;
        }

        public bool IsClientOwned()
        {
            return IsOwner;
        }

        public bool IsControlTarget()
        {
            return IsController;
        }

        public RoleStateSnapshot GetData()
        {
            return Read();
        }
    }

    public struct RoleStateSnapshot : IEquatable<RoleStateSnapshot>
    {
        public Vector3 Pos;
        public Quaternion Rot;
        public int Health;

        public bool Equals(RoleStateSnapshot other)
        {
            return Pos.Equals(other.Pos) && Rot.Equals(other.Rot) && Health == other.Health;
        }

        public override bool Equals(object obj)
        {
            return obj is RoleStateSnapshot other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Pos, Rot, Health);
        }

        public static bool operator ==(RoleStateSnapshot left, RoleStateSnapshot right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RoleStateSnapshot left, RoleStateSnapshot right)
        {
            return !left.Equals(right);
        }
    }
}