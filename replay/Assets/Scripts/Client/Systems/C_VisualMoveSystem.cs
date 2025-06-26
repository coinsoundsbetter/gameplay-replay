using Unity.Entities;
using UnityEngine;

namespace KillCam {
    public partial struct C_VisualMoveSystem : ISystem {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetTickState>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (identifier, movementState, entity) in SystemAPI
                         .Query<RefRO<PlayerIdentifier>, RefRO<PlayerMovementState>>()
                         .WithEntityAccess())
            {
                // 为本地玩家执行移动,它总是在插值向目标位置
                if (identifier.ValueRO.IsLocalPlayer)
                {
                    var view = SystemAPI.ManagedAPI.GetComponent<PlayerView>(entity).Binder;
                    if (view != null)
                    {
                        var lerpPos = Vector3.MoveTowards(view.GetPos(), movementState.ValueRO.Pos, 1.0f / 60);
                        view.SetPos(lerpPos);
                    }
                }
            }
        }
    }
}