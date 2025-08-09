using UnityEngine;

namespace KillCam.Client {
    public class InputManager : Feature {
        
        public override bool OnShouldTick() {
            return Owner.HasData<InputData>();
        }

        protected override void OnTickActive() {
            ref var data = ref Owner.GetDataReadWrite<InputData>();
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
            data.Move = new Vector2Int((int)h, (int)v);
            data.Yaw = Input.GetAxis("Mouse X");
            data.Pitch = Input.GetAxis("Mouse Y");
        }
    }
}