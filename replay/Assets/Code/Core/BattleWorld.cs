using System;
using System.Collections.Generic;

namespace KillCam {
    public class BattleWorld {
        public WorldFlag Flags { get; private set; }
        private readonly List<InitializeFeature> _initializeFeatures = new();
        private readonly Dictionary<Type, Feature> _features = new();
        private readonly List<LogicUpdateDelegate> _logicUpdates = new();
        private readonly List<FrameUpdateDelegate> _frameUpdates = new();
        public INetwork Network { get; private set; }
        public float FrameTickDelta { get; private set; }
        public double LogicTickDelta { get; private set; }

        public BattleWorld(WorldFlag flag) {
            Flags = flag;
        }

        public void SetNetwork(INetwork networkClient) {
            Network = networkClient;
        }

        public void RemoveNetwork(INetwork networkClient) {
            if (networkClient == Network) {
                Network = null;
            }
        }

        public void FrameUpdate(float delta) {
            FrameTickDelta = delta;
            foreach (var u in _frameUpdates) {
                u.Invoke(delta);
            }
        }

        public void LogicUpdate(double delta) {
            LogicTickDelta = delta;
            foreach (var u in _logicUpdates) {
                u.Invoke(delta);
            }
        }

        public void Dispose() {
            foreach (var f in _features.Values) {
                if (f is InitializeFeature) {
                    continue;
                }

                f.OnDestroy();
            }

            foreach (var _initialize in _initializeFeatures) {
                _initialize.OnDestroy();
            }

            _features.Clear();
            _initializeFeatures.Clear();
            _logicUpdates.Clear();
            _frameUpdates.Clear();
        }

        public void AddFrameUpdate(FrameUpdateDelegate frameUpdateDelegate) {
            _frameUpdates.Add(frameUpdateDelegate);
        }

        public void AddLogicUpdate(LogicUpdateDelegate logicUpdateDelegate) {
            _logicUpdates.Add(logicUpdateDelegate);
        }

        public void RemoveFrameUpdate(FrameUpdateDelegate frameUpdateDelegate) {
            _frameUpdates.Remove(frameUpdateDelegate);
        }

        public void RemoveLogicUpdate(LogicUpdateDelegate logicUpdateDelegate) {
            _logicUpdates.Remove(logicUpdateDelegate);
        }

        public void Add(Feature feature) {
            var type = feature.GetType();
            if (!_features.ContainsKey(type)) {
                _features.Add(type, feature);
                if (feature is InitializeFeature initializeFeature) {
                    _initializeFeatures.Add(initializeFeature);
                }

                feature.SetWorld(this);
                feature.OnCreate();
            }
        }

        public void Add<T>(Feature feature) {
            var type = typeof(T);
            if (!_features.ContainsKey(type)) {
                feature.SetWorld(this);
                feature.OnCreate();
                _features.Add(type, feature);
            }
        }

        public void Add<T>() where T : Feature, new() {
            var type = typeof(T);
            if (!_features.ContainsKey(type)) {
                var feature = new T();
                feature.SetWorld(this);
                feature.OnCreate();
                _features.Add(type, feature);
            }
        }

        public T Get<T>() where T : Feature {
            var type = typeof(T);
            if (_features.TryGetValue(type, out var feature)) {
                return (T)feature;
            }

            return default;
        }

        public void Send(INetworkSerialize request) {
            Network?.Send(request);
        }

        public void Rpc(INetworkSerialize notify) {
            Network?.Rpc(notify);
        }

        public bool HasFlag(WorldFlag check) {
            if ((Flags & check) != 0) {
                return true;
            }

            return false;
        }
    }
}