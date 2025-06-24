// AutoGenerate -> 2025/6/24 14:47:02

namespace KillCam
{
	public struct C2S_PlayerInputState  : IClientSend
	{
		public const NetMsg Msg = NetMsg.C2S_PlayerInputState ;

		public System.Int32 a;
		public System.Single b;

		public void Serialize(FishNet.Serializing.Writer writer)
		{
			writer.WriteInt32(a);
			writer.WriteSingle(b);
		}

		public void Deserialize(FishNet.Serializing.Reader reader)
		{
			a = reader.ReadInt32();
			b = reader.ReadSingle();
		}

		public NetMsg GetMsgType() => Msg;
	}
}
