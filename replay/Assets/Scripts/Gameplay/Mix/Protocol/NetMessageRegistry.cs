using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gameplay {
    public sealed class NetMessageRegistry {
        private readonly Dictionary<int, Func<INetworkMessage>> _factories = new();
        private readonly Dictionary<Type, int> _typeToId = new();

        public void Register<T>(int id) where T : struct, INetworkMessage
            => RegisterInternal(typeof(T), id);

        public void Register<T>() where T : struct, INetworkMessage {
            var type = typeof(T);
            var attr = type.GetCustomAttribute<MessageIdAttribute>();
            if (attr == null)
                throw new Exception($"类型 {type.FullName} 没有标记 [MessageId]");
            RegisterInternal(type, attr.Id);
        }

        private void RegisterInternal(Type t, int id) {
            if (_factories.ContainsKey(id))
                throw new Exception($"重复的消息ID: {id}");

            _factories[id] = () => (INetworkMessage)Activator.CreateInstance(t)!;
            _typeToId[t] = id;
        }

        public void CleanAll() {
            _factories.Clear();
            _typeToId.Clear();
        }

        public int GetId<T>() where T : INetworkMessage {
            if (_typeToId.TryGetValue(typeof(T), out var id))
                return id;
            throw new Exception($"类型未注册: {typeof(T).FullName}");
        }

        public INetworkMessage CreateMessage(int id) {
            if (_factories.TryGetValue(id, out var ctor))
                return ctor();
            throw new Exception($"未知的消息ID: {id}");
        }

        /// <summary>
        /// 自动扫描并注册所有带 [MessageId] 的消息
        /// </summary>
        public void AutoRegisterFromAssemblies(params Assembly[] assemblies) {
            if (assemblies == null || assemblies.Length == 0)
                assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var asm in assemblies) {
                Type[] types;
                try {
                    types = asm.GetTypes();
                }
                catch (ReflectionTypeLoadException e) {
                    types = e.Types.Where(t => t != null).ToArray();
                }

                foreach (var t in types) {
                    if (t.IsAbstract) continue;
                    if (!typeof(INetworkMessage).IsAssignableFrom(t)) continue;

                    var attr = t.GetCustomAttribute<MessageIdAttribute>();
                    if (attr == null) continue;

                    RegisterInternal(t, attr.Id);
                }
            }
        }
    }
}