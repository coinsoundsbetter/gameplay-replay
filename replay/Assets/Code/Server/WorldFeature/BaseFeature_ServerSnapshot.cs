using System.Collections.Generic;

namespace KillCam.Server {
    public class BaseFeature_ServerSnapshot : Feature {
        private SortedList<int, RoleStateSnapshot> roleSnapshots = new();
        
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
            var dict = Get<BaseFeature_ServerSpawn>().RoleLogics;
        }
    }
}