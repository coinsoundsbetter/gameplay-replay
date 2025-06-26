using Unity.Entities;
using UnityEngine;

namespace KillCam
{
    public partial struct C_CreateInputSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LocalConnectState>();
            state.RequireForUpdate<NetChannels>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var localConnState = SystemAPI.GetSingleton<LocalConnectState>();
            if (localConnState.State != NetConnectState.Connected)
            {
                return;
            }
            
            // 输入缓冲区
            var inputElement = new InputElement()
            {
                IsAvailable = true,
            };
            var moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (moveInput.x > 0)
            {
                moveInput.x = 1;
            }
            else if (moveInput.x < 0)
            {
                moveInput.x = -1;
            }
            if (moveInput.y > 0)
            {
                moveInput.y = 1;
            }
            else if(moveInput.y < 0)
            {
                moveInput.y = -1;
            }
            inputElement.Move = moveInput;
            if (inputElement.Move != Vector2.zero)
            {
                SystemAPI.GetSingletonBuffer<InputElement>().Add(inputElement);
            }
            
            // 发送给服务器
            var net = SystemAPI.ManagedAPI.GetSingleton<NetSend>();
            net.Add(new C2S_InputElement()
            {
                Data = inputElement,
            });
        }
    }
}