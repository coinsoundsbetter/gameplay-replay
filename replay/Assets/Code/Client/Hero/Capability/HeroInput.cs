using UnityEngine;

namespace KillCam.Client {
    public class HeroInput : Capability {
        protected override void OnDeactivate() {
            ref var data = ref Owner.GetDataReadWrite<HeroInputData>();
            data.Move = new Vector2Int(0, 0);
        }

        public override bool OnShouldActivate() {
            if (World.HasFlag(WorldFlag.Replay)) {
                return false;
            }

            return true;
        }

        protected override void OnTickActive() {
            ref var data = ref Owner.GetDataReadWrite<HeroInputData>();
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
            World.Send(new C2S_SendInput() {
                LocalTick = World.GetTick(),
                Move = data.Move,
            });
        }
    }
}