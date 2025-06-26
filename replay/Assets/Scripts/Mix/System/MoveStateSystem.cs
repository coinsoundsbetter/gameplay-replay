using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KillCam
{
    public partial struct MoveStateSystem : ISystem 
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InputElement>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var cmd = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (identifier, movementState, entity) in SystemAPI
                         .Query<RefRO<PlayerIdentifier>, RefRW<PlayerMovementState>>()
                         .WithEntityAccess())
            {
                if (identifier.ValueRO.IsLocalPlayer)
                {
                    var inputBuffer = SystemAPI.GetSingletonBuffer<InputElement>();
                    if (inputBuffer.Length == 0)
                    {
                        break;
                    }

                    ref var waitInput = ref inputBuffer.ElementAt(0);
                    if (!waitInput.IsAvailable)
                    {
                        break;
                    }

                    waitInput.IsApplyed = true;

                    var moveForward = movementState.ValueRO.Rot * Vector3.forward;
                    var moveRight = movementState.ValueRO.Rot * Vector3.right;
                    var motion = (moveForward.normalized * waitInput.Move.y + moveRight.normalized * waitInput.Move.x) * 5f * SystemAPI.Time.DeltaTime;
                    movementState.ValueRW.Pos += motion;
                    Debug.Log("应用输入 " + SystemAPI.GetSingleton<NetTickState>().Local);
                }
            }
            
            cmd.Playback(state.EntityManager);
            cmd.Dispose();
        }
    } 
}