using System;
using System.Collections.Generic;

[System.Flags]
public enum WorldType {
    Undefined = 0,
    Client = 1 << 1,
    Server = 1 << 2,
    Replay = 1 << 3,
}

public class WorldBase {
    private readonly Dictionary<Type, IData> dataSet = new();
    private readonly Dictionary<Type, Feature> features = new();
    private readonly List<IUpdateable> updateableFeatures = new();
    private readonly List<ILateUpdateable> lateUpdateableFeatures = new();
    private readonly HashSet<Type> featureTypes = new();
    private readonly HashSet<Type> dataTypes = new();
    private WorldLink worldLink;

    public WorldBase() {
        worldLink = new WorldLink(this);
    }

    public void Initialize() {
        OnBeforeFeatureInitialize(new FeatureBuilder(featureTypes, dataTypes));
        foreach (var type in dataTypes) {
            var data = (IData)Activator.CreateInstance(type);
            dataSet.Add(type, data);
        }
        foreach (var type in featureTypes) {
            var feature = (Feature)Activator.CreateInstance(type);
            features.Add(type, feature);
            if (feature is IUpdateable updateable) {
                updateableFeatures.Add(updateable);
            }
            if (feature is ILateUpdateable lateUpdateable) {
                lateUpdateableFeatures.Add(lateUpdateable);
            }
        }
        foreach (var feature in features.Values) {
            feature.OnInitialize(ref worldLink);
        }
        OnAfterInitializeAllFeatures();
    }

    public void Clear() {
        OnBeforeFeatureClear();
        foreach (var feature in features.Values) {
            feature.OnClear();
        }
        features.Clear();
        updateableFeatures.Clear();
        lateUpdateableFeatures.Clear();
        dataSet.Clear();
        OnAfterFeatureClear();
    }

    public void Update() {
        OnBeforeFeatureUpdate();
        foreach (var feature in updateableFeatures) {
            feature.OnUpdate();
        }
        OnAfterFeatureUpdate();
    }

    public void LateUpdate() {
        OnBeforeFeatureLateUpdate();
        foreach (var feature in lateUpdateableFeatures) {
            feature.OnLateUpdate();
        }
        OnAfterFeatureLateUpdate();
    }
    
    protected virtual void OnBeforeFeatureInitialize(FeatureBuilder builder) { }
    protected virtual void OnAfterInitializeAllFeatures() { }
    
    protected virtual void OnBeforeFeatureUpdate() { }
    protected virtual void OnAfterFeatureUpdate() { }
    
    protected virtual void OnBeforeFeatureLateUpdate() { }
    protected virtual void OnAfterFeatureLateUpdate() { }
    
    protected virtual void OnBeforeFeatureClear() { }
    protected virtual void OnAfterFeatureClear() { }

    public T GetFeature<T>() where T : Feature {
        var type = typeof(T);
        if (features.TryGetValue(type, out Feature feature)) {
            return (T)feature;
        }

        return default;
    }

    public T GetData<T>() where T : IData {
        var type = typeof(T);
        if (dataSet.TryGetValue(type, out IData data)) {
            return (T)data;
        }

        return default;
    }
}