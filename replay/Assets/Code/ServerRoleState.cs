using System.Collections.Generic;
using FishNet.Serializing;

public class ServerRoleState : RoleState
{
    private List<C2S_SendInput> inputBuffer = new List<C2S_SendInput>();
    
    public override void OnStartServer()
    {
        TimeManager.OnTick += OnTick;
        TimeManager.OnPostTick += OnPostTick;
    }

    public override void OnStopServer()
    {
        TimeManager.OnTick -= OnTick;    
        TimeManager.OnPostTick -= OnPostTick;
    }
    
    private void OnTick()
    {
        
    }
    
    private void OnPostTick()
    {
        if (inputBuffer.Count > 0)
        {
            var useInput = inputBuffer[0];
            inputBuffer.RemoveAt(0);
            SimulateMove(useInput.Move);
        }
    }

    protected override void OnServerReceived(byte[] data)
    {
        var reader = new Reader(data, null);
        var msgType = (NetworkMsg)reader.ReadUInt16();
        switch (msgType)
        {
            case NetworkMsg.C2S_SendInput:
                var msg1 = new C2S_SendInput();
                msg1.Deserialize(reader);
                inputBuffer.Add(msg1);
                break;
        }
    }
}