using UnityEngine;

public class ServerEventHandler : Feature {
    private ServerRoleMovement movement;
    
    public override void OnInitialize(ref WorldLink link) {
        movement = link.RequireFeature<ServerRoleMovement>();
    }

    public void OnRoleInput(int roleId, Vector2 moveInput) {
        movement.ApplyRoleInput(roleId, moveInput);
    }
}