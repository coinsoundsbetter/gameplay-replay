using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Serializing;
using UnityEngine;

namespace KillCam {
    public class RoleNet : NetworkBehaviour, IClientRoleNet, IServerRoleNet {
        /// <summary>
        /// 角色同步状态
        /// </summary>
        public readonly SyncVar<int> Id = new(new SyncTypeSettings(WritePermission.ServerOnly));

        public readonly SyncVar<Vector3> Pos = new(new SyncTypeSettings(WritePermission.ServerOnly));
        public readonly SyncVar<Quaternion> Rot = new(new SyncTypeSettings(WritePermission.ServerOnly));
        public readonly SyncVar<int> Health = new(new SyncTypeSettings(WritePermission.ServerOnly));

        /// <summary>
        /// 全局事件
        /// </summary>
        public static event Action<IServerRoleNet> OnServerSpawn;

        public static event Action<IServerRoleNet> OnServerDespawn;
        public static event Action<IClientRoleNet> OnClientSpawn;
        public static event Action<IClientRoleNet> OnClientDespawn;
        public static event Action<byte[]> OnClientReceiveData;
        public static event Action<int, byte[]> OnServerReceiveData;

        public override void OnStartServer() {
            OnServerSpawn?.Invoke(this);
        }

        public override void OnStopServer() {
            OnServerDespawn?.Invoke(this);
        }

        public override void OnStartClient() {
            OnClientSpawn?.Invoke(this);
        }

        public override void OnStopClient() {
            OnClientDespawn?.Invoke(this);
        }

        // 客户端发送消息
        public void Send<T>(T message) where T : INetworkMsg {
            var writer = new Writer();
            writer.WriteUInt16((ushort)message.GetMsgType());
            message.Serialize(writer);
            RpcToServer(writer.GetBuffer());
        }

        public void Rpc<T>(T message) where T : INetworkMsg {
            var writer = new Writer();
            writer.WriteUInt16((ushort)message.GetMsgType());
            message.Serialize(writer);
            RpcToClient(writer.GetBuffer());
        }

        public void TargetRpc<T>(int id, T message) where T : INetworkMsg { }

        [ObserversRpc]
        private void RpcToClient(byte[] data) {
            OnClientReceiveData?.Invoke(data);
        }

        [ServerRpc]
        private void RpcToServer(byte[] data) {
            OnServerReceiveData?.Invoke(Id.Value, data);
        }

        public int GetId() {
            return Id.Value;
        }

        public bool IsClientOwned() {
            return IsOwner;
        }

        public bool IsControlTarget() {
            return IsController;
        }

        public CharacterStateData GetData() {
            return new CharacterStateData() {
                Pos = Pos.Value,
                Rot = Rot.Value,
            };
        }
    }
}