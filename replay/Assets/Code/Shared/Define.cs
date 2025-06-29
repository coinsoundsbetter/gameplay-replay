namespace KillCam
{
    /// <summary>
    /// 用于启动完整的功能
    /// 这样,我们可以只在需要的时机启动一些功能
    /// </summary>
    public class InitializeFeature : Feature { }
    
    /// <summary>
    /// 常规的功能继承它
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
    }
}