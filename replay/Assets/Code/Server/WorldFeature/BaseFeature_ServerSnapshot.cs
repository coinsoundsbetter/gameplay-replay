using System.Collections.Generic;
using System.IO;
using FishNet.Serializing;
using KillCam.Client;
using UnityEngine;

namespace KillCam.Server
{
    public class BaseFeature_ServerSnapshot : Feature
    {
        private readonly SortedList<uint, Dictionary<int, RoleStateSnapshot>> roleSnapshots = new();

        public override void OnCreate()
        {
            world.AddLogicUpdate(OnLogicTick);
            BattleInitializer.OnGUIContent += () =>
            {
                if (GUILayout.Button("Save World"))
                {
                    SaveWorld();
                }

                if (GUILayout.Button("Replay World"))
                {
                    ReplayWorld();
                }
            };
        }

        public override void OnDestroy()
        {
            world.RemoveLogicUpdate(OnLogicTick);
        }

        private void OnLogicTick(double delta)
        {
            TakeRoleSnapshot();
        }

        private void TakeRoleSnapshot()
        {
            // 创建该逻辑帧的角色状态快照列表
            var stateSnapshots = new Dictionary<int, RoleStateSnapshot>(8);
            roleSnapshots.Add(GetTick(), stateSnapshots);

            // 填充快照信息
            var dict = Get<Server_RoleManager>().RoleLogics;
            foreach (var kvp in dict)
            {
                var roleSnapState = kvp.Value.GetNetStateData();
                stateSnapshots.Add(kvp.Key, roleSnapState);
            }
        }

        private void SaveWorld()
        {
            SortedList<uint, byte[]> worldStreams = new();
            Dictionary<int, RoleStateSnapshot> roleStateBuffer = new Dictionary<int, RoleStateSnapshot>();
            // 遍历每一个tick的角色状态快照
            foreach (var kvp in roleSnapshots)
            {
                var worldState = new S2C_Replay_WorldStateSnapshot
                {
                    RoleStateSnapshots = new Dictionary<int, RoleStateSnapshot>(),
                    Tick = kvp.Key,
                };
                
                // 遍历当前tick所有角色的快照
                bool isDirty = false;
                foreach (var sKvp in kvp.Value)
                {
                    var roleId = sKvp.Key;
                    var roleState = sKvp.Value;
                    bool isAppend = true;
                    if (!roleStateBuffer.TryGetValue(roleId, out var value))
                    {
                        roleStateBuffer.Add(roleId, roleState);
                    }
                    else
                    {
                        if (value == roleState)
                        {
                            isAppend = false;
                        }
                        else
                        {
                            roleStateBuffer[roleId] = roleState;
                        }
                    }

                    if (isAppend)
                    {
                        isDirty = true;
                        worldState.RoleStateSnapshots.Add(roleId, roleState);
                        Debug.Log($"保存快照:{roleId}, {roleState.MoveInput}");
                    }
                }

                if (isDirty)
                {
                    var bytes = worldState.Serialize(new Writer());
                    worldStreams.Add(kvp.Key, bytes);
                }
            }
            
            SaveWorldFile(worldStreams);
        }

        private void SaveWorldFile(SortedList<uint, byte[]> worldStreams)
        {
            // 序列化世界快照列表,并且保存起来
            using var ms = new System.IO.MemoryStream();
            using var bw = new System.IO.BinaryWriter(ms);

            // 写入总数
            bw.Write(worldStreams.Count);

            foreach (var kvp in worldStreams)
            {
                bw.Write(kvp.Key);                    
                bw.Write(kvp.Value.Length);            
                bw.Write(kvp.Value);                   
            }
            
            // 保存为文件
            var bytes = ms.ToArray();
            string filePath = Path.Combine(Application.dataPath, "world_streams.txt");
            File.WriteAllBytes(filePath, bytes);
        }

        private void ReplayWorld()
        {
            var filePath = Path.Combine(Application.dataPath, "world_streams.txt");
            if (!File.Exists(filePath))
            {
                Debug.LogError("回放文件不存在:" + filePath);
                return;
            }

            var fileData = File.ReadAllBytes(filePath);
            ClientWorldsChannel.StartReplay(fileData);
        }
    }
}