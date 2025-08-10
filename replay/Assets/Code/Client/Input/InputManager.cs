using UnityEngine;

namespace KillCam.Client {
    public class InputManager : Feature {
        
        public override bool OnShouldActivate() {
            return Owner.HasData<UserInputData>();
        }

        protected override void OnTickActive() {
            ref var data = ref Owner.GetDataReadWrite<UserInputData>();
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
            data.IsFirePressed = Input.GetButton("Fire1");
        }
    }
}