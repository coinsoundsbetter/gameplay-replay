using System;
using System.Collections.Generic;

namespace Gameplay {
    public static class NetMessageBoostrap {
        private static bool _initialized;

        public static void Initialize() {
            if (_initialized) return;
            _initialized = true;

            // C → S 消息
            NetMessageRegistry.Register<Protocol.CreatePlayer>(1001);

            // S → C 消息
        }

        public static void Dispose() {
            NetMessageRegistry.CleanAll();
        }
    }
    
    public static class NetMessageRegistry {
        private static readonly Dictionary<int, Func<INetworkMessage>> _factories = new();
        private static readonly Dictionary<Type, int> _typeToId = new();

        /// <summary>
        /// 注册一个消息类型
        /// </summary>
        public static void Register<T>(int id) where T : INetworkMessage, new() {
            if (_factories.ContainsKey(id))
                throw new Exception($"重复的消息ID: {id}");

            _factories[id] = () => new T();
            _typeToId[typeof(T)] = id;
        }

        public static void CleanAll() {
            _factories.Clear();
            _typeToId.Clear();
        }

        /// <summary>
        /// 获取消息类型的 ID
        /// </summary>
        public static int GetId<T>() where T : INetworkMessage {
            return _typeToId[typeof(T)];
        }

        /// <summary>
        /// 通过 ID 创建消息实例
        /// </summary>
        public static INetworkMessage CreateMessage(int id) {
            if (_factories.TryGetValue(id, out var ctor))
                return ctor();

            throw new Exception($"未知的消息ID: {id}");
        }
    }
}