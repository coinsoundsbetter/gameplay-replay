using Unity.Entities;

namespace KillCam {
    internal class Server {
        private World world;

        internal void Init() {
            // 创建服务器世界
            world = new World("server");
            
            // Unity 初始化系统组
            var initializeGroup = world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            initializeGroup.AddSystemToUpdateList(world.GetOrCreateSystem<S_InitializeSystem>());
            initializeGroup.AddSystemToUpdateList(world.GetOrCreateSystem<S_ConnectSystem>());
            initializeGroup.AddSystemToUpdateList(world.GetOrCreateSystem<S_LoginSystem>());
            
            // Unity 模拟系统组
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            
            // 自定义子组 处理网络事件
            var receiveNetMsgGroup = world.GetOrCreateSystemManaged<S_ReceiveNetMsgGroup>();
            receiveNetMsgGroup.AddSystemToUpdateList(world.GetOrCreateSystem<S_HandleNetworkMsgSystem>());
            receiveNetMsgGroup.AddSystemToUpdateList(world.GetOrCreateSystem<S_SpawnPlayerSystem>());
            simulationGroup.AddSystemToUpdateList(receiveNetMsgGroup);
            
            // 自定义子组 逻辑状态固定间隔更新
            var stateGroup = world.GetOrCreateSystemManaged<S_StateGroup>();
            stateGroup.SetRateManagerCreateAllocator(new RateUtils.FixedRateCatchUpManager(0.016f));
            stateGroup.AddSystemToUpdateList(world.GetOrCreateSystem<C_LocalTickSystem>());
            stateGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MoveStateSystem>());
            stateGroup.AddSystemToUpdateList(world.GetOrCreateSystem<StateSnapshotSystem>());
            simulationGroup.AddSystemToUpdateList(stateGroup);
            
            // Unity 后模拟系统组
            var lateSimulateGroup = world.GetOrCreateSystemManaged<LateSimulationSystemGroup>();
            
            // 自定义子组 发送网络事件
            var sendNetMsgGroup = world.GetOrCreateSystemManaged<S_RpcNetMsgGroup>();
            sendNetMsgGroup.AddSystemToUpdateList(world.GetOrCreateSystem<S_RpcSystem>());
            lateSimulateGroup.AddSystemToUpdateList(sendNetMsgGroup);
            
            // 开始循环
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(world);
        }

        internal void Clear() {
            world.Dispose();
            ScriptBehaviourUpdateOrder.RemoveWorldFromCurrentPlayerLoop(world);
        }
    }
}