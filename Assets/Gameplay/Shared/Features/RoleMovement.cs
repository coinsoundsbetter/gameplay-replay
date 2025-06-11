using System.Collections.Generic;
using UnityEngine;

public class RoleMovement : Feature {
    protected Queue<(int, Vector2)> waitHandleInputs = new Queue<(int, Vector2)>();
    
    public override void OnInitialize(ref WorldLink link) {
        base.OnInitialize(ref link);
    }

    public void ApplyRoleInput(int roleId, Vector2 input) {
        waitHandleInputs.Enqueue((roleId, input));
    }
}
