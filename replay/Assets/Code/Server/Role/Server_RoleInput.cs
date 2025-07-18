using System.Collections.Generic;
using UnityEngine;

namespace KillCam.Server
{
    public class Server_RoleInput
    {
        private readonly SortedList<uint, InputData> inputBuffer = new();
        private BattleWorld battleWorld;
        private Server_RoleMovement movement;
        public C2S_SendInput UseInputData;

        private struct InputData
        {
            public float addTime;
            public C2S_SendInput data;
        }

        public Server_RoleInput(Server_RoleMovement movement, BattleWorld world)
        {
            this.movement = movement;
            battleWorld = world;
        }
        
        public void Update(double delta)
        {
            if (inputBuffer.Count > 0)
            {
                var firstKey = inputBuffer.Keys[0];
                var useInput = inputBuffer[firstKey];
                inputBuffer.Remove(firstKey);
                Debug.Log(" " + useInput.data.Move);
                movement.ApplyInput(useInput.data.Move, (float)delta);
                UseInputData = useInput.data;
            }
            else
            {
                UseInputData = new C2S_SendInput();
            }
        }

        public void AddInput(C2S_SendInput inputData)
        {
            var nowTime = Time.unscaledTime;
            inputBuffer.Add(inputData.LocalTick, new InputData()
            {
                addTime = nowTime,
                data = inputData,
            });

            // 移除时间过长的输入
            bool isRemoveInvalid = false;
            while (!isRemoveInvalid)
            {
                var checkTime = inputBuffer.Values[0].addTime;
                if (nowTime - checkTime > 1f)
                {
                    inputBuffer.Remove(inputBuffer.Keys[0]);
                }
                else
                {
                    isRemoveInvalid = true;
                }
            }
        }
    }
}