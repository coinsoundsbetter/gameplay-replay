using UnityEngine;

namespace KillCam.Client {
    public class CharacterInput : Capability {
        protected override void OnDeactivate() {
            ref var data = ref Owner.GetDataReadWrite<CharacterInputData>();
            data.Move = new Vector2Int(0, 0);
        }

        public override bool OnShouldActivate() {
            if (World.HasFlag(WorldFlag.Replay)) {
                return false;
            }

            return true;
        }

        protected override void OnTickActive() {
            ref var data = ref Owner.GetDataReadWrite<CharacterInputData>();
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");
            if (h > 0) h = 1;
            else if (h < 0) h = -1;
            if (v > 0) v = 1;
            else if (v < 0) v = -1;
            data.Move = new Vector2Int((int)h, (int)v);
            data.MouseX = Input.GetAxis("Mouse X");
            World.Send(new C2S_SendInput() {
                LocalTick = World.Network.GetTick(),
                Move = data.Move,
            });
        }
    }
}