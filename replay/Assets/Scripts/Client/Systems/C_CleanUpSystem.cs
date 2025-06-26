using Unity.Entities;
using UnityEngine;

namespace KillCam
{
    [UpdateInGroup(typeof(C_StateGroup), OrderLast = true)]
    public partial struct C_CleanUpSystem : ISystem 
    {
        public void OnUpdate(ref SystemState state)
        {
            var inputBuffer = SystemAPI.GetSingletonBuffer<InputElement>();
            for (int i = inputBuffer.Length - 1; i >= 0; i--)
            {
                var input = inputBuffer[i];
                if (input.IsApplyed)
                {
                    inputBuffer.RemoveAt(i);
                    Debug.Log("移除输入 " + SystemAPI.GetSingleton<NetTickState>().Local);
                }
            }
        }
    }
}