using Gameplay.Core;
using UnityEngine;

namespace Gameplay.Client {
    /// <summary>
    /// 客户端角色生成系统
    /// </summary>
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    [WorldFilter(Any = WorldFlag.Client, None = WorldFlag.Replay)]
    public class Client_SpawnHeroSystem : ISystem {
        public void Update(ref SystemState state) {
            Debug.Log("1");
        }
    }
}