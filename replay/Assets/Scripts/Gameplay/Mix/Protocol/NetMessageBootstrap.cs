using System.Reflection;

namespace Gameplay {
    public static class NetMessageBootstrap {
        private static bool _initialized;
        public static readonly NetMessageRegistry ServerRecv = new();
        public static readonly NetMessageRegistry ClientRecv = new();

        public static void Initialize() {
            if (_initialized) return;
            _initialized = true;

            // 自动扫描当前程序集，注册所有带 [MessageId] 的消息
            Assembly asm = typeof(NetMessageBootstrap).Assembly;
            ServerRecv.AutoRegisterFromAssemblies(asm);
            ClientRecv.AutoRegisterFromAssemblies(asm);
        }

        public static void Dispose() {
            ServerRecv.CleanAll();
            ClientRecv.CleanAll();
            _initialized = false;
        }
    }
}