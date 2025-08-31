using System;
using Gameplay.Core;

public abstract class Capability
{
    public Actor Owner { get; private set; }
    public World World => WorldRegistry.GetWorldById(Owner.WorldId);

    /// <summary>
    /// 指定该能力应该挂在哪个 SystemGroup 里更新
    /// （由装配时设置，默认为 null = 不调度）
    /// </summary>
    internal Type SystemGroupType { get; set; }

    public bool Enabled { get; private set; } = true;

    internal void Attach(Actor actor, Type? groupType)
    {
        Owner = actor;
        SystemGroupType = groupType;
        OnAttach();
    }

    internal void Detach()
    {
        OnDetach();
    }

    protected void SetEnabled(bool enabled)
    {
        if (Enabled == enabled) return;
        Enabled = enabled;
        if (Enabled) OnEnabled();
        else OnDisabled();
    }

    internal void Tick(double deltaTime)
    {
        if (Enabled) OnUpdate(deltaTime);
    }

    // 生命周期钩子
    protected virtual void OnAttach() { }
    protected virtual void OnDetach() { }
    protected virtual void OnEnabled() { }
    protected virtual void OnDisabled() { }
    protected virtual void OnUpdate(double deltaTime) { }
}