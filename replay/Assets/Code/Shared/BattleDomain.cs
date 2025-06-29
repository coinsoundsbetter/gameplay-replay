using System;
using System.Collections.Generic;
using UnityEngine;

namespace KillCam
{
    public delegate void UpdateDelegate(float delta);

    public interface INetworkClient
    {
        void Send(INetworkSerialize data);
    }

    public interface INetworkServer
    {
        void Rpc(INetworkSerialize data);
    }
    
    public class BattleWorld
    {
        public string WorldName { get; private set; } 
        private readonly List<InitializeFeature> _initializeFeatures = new List<InitializeFeature>();
        private readonly Dictionary<Type, Feature> _features = new Dictionary<Type, Feature>();
        private readonly List<UpdateDelegate> updates = new List<UpdateDelegate>();
        private INetworkClient _networkClient;
        private INetworkServer _networkServer;
        
        public BattleWorld(string name)
        {
            WorldName = name;
        }

        public void SetNetworkClient(INetworkClient networkClient)
        {
            _networkClient = networkClient;
        }

        public void SetNetworkServer(INetworkServer networkServer)
        {
            _networkServer = networkServer;
        }

        public void Update()
        {
            foreach (var u in updates)
            {
                u.Invoke(Time.deltaTime);
            }
        }

        public void Dispose()
        {
            foreach (var f in _features.Values)
            {
                if (f is InitializeFeature)
                {
                    continue;
                }
                
                f.OnDestroy();
            }

            foreach (var _initialize in _initializeFeatures)
            {
                _initialize.OnDestroy();
            }
            
            _features.Clear();
            _initializeFeatures.Clear();
        }

        public void AddUpdate(UpdateDelegate updateDelegate)
        {
            updates.Add(updateDelegate);
        }

        public void RemoveUpdate(UpdateDelegate updateDelegate)
        {
            updates.Remove(updateDelegate);
        }

        public void Add(Feature feature)
        {
            var type = feature.GetType();
            if (!_features.ContainsKey(type))
            {
                _features.Add(type, feature);
                if (feature is InitializeFeature initializeFeature)
                {
                    _initializeFeatures.Add(initializeFeature);
                }
                feature.SetWorld(this);
                feature.OnCreate();
            }
        }

        public void Add<T>() where T : Feature, new()
        {
            var type = typeof(T);
            if (!_features.ContainsKey(type))
            {
                var feature = new T();
                feature.SetWorld(this);
                feature.OnCreate();
                _features.Add(type, feature);
            }
        }

        public T Get<T>() where T : Feature
        {
            var type = typeof(T);
            if (_features.TryGetValue(type, out var feature))
            {
                return (T)feature;
            }

            return default;
        }

        public void Send(INetworkSerialize request)
        {
            _networkClient?.Send(request);
        }

        public void Rpc(INetworkSerialize notify)
        {
            _networkServer?.Rpc(notify);
        }
    }
}