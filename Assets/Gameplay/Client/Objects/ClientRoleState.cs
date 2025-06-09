using UnityEngine;

public class ClientRoleState : RoleState {
    private UnityRole unityRole;

    public void Init(int gameId) {
        RoleId = gameId;
        CreateUnityView();
    }

    public void Clear() {
        
    }

    private void CreateUnityView() {
        var asset = Resources.Load("UnityView/UnityRole");
        var instance = (GameObject)Object.Instantiate(asset);
        unityRole = instance.GetComponent<UnityRole>();
    }

    public void SetPos(Vector3 pos) {
        if (unityRole == null) {
            return;
        }
        
        unityRole.transform.position = pos;
    }
}