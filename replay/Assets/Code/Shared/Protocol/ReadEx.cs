using FishNet.Serializing;

namespace KillCam
{
    public static class ReadEx
    {
        public static RoleStateSnapshot ReadRoleSnapshot(this Reader reader)
        {
            var data = new RoleStateSnapshot
            {
                Pos = reader.ReadVector3(),
                Rot = reader.ReadQuaternion64(),
                Health = reader.ReadInt32(),
                MoveInput = reader.ReadVector2Int(),
            };
            return data;
        }
        
        public static AllCharacterSnapshot ReadAllCharacterSnapshot(this Reader reader)
        {
            int stateDataCnt = reader.ReadInt32();
            for (int i = 0; i < stateDataCnt; i++)
            {
                
            }

            /*if (data.StateData.IsCreated)
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
            }*/
        }

        /*public static void WriteCharacterStateData(this Writer writer, CharacterStateData data)
        {
            writer.WriteVector3(data.Pos);
            writer.WriteQuaternion64(data.Rot);
        }*/
        
        /*public static void WriteCharacterInputData(this Writer writer, CharacterInputData data)
        {
            writer.WriteVector2Int(data.Move);
        }*/
    }
}