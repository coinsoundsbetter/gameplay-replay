using UnityEngine;

namespace KillCam.Client
{
    public class Client_CharacterInput : Capability
    {
        protected override void OnDeactivate()
        {
            ref var data = ref Owner.GetDataReadWrite<CharacterInputData>();
            data.MoveInput = new Vector2Int(0, 0);
        }

        protected override void OnTickActive()
        {
            ref var data = ref Owner.GetDataReadWrite<CharacterInputData>();
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");
            if (h > 0) h = 1;
            else if (h < 0) h = -1;
            if (v > 0) v = 1;
            else if (v < 0) v = -1;
            data.MoveInput = new Vector2Int((int)h, (int)v);
            World.Send(new C2S_SendInput()
            {
                LocalTick = World.Network.GetTick(),
                Move = data.MoveInput,
            });
        }
    }
}