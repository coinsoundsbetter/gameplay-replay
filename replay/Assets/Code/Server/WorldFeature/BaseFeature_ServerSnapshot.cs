using System.Collections.Generic;

namespace KillCam.Server {
    public class BaseFeature_ServerSnapshot : Feature {
        private readonly SortedList<uint, Dictionary<int, RoleStateSnapshot>> roleSnapshots = new();
        
        public override void OnCreate() {
            world.AddLogicUpdate(OnLogicTick);
        }

        public override void OnDestroy() {
            world.RemoveLogicUpdate(OnLogicTick);
        }

        private void OnLogicTick(double delta) {
            TakeRoleSnapshot();
        }

        private void TakeRoleSnapshot() {
            // 创建该逻辑帧的角色状态快照列表
            var stateSnapshots = new Dictionary<int, RoleStateSnapshot>(8);
            roleSnapshots.Add(GetTick(), stateSnapshots);
            
            // 填充快照信息
            var dict = Get<BaseFeature_ServerSpawn>().RoleLogics;
            foreach (var kvp in dict)
            {
                stateSnapshots.Add(kvp.Key, kvp.Value.GetNetStateData());
            }
        }

        public void SerializeWorldState()
        {
            
        }
    }
}