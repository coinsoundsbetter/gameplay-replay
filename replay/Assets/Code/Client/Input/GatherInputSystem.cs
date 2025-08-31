using UnityEngine;

namespace KillCam.Client {
    /// <summary>
    /// 采集客户端输入
    /// </summary>
    public class GatherInputSystem : SystemBase {
        
        protected override void OnCreate() {
            CreateData<UserInputState>();
        }

        protected override void OnTick() {
            if (!TryGetData(out NetworkData networkData)) {
                return;
            }
            
            ref var inputState = ref GetDataRef<UserInputState>();
            var h = Input.GetAxisRaw("Horizontal");
            var v = Input.GetAxisRaw("Vertical");
            h = h switch {
                > 0 => 1,
                < 0 => -1,
                _ => 0
            };
            v = v switch {
                > 0 => 1,
                < 0 => -1,
                _ => 0
            };
            inputState.Move = new Vector2Int((int)h, (int)v);
            inputState.Yaw = Input.GetAxis("Mouse X");
            inputState.Pitch = Input.GetAxis("Mouse Y");
            inputState.IsFirePressed = Input.GetButton("Fire1");
            
            Send(new C2S_UserInputCmd() {
                LocalTick = networkData.Tick,
                InputState = inputState,
            });
        }
    }
}