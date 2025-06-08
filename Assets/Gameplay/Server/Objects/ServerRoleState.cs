using UnityEngine;

public class ServerRoleState : RoleState {
    private GameObject view;

    public void Init(int gameId) {
        GameId = gameId;
    }
}