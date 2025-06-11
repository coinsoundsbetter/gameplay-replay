using System.Collections.Generic;
using UnityEngine;

public class ServerRoleMovement : RoleMovement, IUpdateable {
    private ServerRoleRegistry registry;
    
    
    public override void OnInitialize(ref WorldLink link) {
        registry = link.RequireFeature<ServerRoleRegistry>();
    }

    public void OnUpdate() {
        var roleStates = registry.GameIdToRoleStates;
        while (waitHandleInputs.Count > 0) {
            (int, Vector2) input = waitHandleInputs.Dequeue();
            if (!roleStates.TryGetValue(input.Item1, out var net)) {
                continue;
            }
            
            var inputValue = input.Item2;
            var dir = net.Rot * Vector3.forward;
            var nextPos = net.Pos + dir * inputValue.y * Time.deltaTime * 3f;
            net.Pos = nextPos;
        }
    }

    public void ApplyRoleInput(Vector2 moveInput) {
        
    }
}