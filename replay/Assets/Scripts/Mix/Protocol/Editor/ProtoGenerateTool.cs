using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using UnityEditor;

namespace KillCam
{
    public class ProtoGenerateTool : EditorWindow
    {
        [MenuItem("GameTools/Proto Generate Tool")]
        public static void Execute()
        {
            //GenerateCode();
        }

        public static void GenerateCode(List<ProtocolDef> defs, string outputPath)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"// AutoGenerate -> {System.DateTime.Now}");
            sb.AppendLine();
            sb.AppendLine("namespace KillCam");
            sb.AppendLine("{");
            
            foreach (var def in defs)
            {
                // 消息类型
                sb.AppendLine($"\tpublic struct {def.ProtocolName} : {def.InterfaceName}");
                sb.AppendLine("\t{");
                sb.AppendLine($"\tpublic const NetMsg Msg = NetMsg.{def.ProtocolName}");
                sb.AppendLine();
                // 字段
                foreach (var (fieldType, fieldName) in def.TypeToNames)
                {
                    sb.AppendLine($"\tpublic {fieldType} {fieldName}");
                }
                sb.AppendLine();
                // 序列化方法
                sb.AppendLine($"\tpublic void Serialize(Writer writer)");
                sb.AppendLine("\t{");
                // 序列化
                foreach (var (fieldType, fieldName) in def.TypeToNames)
                {
                    //sb.AppendLine()
                }
                sb.AppendLine("\t}");
            }

            sb.AppendLine("}");
        }

        private string GetWrite(Type type)
        {
            if (type == typeof(FixedString64Bytes) ||
                type == typeof(FixedString32Bytes))
            {
               
            }

            return "";
        }

        public class ProtocolDef
        {
            public string ProtocolName;
            public string InterfaceName;
            public List<(Type, string)> TypeToNames = new List<(Type, string)>();
        }
    }
}