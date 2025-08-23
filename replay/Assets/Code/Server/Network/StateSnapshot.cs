using System;
using System.Collections.Generic;
using System.IO;
using FishNet.Serializing;
using Unity.Collections;
using UnityEngine;

namespace KillCam.Server {
    public class StateSnapshot : Feature {
        private readonly SortedList<uint, AllHeroSnapshot> characterSnapshots = new();
        private int stateSnapshotCountdown = 0;

        protected override void OnCreate() {
            BattleInitializer.OnGUIContent += () => {
                if (GUILayout.Button("Save World")) {
                    SaveWorld();
                }

                if (GUILayout.Button("Replay World")) {
                    ReplayWorld();
                }
            };
        }

        protected override void OnDestroy() {
            foreach (var data in characterSnapshots.Values) {
                data.Dispose();
            }

            characterSnapshots.Clear();
        }

        protected override void OnTickActive() {
            var snapshot = new AllHeroSnapshot() {
                StateData = new NativeHashMap<int, HeroMoveData>(4, Allocator.Persistent),
                InputData = new NativeHashMap<int, HeroInputData>(4, Allocator.Persistent),
            };
            characterSnapshots.Add(GetTick(), snapshot);

            stateSnapshotCountdown--;
            if (stateSnapshotCountdown < 0) {
                TakeCharacterStateSnapshot(ref snapshot);
                stateSnapshotCountdown = 3;
            }

            TakeCharacterInputSnapshot(ref snapshot);
        }

        private void TakeCharacterStateSnapshot(ref AllHeroSnapshot snapshot) {
            var characterMgr = GetWorldFeature<HeroManager>();
            foreach (var (id, character) in characterMgr.RoleActors) {
                var stateData = character.GetDataReadOnly<HeroMoveData>();
                snapshot.StateData.Add(id, stateData);
            }
        }

        private void TakeCharacterInputSnapshot(ref AllHeroSnapshot snapshot) {
            var characterMgr = GetWorldFeature<HeroManager>();
            foreach (var (id, character) in characterMgr.RoleActors) {
                var inputData = character.GetDataReadOnly<HeroInputData>();
                snapshot.InputData.Add(id, inputData);
            }
        }

        private void SaveWorld() {
            NativeList<S2C_WorldStateSnapshot> worldSnapshots =
                new NativeList<S2C_WorldStateSnapshot>(Allocator.Temp);
            foreach (var (tick, snapshot) in characterSnapshots) {
                var s2cSnapshot = new S2C_WorldStateSnapshot() {
                    Tick = tick,
                    HeroSnapshot = new AllHeroSnapshot() {
                        StateData = new NativeHashMap<int, HeroMoveData>(4, Allocator.Temp),
                        InputData = new NativeHashMap<int, HeroInputData>(4, Allocator.Temp),
                    }
                };

                foreach (var kvp in snapshot.StateData) {
                    var id = kvp.Key;
                    var data = kvp.Value;
                    s2cSnapshot.HeroSnapshot.StateData.Add(id, data);
                }

                foreach (var kvp in snapshot.InputData) {
                    var id = kvp.Key;
                    var data = kvp.Value;
                    s2cSnapshot.HeroSnapshot.InputData.Add(id, data);
                }

                bool isAddSomething = s2cSnapshot.HeroSnapshot.StateData.Count > 0 ||
                                      s2cSnapshot.HeroSnapshot.InputData.Count > 0;
                if (isAddSomething) {
                    worldSnapshots.Add(s2cSnapshot);
                }
            }

            if (worldSnapshots.Length > 0) {
                byte[] packData;
                using (var ms = new MemoryStream())
                using (var bw = new BinaryWriter(ms)) {
                    bw.Write(worldSnapshots.Length);
                    foreach (var snapshot in worldSnapshots) {
                        var data = snapshot.Serialize(new Writer());
                        bw.Write(data.Length);
                        bw.Write(data);
                    }

                    packData = ms.ToArray();
                }

                string filePath = Path.Combine(Application.dataPath, "world_streams.txt");
                File.WriteAllBytes(filePath, packData);
            }

            worldSnapshots.Dispose();
        }

        private void ReplayWorld() {
            var filePath = Path.Combine(Application.dataPath, "world_streams.txt");
            if (!File.Exists(filePath)) {
                Debug.LogError("回放文件不存在:" + filePath);
                return;
            }

            var byteData = File.ReadAllBytes(filePath);
            var startReplayData = new S2C_StartReplay {
                FullData = byteData
            };
            BroadcastAll(startReplayData);
        }
    }
}