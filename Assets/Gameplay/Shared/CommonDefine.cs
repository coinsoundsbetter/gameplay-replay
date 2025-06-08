using System;
using System.Collections.Generic;

public interface IData { }

public class Feature {
    public virtual void OnInitialize(ref WorldLink link) { }
    public virtual void OnClear() { }
}

public interface IUpdateable {
    void OnUpdate();
}

public interface ILateUpdateable {
    void OnLateUpdate();
}

public struct WorldLink {
    private WorldBase belongWorld;

    public WorldLink(WorldBase belongWorld) {
        this.belongWorld = belongWorld;
    }

    public T RequireFeature<T>() where T : Feature {
        return belongWorld.GetFeature<T>();
    }

    public T RequireData<T>() where T : IData {
        return belongWorld.GetData<T>();
    }

    public WorldType GetWorldType() => WorldManager.Instance.GetWorldType(belongWorld);
}

public struct FeatureBuilder {
    private HashSet<Type> featureTypes;
    private HashSet<Type> dataTypes;

    public FeatureBuilder(HashSet<Type> featureTypesRef, HashSet<Type> dataTypesRef) {
        featureTypes = featureTypesRef;
        dataTypes = dataTypesRef;
    }
    
    public void RegisterFeature<T>() where T : Feature {
        featureTypes.Add(typeof(T));
    }

    public void RegisterData<T>() where T : IData {
        dataTypes.Add(typeof(T));
    }
}

public struct RemoteValue<T> {
    public T Local { get; set; }
    public T Remote { get; set; }

    public void Set(T value) {
        Local = value;
        Remote = value;
    }

    public void SetRemote(T value) {
        Remote = value;
    }
}