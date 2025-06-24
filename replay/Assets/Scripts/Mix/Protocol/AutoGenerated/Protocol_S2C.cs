// AutoGenerate -> 2025/6/24 18:27:20

namespace KillCam
{
	public struct S2C_NetSpawnPlayer  : IServerRpc
	{
		public const NetMsg Msg = NetMsg.S2C_NetSpawnPlayer ;

		public int PlayerId;
		public Unity.Collections.FixedString32Bytes PlayerName;
		public UnityEngine.Vector3 Pos;
		public UnityEngine.Quaternion Rot;

		public void Serialize(FishNet.Serializing.Writer writer)
		{
			writer.WriteInt32(PlayerId);
			writer.WriteFixedString32Bytes(PlayerName);
			writer.WriteVector3(Pos);
			writer.WriteQuaternion64(Rot);
		}

		public void Deserialize(FishNet.Serializing.Reader reader)
		{
			PlayerId = reader.ReadInt32();
			PlayerName = reader.ReadFixedString32Bytes();
			Pos = reader.ReadVector3();
			Rot = reader.ReadQuaternion64();
		}

		public NetMsg GetMsgType() => Msg;
	}
}
