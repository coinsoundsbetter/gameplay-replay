using System;
using System.Collections.Generic;
using System.IO;
using FishNet.Serializing;
using KillCam.Client;
using Unity.Collections;
using UnityEngine;

namespace KillCam.Server
{
    public class Server_StateSnapshot : Feature
    {
        private readonly SortedList<uint, AllCharacterSnapshot> characterSnapshots = new();
        private int stateSnapshotCountdown = 0;

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
            var snapshot = new AllCharacterSnapshot()
            {
                StateData = new NativeHashMap<int, CharacterStateData>(4, Allocator.Persistent)
            };
            characterSnapshots.Add(GetTick(), snapshot);
            
            stateSnapshotCountdown--;
            if (stateSnapshotCountdown < 0)
            {
                TakeCharacterStateSnapshot(ref snapshot);
                stateSnapshotCountdown = 3;
            }

            TakeCharacterInputSnapshot(ref snapshot);
        }

        private void TakeCharacterStateSnapshot(ref AllCharacterSnapshot snapshot)
        {
            var characterMgr = Get<Server_CharacterManager>();
            foreach (var (id, character) in characterMgr.RoleActors)
            {
                var stateData = character.GetDataReadOnly<CharacterStateData>();
                snapshot.StateData.Add(id, stateData);
            }
        }

        private void TakeCharacterInputSnapshot(ref AllCharacterSnapshot snapshot)
        {
            var characterMgr = Get<Server_CharacterManager>();
            foreach (var (id, character) in characterMgr.RoleActors)
            {
                var inputData = character.GetDataReadOnly<CharacterInputData>();
                snapshot.InputData.Add(id, inputData);
            }
        }

        private void SaveWorld()
        {
            NativeList<S2C_Replay_WorldStateSnapshot> worldSnapshots =
                new NativeList<S2C_Replay_WorldStateSnapshot>(Allocator.Temp);
            foreach (var (tick, snapshot) in characterSnapshots)
            {
                var s2cSnapshot = new S2C_Replay_WorldStateSnapshot()
                {
                    Tick = tick,
                    CharacterSnapshot = new AllCharacterSnapshot()
                    {
                        StateData = new NativeHashMap<int, CharacterStateData>(4, Allocator.Temp),
                        InputData = new NativeHashMap<int, CharacterInputData>(4, Allocator.Temp),
                    }
                };
                
                foreach (var kvp in snapshot.StateData)
                {
                    var id = kvp.Key;
                    var data = kvp.Value;
                    s2cSnapshot.CharacterSnapshot.StateData.Add(id, data);
                }

                foreach (var kvp in snapshot.InputData)
                {
                    var id = kvp.Key;
                    var data = kvp.Value;
                    s2cSnapshot.CharacterSnapshot.InputData.Add(id, data);
                }

                bool isAddSomething = s2cSnapshot.CharacterSnapshot.StateData.Count > 0 ||
                                      s2cSnapshot.CharacterSnapshot.InputData.Count > 0;
                if (isAddSomething)
                {
                    worldSnapshots.Add(s2cSnapshot);
                }
            }

            if (worldSnapshots.Length > 0)
            {
                byte[] packData;
                using (var ms = new MemoryStream())
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(worldSnapshots.Length);
                    foreach (var snapshot in worldSnapshots)
                    {
                        var data = snapshot.Serialize(new Writer());
                        bw.Write(data.Length);
                        bw.Write(data);
                    }
                    packData = ms.ToArray();
                }

                string filePath = Path.Combine(Application.dataPath, "world_streams.txt");
                File.WriteAllBytes(filePath, packData);
            }
        }

        private void ReplayWorld()
        {
            var filePath = Path.Combine(Application.dataPath, "world_streams.txt");
            if (!File.Exists(filePath))
            {
                Debug.LogError("回放文件不存在:" + filePath);
                return;
            }
            
            var byteData = File.ReadAllBytes(filePath);
            ClientWorldsChannel.StartReplay(byteData);
        }
    }
}