using System.Collections.Generic;

namespace Gameplay.Core {
    public static class WorldRegistry {
        private static readonly Dictionary<int, World> _worlds = new();

        public static void Register(World w) {
            _worlds[w.WorldId] = w; // ✅ 覆盖写入，保证唯一
        }

        public static void Unregister(World w) {
            _worlds.Remove(w.WorldId);
        }

        /// <summary>根据 Id 获取 World，没有则返回 null</summary>
        public static World GetWorldById(int id) {
            _worlds.TryGetValue(id, out var world);
            return world;
        }

        /// <summary>尝试获取 World，更安全</summary>
        public static bool TryGetWorld(int id, out World world) {
            return _worlds.TryGetValue(id, out world);
        }

        /// <summary>获取所有已注册的 World</summary>
        public static IEnumerable<World> GetAllWorlds() {
            return _worlds.Values; // ⚠️ 遍历仍会产生枚举器 GC，如果要完全无 GC，可以改 for 循环
        }
    }
}