using FishNet.Object.Synchronizing;
using UnityEngine;

public class RoleNet : NetworkObj {
    public readonly SyncVar<int> RoleId = new (new SyncTypeSettings(WritePermission.ServerOnly));
    public readonly SyncVar<string> GameName = new (new SyncTypeSettings(WritePermission.ServerOnly));
    public readonly SyncVar<Vector3> Pos = new (new SyncTypeSettings(WritePermission.ServerOnly));
    public readonly SyncVar<Quaternion> Rot = new (new SyncTypeSettings(WritePermission.ServerOnly));
}