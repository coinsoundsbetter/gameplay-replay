using FishNet.Serializing;
using Unity.Collections;

namespace KillCam {
    public static class ReadEx {
        public static AllHeroSnapshot ReadAllCharacterSnapshot(this Reader reader) {
            var res = new AllHeroSnapshot();

            int stateDataCnt = reader.ReadInt32();
            if (stateDataCnt > 0) {
                res.StateData = new NativeHashMap<int, HeroMoveData>(4, Allocator.Persistent);
            }

            for (int i = 0; i < stateDataCnt; i++) {
                var id = reader.ReadInt32();
                var stateData = reader.ReadCharacterStateData();
                res.StateData.Add(id, stateData);
            }

            int inputDataCnt = reader.ReadInt32();
            if (inputDataCnt > 0) {
                res.InputData = new NativeHashMap<int, HeroInputData>(4, Allocator.Persistent);
            }

            for (int i = 0; i < stateDataCnt; i++) {
                var id = reader.ReadInt32();
                var inputData = reader.ReadCharacterInputData();
                res.InputData.Add(id, inputData);
            }

            return res;
        }

        public static HeroMoveData ReadCharacterStateData(this Reader reader) {
            var res = new HeroMoveData {
                Pos = reader.ReadVector3(),
                Rot = reader.ReadQuaternion64()
            };
            return res;
        }

        public static HeroInputData ReadCharacterInputData(this Reader reader) {
            var res = new HeroInputData() {
                Move = reader.ReadVector2Int(),
            };

            return res;
        }
    }
}