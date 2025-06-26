using Unity.Entities;

namespace KillCam {
    internal class Server {
        private World world;

        internal void Init() {
            // 创建服务器世界
            world = new World("server");
            
            // Unity 初始化系统组
            var initializeGroup = world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            initializeGroup.AddSystemToUpdateList(world.GetOrCreateSystem<UpdateWorldTimeSystem>());
            initializeGroup.AddSystemToUpdateList(world.GetOrCreateSystem<S_InitializeSystem>());
            
            // Unity 模拟系统组
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            
            // 自定义子组 逻辑状态固定间隔更新
            var stateGroup = world.GetOrCreateSystemManaged<S_StateGroup>();
            stateGroup.SetRateManagerCreateAllocator(new RateUtils.FixedRateCatchUpManager(0.016f));
            stateGroup.AddSystemToUpdateList(world.GetOrCreateSystem<S_LocalTickSystem>());
            stateGroup.AddSystemToUpdateList(world.GetOrCreateSystem<S_HandleNetworkMsgSystem>());
            stateGroup.AddSystemToUpdateList(world.GetOrCreateSystem<S_SpawnPlayerSystem>());
           // stateGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MoveStateSystem>());
            stateGroup.AddSystemToUpdateList(world.GetOrCreateSystem<StateSnapshotSystem>());
            stateGroup.AddSystemToUpdateList(world.GetOrCreateSystem<S_ConnectSystem>());
            stateGroup.AddSystemToUpdateList(world.GetOrCreateSystem<S_LoginSystem>());
            stateGroup.AddSystemToUpdateList(world.GetOrCreateSystem<S_RpcSystem>());
            simulationGroup.AddSystemToUpdateList(stateGroup);
            
            // 开始循环
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(world);
        }

        internal void Clear() {
            world.Dispose();
            ScriptBehaviourUpdateOrder.RemoveWorldFromCurrentPlayerLoop(world);
        }
    }
}