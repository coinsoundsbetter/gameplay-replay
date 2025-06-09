using UnityEngine;

public class ClientInputs : Feature, IUpdateable {
    private ClientGameState gameState;
    private ClientNetworking networking;
    private ClientRoleRegistry roleRegistry;
    
    public override void OnInitialize(ref WorldLink link) {
        networking = link.RequireFeature<ClientNetworking>();
        roleRegistry = link.RequireFeature<ClientRoleRegistry>();
    }

    public void OnUpdate() {
        RoleMoveInput();
    }

    private void RoleMoveInput() {
        if (gameState.CurrentGameState != GameState.Gaming) {
            return;
        }
        
        /*if (!roleRegistry.IsLocalInitialized) {
            return;
        }
        
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (moveInput.x > 0) {
            moveInput.x = 1;
        }else if (moveInput.x < 0) {
            moveInput.x = -1;
        }
        if (moveInput.y > 0) {
            moveInput.y = 1;
        }else if(moveInput.y < 0) {
            moveInput.y = -1;
        }
        networking.BroadcastToServer(new FishBroadcastDefine.InputInfo() {
            GameId = roleRegistry.LocalNet.RoleId.Value,
            Move = moveInput,
        });*/
    }
}