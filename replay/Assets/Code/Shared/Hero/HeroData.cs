using System;
using Unity.Collections;
using UnityEngine;

namespace KillCam {
    public struct HeroIdentifier {
        public bool IsControlTarget;
        public int PlayerId;
        public FixedString64Bytes PlayerName;
    }
    
    public struct AllHeroSnapshot : IDisposable {
        public NativeHashMap<int, HeroMoveData> StateData;
        public NativeHashMap<int, HeroInputData> InputData;

        public bool IsEmpty() => StateData.IsEmpty && InputData.IsEmpty;

        public void Dispose() {
            StateData.Dispose();
            InputData.Dispose();
        }
    }
    
    public struct HeroSkinData {
        public FixedString64Bytes SkinSign;
    }
    
    public struct HeroInputData {
        public Vector2Int Move;
        public float Yaw;
        public float Pitch;
        public bool IsFirePressed;
        public bool HasValidInput() => Move != default;
    }
    
    public struct HeroMoveData {
        public Vector3 Pos;
        public Quaternion Rot;
        public bool IsMoving;
        public Vector3 WorldMoveDirection;
        public Vector3 LocalMoveDirection;
    }

    public struct HeroFireData {
        public float FireCd;
        public uint FireId;
    }

    public struct HeroAnimData {
        public bool IsAnimMoving;
    }

    public struct HeroHealth {
        public float Health;
    }
    
    public class UnityHeroLink {
        public IUnityHero Actor;
    }

    public class ClientHeroNetLink {
        public IClientHeroNet NetClient;
    }

    public class ServerHeroNetLink {
        public IServerHeroNet NetServer;
    }
    
    public static class HeroAnimKeys {
        public const string IsMove = "IsMove";
        public const string MoveDirX = "MoveDirX";
        public const string MoveDirZ = "MoveDirZ";
    }
}