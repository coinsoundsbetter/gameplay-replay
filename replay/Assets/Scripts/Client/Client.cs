using Unity.Entities;

namespace KillCam {
    internal class Client {
        private World world;

        internal void Init() {
            // 创建客户端世界
            world = new World("client");
            
            // Unity 初始化系统组,一些全局的单例可以在这里注册
            var initializeGroup = world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            initializeGroup.AddSystemToUpdateList(world.GetOrCreateSystem<C_InitializeSystem>());
            
            // Unity 模拟系统组
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            
            // 自定义子组 的网络消息处理组
            var receiveNetEventGroup = world.GetOrCreateSystemManaged<C_ReceiveNetEventGroup>();
            receiveNetEventGroup.AddSystemToUpdateList(world.GetOrCreateSystem<C_HandleNetworkMsgSystem>());
            receiveNetEventGroup.AddSystemToUpdateList(world.GetOrCreateSystem<C_SpawnPlayerSystem>());
            simulationGroup.AddSystemToUpdateList(receiveNetEventGroup);
            
            // 自定义子组 逻辑状态系统组,固定间隔更新,在这个组里的都需要有支持预测跟回滚的能力
            var stateGroup = world.GetOrCreateSystemManaged<C_StateGroup>();
            stateGroup.SetRateManagerCreateAllocator(new RateUtils.FixedRateCatchUpManager(0.016f));
            stateGroup.AddSystemToUpdateList(world.GetOrCreateSystem<C_LocalTickSystem>());
            stateGroup.AddSystemToUpdateList(world.GetOrCreateSystem<C_InputSystem>());
            stateGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MoveStateSystem>());
            stateGroup.AddSystemToUpdateList(world.GetOrCreateSystem<StateSnapshotSystem>());
            simulationGroup.AddSystemToUpdateList(stateGroup);
            
            // 表现系统组,平滑逻辑状态带来的表现
            var presentationGroup = world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            presentationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<C_VisualMoveSystem>());
            
            // 自定义子组 网络消息发送组
            var sendNetEventGroup = world.GetOrCreateSystemManaged<C_SendNetEventGroup>();
            sendNetEventGroup.AddSystemToUpdateList(world.GetOrCreateSystem<C_ConnectSystem>());
            sendNetEventGroup.AddSystemToUpdateList(world.GetOrCreateSystem<C_LoginSystem>());
            sendNetEventGroup.AddSystemToUpdateList(world.GetOrCreateSystem<C_SendNetMsgSystem>());
            presentationGroup.AddSystemToUpdateList(sendNetEventGroup);
            
            // 开始循环
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(world);
        }

        internal void Clear() {
            ScriptBehaviourUpdateOrder.RemoveWorldFromCurrentPlayerLoop(world);
            world.Dispose();
        }
    }
}