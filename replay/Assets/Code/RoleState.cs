using System;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Object.Synchronizing;
using FishNet.Serializing;
using UnityEngine;

public class RoleState : NetworkBehaviour, ISerializeAs<RoleStateSnapshot>
{
    public readonly SyncVar<int> Id = new SyncVar<int>();
    public readonly SyncVar<Vector3> Pos = new SyncVar<Vector3>();
    public readonly SyncVar<Quaternion> Rot = new SyncVar<Quaternion>();
    public readonly SyncVar<int> Health = new SyncVar<int>();

    private double tickDelta;
    private Vector2 moveInput;

    public override void OnStartNetwork()
    {
        TimeManager.OnPostTick += OnTick;
    }

    public override void OnStopNetwork()
    {
        TimeManager.OnPostTick -= OnTick;
    }
    
    public struct ReplicatedData : IReconcileData
    {
        public int Id;
        public Vector3 Pos;
        public Quaternion Rot;
        public int Health;
        
        private uint _tick;
        
        public uint GetTick()
        {
            return _tick;
        }

        public void SetTick(uint value)
        {
            _tick = value;
        }

        public void Dispose() { }
    }
    
    public struct ReconcileData : IReconcileData
    {
        
        private uint _tick;
        
        public uint GetTick()
        {
            return _tick;
        }

        public void SetTick(uint value)
        {
            _tick = value;
        }

        public void Dispose() { }
    }

    public override void CreateReconcile()
    {
        base.CreateReconcile();
    }

    private void OnTick()
    {
        tickDelta = TimeManager.TickDelta;
        
        // 客户端输入采样
        if (IsClientInitialized)
        {
            Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (move.x > 0) move.x = 1;
            if (move.x < 0) move.x = -1;
            if (move.y > 0) move.y = 1;
            if (move.y < 0) move.y = -1;
        }
        
        Movement();
        Snapshot();
    }

    private void CreateReplicateData()
    {
        ReplicatedData data = new ReplicatedData()
        {
            Id = Id.Value,
            Pos = Pos.Value,
            Rot = Rot.Value,
        };
    }

    [Replicate]
    private void ReplicateInput(ReplicatedData data, ReplicateState state = ReplicateState.Invalid)
    {
        moveInput = move;
    }

    [Reconcile]
    private void ReconcileInput(Vector2 move)
    {
        
    }
    
    
    
    private void Movement()
    {
        
    }

    private void Snapshot()
    {
        
    }

    public byte[] Serialize()
    {
        var writer = new Writer();
        writer.Write(Id);
        writer.Write(Pos);
        writer.Write(Rot);
        writer.Write(Health);
        return writer.GetBuffer();
    }

    public RoleStateSnapshot Deserialize(byte[] data)
    {
        var reader = new Reader();
        reader.Initialize(data, null, Reader.DataSource.Unset);
        var target = new RoleStateSnapshot()
        {
            Id = reader.ReadInt32(),
            Pos = reader.ReadVector3(),
            Rot = reader.ReadQuaternion64(),
            Health = reader.ReadInt32(),
        };

        return target;
    }
}

public interface ISerializeAs<T>
{
    byte[] Serialize();
    T Deserialize(byte[] data);
}

public struct RoleStateSnapshot 
{
    public int Id;
    public Vector3 Pos;
    public Quaternion Rot;
    public int Health;
}
