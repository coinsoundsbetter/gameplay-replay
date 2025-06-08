using FishNet.Broadcast;
using UnityEngine;

public class FishBroadcastDefine {
    
    public struct LoginRequest : IBroadcast {
        public string UserName;
        public string Token;
    }
    
    public struct LoginResult : IBroadcast{
        public bool IsSuccess;
    }
    
    public struct InputInfo : IBroadcast {
        public int GameId;
        public Vector2 Move;
    }
}