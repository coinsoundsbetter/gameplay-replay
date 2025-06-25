// AutoGenerate -> 2025/6/25 21:06:48

namespace KillCam
{
	public struct S2C_NetSpawnPlayer : IServerRpc
	{
		public const NetMsg Msg = NetMsg.S2C_NetSpawnPlayer;

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

	public struct S2C_Tick : IServerRpc
	{
		public const NetMsg Msg = NetMsg.S2C_Tick;

		public uint ServerTick;

		public void Serialize(FishNet.Serializing.Writer writer)
		{
			writer.WriteUInt32(ServerTick);
		}

		public void Deserialize(FishNet.Serializing.Reader reader)
		{
			ServerTick = reader.ReadUInt32();
		}

		public NetMsg GetMsgType() => Msg;
	}
}
