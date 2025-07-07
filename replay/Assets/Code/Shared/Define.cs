using FishNet.Serializing;

namespace KillCam
{
    /// <summary>
    /// 用于启动完整的功能
    /// 这样,我们可以只在需要的时机启动一些功能
    /// </summary>
    public class InitializeFeature : Feature { }
    
    /// <summary>
    /// 世界范围的功能继承它
    /// </summary>
    public class Feature
    {
        protected BattleWorld world { get; private set; }

        public void SetWorld(BattleWorld w)
        {
            world = w;
        }

        public virtual void OnCreate() { }
        public virtual void OnDestroy() { }

        protected T Get<T>() where T : Feature 
        {
            return world.Get<T>();
        }

        protected uint GetTick() => world.Network.GetTick();
    }
    
    public interface INetworkSerialize
    {
        byte[] Serialize(Writer writer);
        void Deserialize(Reader reader);
        NetworkMsg GetMsgType();
    }
}