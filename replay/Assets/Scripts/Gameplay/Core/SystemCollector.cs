namespace Gameplay.Core {
    using System;
    using System.Linq;
    using System.Reflection;

    public static class SystemCollector {
        
        public static void CollectInto<T>(T group, WorldFlag world) where T : SystemGroup
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var allTypes = allAssemblies
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null)!; }
                })
                .Where(t => typeof(ISystem).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var type in allTypes)
            {
                // 跳过禁用收集
                if (type.GetCustomAttribute<DisableCollectorAttribute>() != null)
                    continue;

                // 世界过滤
                var wf = type.GetCustomAttribute<WorldFilterAttribute>();
                if (wf != null)
                {
                    if (wf.All != WorldFlag.Default && (world & wf.All) != wf.All) continue;
                    if (wf.Any != WorldFlag.Default && (world & wf.Any) == 0) continue;
                    if (wf.None != WorldFlag.Default && (world & wf.None) != 0) continue;
                }

                // 检查是否在这个 Group 内
                var inGroup = type.GetCustomAttributes<UpdateInGroupAttribute>()
                    .Any(a => a.GroupType == typeof(T));
                if (!inGroup) continue;

                if (group.Systems.Any(s => s.GetType() == type))
                    continue;

                var instance = (ISystem)Activator.CreateInstance(type)!;
                group.AddSystem(instance);
            }

            group.SortSystems();
        }
    }
}