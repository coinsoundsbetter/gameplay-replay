using Gameplay.Core;
using UnityEngine;

namespace Gameplay.Client {
    /// <summary>
    /// 客户端角色生成系统
    /// </summary>
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    [SystemTag(SystemFlag.Client, SystemFilterMode.Strict)]
    public class Client_SpawnHeroSystem : SystemBase {
        
        protected override void OnCreate() {
            
        }
    }
}