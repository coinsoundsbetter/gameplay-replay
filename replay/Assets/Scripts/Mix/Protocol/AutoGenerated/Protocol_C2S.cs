// AutoGenerate -> 2025/6/25 21:51:26

namespace KillCam
{
	public struct C2S_PlayerInputState : IClientSend
	{
		public const NetMsg Msg = NetMsg.C2S_PlayerInputState;

		public PlayerInputState Data;

		public void Serialize(FishNet.Serializing.Writer writer)
		{
			writer.WritePlayerInputState(Data);
		}

		public void Deserialize(FishNet.Serializing.Reader reader)
		{
			Data = reader.ReadPlayerInputState();
		}

		public NetMsg GetMsgType() => Msg;
	}
}
