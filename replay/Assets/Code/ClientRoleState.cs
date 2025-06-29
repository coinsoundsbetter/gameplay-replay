using UnityEngine;

public class ClientRoleState
{
    /*public override void OnStartClient()
    {
        TimeManager.OnTick += OnTick;
    }

    public override void OnStopClient()
    {
        TimeManager.OnTick -= OnTick;
    }

    private void OnTick()
    {
        ReceiveInput();
        SimulateMove(MoveInput, GetTickDelta());
    }

    private void ReceiveInput()
    {
        var h = Input.GetAxis("Horizontal");    
        var v = Input.GetAxis("Vertical");
        if (h > 0) h = 1; 
        else if (h < 0) h = -1;
        if (v > 0) v = 1; 
        else if (v < 0) v = -1;

        MoveInput = new Vector2(h, v);
        
        Send(new C2S_SendInput()
        {
            LocalTick = GetLocalTick(),
            Move = new Vector2(h, v),
        });
    }*/
}