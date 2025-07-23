using FishNet.Serializing;

namespace KillCam
{
    public static class WriteEx
    {
        public static void WriteAllCharacterSnapshot(this Writer writer, AllCharacterSnapshot data)
        {
            if (data.StateData.IsCreated)
            {
                writer.Write(data.StateData.Count);
                foreach (var kvp in data.StateData)
                {
                    writer.Write(kvp.Key);
                    writer.WriteCharacterStateData(kvp.Value);
                }
            }
            else
            {
                writer.Write(0);
            }
            
            if (data.InputData.IsCreated)
            {
                writer.Write(data.InputData.Count);
                foreach (var kvp in data.InputData)
                {
                    writer.Write(kvp.Key);
                    writer.WriteCharacterInputData(kvp.Value);
                }
            }
            else
            {
                writer.Write(0);
            }
        }

        public static void WriteCharacterStateData(this Writer writer, CharacterStateData data)
        {
            writer.WriteVector3(data.Pos);
            writer.WriteQuaternion64(data.Rot);
        }
        
        public static void WriteCharacterInputData(this Writer writer, CharacterInputData data)
        {
            writer.WriteVector2Int(data.Move);
        }
    }
}