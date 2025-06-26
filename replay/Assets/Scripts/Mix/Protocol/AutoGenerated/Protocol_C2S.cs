// AutoGenerate -> 2025/6/26 11:11:49

namespace KillCam
{
	public struct C2S_InputElement : IClientSend
	{
		public const NetMsg Msg = NetMsg.C2S_InputElement;

		public InputElement Data;

		public void Serialize(FishNet.Serializing.Writer writer)
		{
			writer.WriteInputElement(Data);
		}

		public void Deserialize(FishNet.Serializing.Reader reader)
		{
			Data = reader.ReadInputElement();
		}

		public NetMsg GetMsgType() => Msg;
	}
}
